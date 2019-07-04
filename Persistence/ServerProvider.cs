using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using factorio.Models;
using Newtonsoft.Json;
using Slugify;
using Docker.DotNet;
using System.IO.Compression;

//https://hub.docker.com/v2/repositories/factoriotools/factorio/tags/
// TODO - concurrency
namespace factorio.Persistence
{
    public class ServerProvider : IServerProvider
    {
        // Constants
        private const string SERVER_INFO_FILE_NAME = "server-info.json";

        // Static config
        private const string _base = @"C:\Users\mike\Desktop\opt";

        private DirectoryInfo _baseDirectory;
        private ISlugHelper _slug;

        public ServerProvider()
        {
            _baseDirectory = new DirectoryInfo(_base);
            _slug = new SlugHelper(new SlugHelper.Config());
        }

        public IEnumerable<Server> getAll()
        {
            return _baseDirectory.EnumerateDirectories().Select(d => loadSingleDirectory(d));
        }

        public Server getById(string slug)
        {
            DirectoryInfo d = GetDirectory(slug);

            if (d.Exists)
            {
                return loadSingleDirectory(d);
            }
            else
            {
                return null;
            }
        }

        public bool idExists(string slug)
        {
            DirectoryInfo d = GetDirectory(slug);
            return d.Exists;
        }

        public bool tryAddServer(Server newServer, out string newId)
        {
            newId = _slug.GenerateSlug(newServer.Name);

            // verify uniqueness of slug
            if (_baseDirectory.EnumerateDirectories(newId).Count() > 0)
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
                    MajorVersion = newServer.TargetMajorVersion,
                    MinorVersion = newServer.TargetMinorVersion
                };
                w.Write(JsonConvert.SerializeObject(sInfo));
            }

            return true;
        }

        public void updateServer(string slug, Server value)
        {
            // TODO - write to file system and update environments
            DirectoryInfo d = GetDirectory(slug);

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
                    MajorVersion = value.TargetMajorVersion,
                    MinorVersion = value.TargetMinorVersion
                };
                w.Write(JsonConvert.SerializeObject(sInfo));
            }
        }

        private DirectoryInfo GetDirectory(string slug)
        {
            return new DirectoryInfo(Path.Combine(this._baseDirectory.FullName, slug));
        }

        #region Loading Servers
        private Server loadSingleDirectory(DirectoryInfo d)
        {
            Server ret = loadEmptyServer(d);

            FileInfo gameInfo = getServerInfo(d);
            if (gameInfo != null)
            {
                loadServerFieldsFromJSON(gameInfo, ret);
            }

            FileInfo gameSave = getActiveGameSave(d);
            if (gameSave != null)
            {
                loadServerFieldsFromGameSave(gameSave, ret);
            }

            return ret;
        }

        /// <summary>
        ///     Given a game directory, returns the save which gets loaded
        /// </summary>
        /// <param name="d">Directory of the game containing the saves, mods, and more</param>
        /// <returns>The FileInfo of the zip-file for the active save</returns>
        private static FileInfo getActiveGameSave(DirectoryInfo d)
        {
            DirectoryInfo[] directories = d.GetDirectories("saves");

            if (directories.Length > 0)
            {
                FileInfo[] gameSaves = directories[0].GetFiles();
                return gameSaves.OrderByDescending(fInfo => fInfo.LastWriteTime).FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// Given a game directory, returns the server-info.json file loaded
        /// </summary>
        /// <param name="d">Directory of the game containing the saves, mods, and more</param>
        /// <returns>The FileInfo of the server-info.json file</returns>
        private static FileInfo getServerInfo(DirectoryInfo d)
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
        private Server loadEmptyServer(DirectoryInfo serverFolder)
        {
            return new Server()
            {
                Slug = serverFolder.Name,
                Name = serverFolder.Name,
                Description = "*** TODO ***",
            };
        }

        /// <summary>
        /// Loads fields it can from the server-info.json file
        /// </summary>
        /// <param name="file">The server-info.json file</param>
        /// <param name="s">The output server object</param>
        private void loadServerFieldsFromJSON(FileInfo serverInfoFile, Server s)
        {

            ServerInfoFile sInfo = null;
            using (StreamReader r = new StreamReader(serverInfoFile.FullName))
            {
                sInfo = JsonConvert.DeserializeObject<ServerInfoFile>(r.ReadToEnd());
            }
            if (sInfo != null)
            {
                s.Slug = serverInfoFile.Directory.Name;
                s.Name = sInfo.Name;
                s.Description = sInfo.Description;
                s.TargetMajorVersion = sInfo.MajorVersion.GetValueOrDefault(17);
                s.TargetMinorVersion = sInfo.MinorVersion;
            }
        }

        /// <summary>
        /// Loads fields from the game save zip file
        /// </summary>
        /// <param name="gameSaveFile">The game save zip file</param>
        /// <param name="server">The putput server</param>
        private void loadServerFieldsFromGameSave(FileInfo gameSaveFile, Server server)
        {
            // TODO
            using (ZipArchive archive = ZipFile.OpenRead(gameSaveFile.FullName))
            {
                ZipArchiveEntry levelDAT = null;

                foreach (ZipArchiveEntry e in archive.Entries)
                {
                    if (e.FullName.EndsWith("level.dat"))
                    {
                        levelDAT = e;
                    }
                }

                if (levelDAT == null)
                {
                    return;
                }

                using (BinaryReader r = new BinaryReader(levelDAT.Open()))
                {
                    r.ReadInt16();
                    server.LastSaveMajorVersion = r.ReadInt16();
                    server.LastSaveMinorVersion = r.ReadInt16();
                }
            }
        }
        #endregion

        #region On disk files
        private class ServerInfoFile
        {
            public string Name;
            public string Description;
            public int? MajorVersion;
            public int? MinorVersion;
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
