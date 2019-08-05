using Factorio.Models;
using Factorio.Persistence.Utils;
using Factorio.Services.Interfaces;
using Newtonsoft.Json;
using Slugify;
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

        public AbstractFileSystemInstanceProvider(DirectoryInfo baseDir)
        {
            _slug = new SlugHelper(new SlugHelper.Config());
            _baseDirectory = baseDir;
        }

        public abstract IReadOnlyDictionary<string, string> GetImplementationInfo(string key);

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

        public bool TryAddServer(GameInstance newServer, out string newId)
        {
            newId = _slug.GenerateSlug(newServer.Name);

            // verify uniqueness of slug
            if (IdExists(newId))
            {
                newId = "";
                return false;
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

            return true;
        }

        public void UpdateServer(string slug, GameInstance value)
        {
            // TODO - write to file system and update environments
            DirectoryInfo d = GetServerDirectory(slug);

            if (!d.Exists)
            {
                // TODO - Error
                return;
            }

            using (StreamWriter w = new StreamWriter(Path.Combine(d.FullName, SERVER_INFO_FILE_NAME)))
            {
                ServerInfoFile sInfo = new ServerInfoFile()
                {
                    Name = value.Name,
                    Description = value.Description,
                    MajorVersion = value.TargetVersion.Major,
                    MinorVersion = value.TargetVersion.Minor,
                    PatchVersion = value.TargetVersion.Patch
                };
                w.Write(JsonConvert.SerializeObject(sInfo));
            }
        }

        private DirectoryInfo GetServerDirectory(string slug)
        {
            return new DirectoryInfo(Path.Combine(this._baseDirectory.FullName, slug));
        }

        #region Loading Servers
        private GameInstance LoadSingleDirectory(DirectoryInfo d)
        {
            GameSave lastSave = LoadActiveSave(d);
            IList<Mod> modList = LoadModList(d);


            GameInstance ret;

            FileInfo gameInfo = GetServerInfo(d);
            if (gameInfo != null)
            {
                ret = LoadServerFieldsFromJSON(gameInfo);
            }
            else
            {
                ret = LoadEmptyServer(d);
            }

            ret.LastSave = lastSave;
            ret.Mods = modList;

            return ret;
        }

        private IList<Mod> LoadModList(DirectoryInfo d)
        {
            List<Mod> ret = new List<Mod>();

            DirectoryInfo modsDir = d.EnumerateDirectories("mods").FirstOrDefault();
            if (modsDir == null) return new List<Mod>();
            return new List<Mod>(modsDir.EnumerateFiles("*.zip").Select<FileInfo, Mod>(modZip =>
            {
                Match regexMathc = MOD_ZIP_REGEX.Match(modZip.Name);
                string modName = regexMathc.Groups[1].Value;
                string majorVersion = regexMathc.Groups[2].Value;
                string minorVersion = regexMathc.Groups[3].Value;
                string patchVersion = regexMathc.Groups[4].Value;

                return new Mod
                {
                    Name = modName,
                    Version = new SpecificVersion
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

        /// <summary>
        /// Given a game directory, returns the server-info.json file loaded
        /// </summary>
        /// <param name="d">Directory of the game containing the saves, mods, and more</param>
        /// <returns>The FileInfo of the server-info.json file</returns>
        private static FileInfo GetServerInfo(DirectoryInfo d)
        {
            FileInfo[] file = d.GetFiles(SERVER_INFO_FILE_NAME);
            if (file.Length > 0)
            {
                return file[0];
            }
            return null;
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
                Description = "",
                ImplementationInfo = GetImplementationInfo(serverFolder.Name)
            };
        }

        /// <summary>
        /// Loads fields it can from the server-info.json file
        /// </summary>
        /// <param name="file">The server-info.json file</param>
        /// <param name="s">The output server object</param>
        private GameInstance LoadServerFieldsFromJSON(FileInfo serverInfoFile)
        {

            ServerInfoFile sInfo = null;
            using (StreamReader r = new StreamReader(serverInfoFile.FullName))
            {
                sInfo = JsonConvert.DeserializeObject<ServerInfoFile>(r.ReadToEnd());
            }
            if (sInfo == null)
            {
                return null; // TODO
            }
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
                ImplementationInfo = GetImplementationInfo(serverInfoFile.Directory.Name)
            };
        }
        #endregion

        #region On disk files
        private class ServerInfoFile
        {
            public string Name;
            public string Description;
            public int? MajorVersion;
            public int? MinorVersion;
            public int? PatchVersion;
        }

        private class ModFile
        {
            public List<ModEntry> Mods;
        }

        private class ModEntry
        {
            public string Name;
            public bool Enabled;
        }
        #endregion

    }
}
