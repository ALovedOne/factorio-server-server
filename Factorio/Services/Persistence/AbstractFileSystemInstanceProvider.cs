using Factorio.Models;
using Factorio.Persistence.Utils;
using Factorio.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Slugify;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Factorio.Services.Persistence
{
    public abstract class AbstractFileSystemInstanceProvider : IInstanceProvider
    {
        // Constants
        private const string SERVER_INFO_FILE_NAME = "server-info.json";

        private Regex MOD_ZIP_REGEX = new Regex(@"(.*)_(\d+)\.(\d+)\.(\d+)\.zip");

        // Static Config
        protected readonly DirectoryInfo _baseDirectory;
        private readonly ISlugHelper _slug;
        private readonly ILogger _logger;

        public AbstractFileSystemInstanceProvider(DirectoryInfo baseDir, ILogger logger)
        {
            _slug = new SlugHelper(new SlugHelper.Config());
            _baseDirectory = baseDir;
            _logger = logger;
        }

        public abstract string ConfigBaseDir(string key);

        public IEnumerable<GameInstance> GetAll()
        {
            if (_baseDirectory.Exists)
            {
                return _baseDirectory.EnumerateDirectories().Select(d => LoadSingleDirectory(d));
            }
            else
            {
                return new GameInstance[0];
            }
        }

        public GameInstance GetById(string slug)
        {
            DirectoryInfo d = GetServerDirectory(slug);

            if (d.Exists)
            {
                return LoadSingleDirectory(d);
            }
            else
            {
                return null;
            }
        }

        public bool IdExists(string slug)
        {
            DirectoryInfo d = GetServerDirectory(slug);
            return d.Exists;
        }

        public GameInstance TryAddServer(GameInstance newServer)
        {
            string newId = _slug.GenerateSlug(newServer.Name);

            // verify uniqueness of slug
            if (IdExists(newId))
            {
                return null;
            }

            DirectoryInfo newDirectory = _baseDirectory.CreateSubdirectory(newId);

            // Create server-info.json
            using (StreamWriter w = new StreamWriter(Path.Combine(newDirectory.FullName, SERVER_INFO_FILE_NAME)))
            {
                ServerInfoFile sInfo = new ServerInfoFile()
                {
                    Name = newServer.Name,
                    Description = newServer.Description,
                    MajorVersion = newServer.TargetVersion.Major,
                    MinorVersion = newServer.TargetVersion.Minor,
                    PatchVersion = newServer.TargetVersion.Patch
                };
                w.Write(JsonConvert.SerializeObject(sInfo));
            }
            // TODO -create folders structure

            // Just reload, ignore the performance hit
            return GetById(newId); ;
        }

        public GameInstance UpdateServer(GameInstance value)
        {
            // TODO - write to file system and update environments
            DirectoryInfo d = GetServerDirectory(value.Key);

            if (!d.Exists)
            {
                // TODO - Error
                return null;
            }

            using (StreamWriter w = new StreamWriter(Path.Combine(d.FullName, SERVER_INFO_FILE_NAME)))
            {
                ServerInfoFile sInfo = new ServerInfoFile()
                {
                    Name = value.Name,
                    Description = value.Description,
                    MajorVersion = value.TargetVersion.Major,
                    MinorVersion = value.TargetVersion.Minor,
                    PatchVersion = value.TargetVersion.Patch,
                    Mods = value.Mods
                };
                w.Write(JsonConvert.SerializeObject(sInfo));
            }

            return GetById(value.Key);
        }

        private DirectoryInfo GetServerDirectory(string slug)
        {
            return new DirectoryInfo(Path.Combine(this._baseDirectory.FullName, slug));
        }

        #region Loading Servers
        /// <summary>
        /// Load a single directory from server-info.json or a default
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private GameInstance LoadSingleDirectory(DirectoryInfo d)
        {
            GameSave lastSave = LoadActiveSave(d);
            IList<TargetMod> modFolderList = LoadModModsFolderList(d);

            GameInstance gameInfo = GetServerInstanceFromFile(d) ?? LoadEmptyServer(d);
            gameInfo.LastSave = lastSave;

            // First defined, 
            gameInfo.Mods = gameInfo.Mods ?? modFolderList ?? lastSave?.Mods.Select(m => (TargetMod)m);

            return gameInfo;
        }

        private IList<TargetMod> LoadModModsFolderList(DirectoryInfo d)
        {
            DirectoryInfo modsDir = d.EnumerateDirectories("mods").FirstOrDefault();
            if (modsDir == null) return new List<TargetMod>();
            return new List<TargetMod>(modsDir.EnumerateFiles("*.zip").Select<FileInfo, TargetMod>(modZip =>
            {
                Match regexMathc = MOD_ZIP_REGEX.Match(modZip.Name);
                string modName = regexMathc.Groups[1].Value;
                string majorVersion = regexMathc.Groups[2].Value;
                string minorVersion = regexMathc.Groups[3].Value;
                string patchVersion = regexMathc.Groups[4].Value;

                return new TargetMod
                {
                    Name = modName,
                    TargetVersion = new FuzzyVersion
                    {
                        Major = byte.Parse(majorVersion),
                        Minor = byte.Parse(minorVersion),
                        Patch = byte.Parse(patchVersion)
                    }
                };
            }));
        }

        private GameSave LoadActiveSave(DirectoryInfo d)
        {
            FileInfo gameSaveFile = GetActiveGameSave(d);
            if (gameSaveFile == null) return null;

            return SaveFolderParser.ParserZipFile(gameSaveFile);
        }

        /// <summary>
        ///     Given a game directory, returns the save which gets loaded
        /// </summary>
        /// <param name="d">Directory of the game containing the saves, mods, and more</param>
        /// <returns>The FileInfo of the zip-file for the active save</returns>
        private static FileInfo GetActiveGameSave(DirectoryInfo d)
        {
            DirectoryInfo[] directories = d.GetDirectories("saves");

            if (directories.Length > 0)
            {
                FileInfo[] gameSaves = directories[0].GetFiles("*.zip");
                return gameSaves.OrderByDescending(fInfo => fInfo.LastWriteTime).FirstOrDefault();
            }
            return null;
        }

        private GameInstance GetServerInstanceFromFile(DirectoryInfo d)
        {
            FileInfo[] file = d.GetFiles(SERVER_INFO_FILE_NAME);
            if (file.Length != 1)
                return null;

            FileInfo serverInfoFile = file[0];
            using (StreamReader r = new StreamReader(serverInfoFile.FullName))
            {
                ServerInfoFile sInfo = JsonConvert.DeserializeObject<ServerInfoFile>(r.ReadToEnd());
                if (sInfo == null) return null;

                return new GameInstance
                {
                    Key = serverInfoFile.Directory.Name,
                    Name = sInfo.Name,
                    Description = sInfo.Description,
                    TargetVersion = new FuzzyVersion
                    {
                        Major = sInfo.MajorVersion.GetValueOrDefault(0),
                        Minor = sInfo.MinorVersion.GetValueOrDefault(17),
                        Patch = sInfo.PatchVersion
                    },
                    Mods = sInfo.Mods,
                    ConfigUrl = GetConfigUrl(serverInfoFile.Directory.Name),
                    LastSaveUrl = GetLastSaveUrl(serverInfoFile.Directory.Name)
                };
            }
        }

        /// <summary>
        /// Creates a blank Server object based on the directory
        /// </summary>
        /// <param name="serverFolder">The game folder</param>
        /// <returns>A partial Server object</returns>
        private GameInstance LoadEmptyServer(DirectoryInfo serverFolder)
        {
            return new GameInstance()
            {
                Key = serverFolder.Name,
                Name = serverFolder.Name,
                TargetVersion = new FuzzyVersion { Major = 0, Minor = 17, Patch = null },
                Description = "",
                ConfigUrl = GetConfigUrl(serverFolder.Name),
                LastSaveUrl = GetLastSaveUrl(serverFolder.Name)
            };
        }
        #endregion

        private Uri GetConfigUrl(string gameKey)
        {
            return new Uri("file:///" + ConfigBaseDir(gameKey));
        }

        private Uri GetLastSaveUrl(string gameKey)
        {
            FileInfo lastSave = GetActiveGameSave(GetServerDirectory(gameKey));
            if (lastSave == null) return null;
            return new Uri("file:///" + Path.Combine(ConfigBaseDir(gameKey), "saves", lastSave.Name));
        }

        #region On disk files
        private class ServerInfoFile
        {
            public string Name;
            public string Description;
            public int? MajorVersion;
            public int? MinorVersion;
            public int? PatchVersion;
            public IEnumerable<TargetMod> Mods;
        }

        private class ModFile
        {
            public List<ModEntry> Mods = new List<ModEntry>();
        }

        private class ModEntry
        {
            public string Name = "";
            public bool Enabled = true;
        }
        #endregion
    }
}
