using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using factorio.Models;
using Newtonsoft.Json;
using Slugify;
using Docker.DotNet;

//https://hub.docker.com/v2/repositories/factoriotools/factorio/tags/
// TODO - concurrency
namespace factorio.Persistence
{
    public class ServerProvider : IServerProvider
    {
        private const string _base = @"C:\Users\mike\Desktop\opt";
        private DirectoryInfo _baseDirectory;
        private IEnumerable<Server> _servers;
        private ISlugHelper _slug;
        //private DockerClient _docker;

        public ServerProvider()
        {
            _baseDirectory = new DirectoryInfo(_base);
            _servers = loadFromFS(_baseDirectory);
            _slug = new SlugHelper(new SlugHelper.Config());

            //_docker = new DockerClientConfiguration(
            //    new Uri("http://localhost:9999")).CreateClient();
        }

        public IEnumerable<Server> getAll()
        {
            return _servers;
        }

        public Server getById(string slug)
        {
            return _servers.First(s => s.Slug == slug);
        }

        public string addServer(Server newServer)
        {
            string slug = _slug.GenerateSlug(newServer.Name);

            // verify uniqueness of slug
            if (_baseDirectory.EnumerateDirectories(slug).Count() > 0)
            {
                return "";
            }

            DirectoryInfo newDirectory = _baseDirectory.CreateSubdirectory(slug);

            // Create server-info.json
            using (StreamWriter w = new StreamWriter(Path.Combine(newDirectory.FullName, "server-info.json")))
            {
                ServerInfoFile sInfo = new ServerInfoFile()
                {
                    Name = newServer.Name,
                    Description = newServer.Description,
                    MajorVersion = newServer.MajorVersion,
                    MinorVersion = newServer.MinorVersion
                };
                w.Write(JsonConvert.SerializeObject(sInfo));
            }

            newServer.Slug = slug;
            _servers = _servers.Append(newServer);

            return slug;
        }

        public void updateServer(string slug, Server value)
        {
            // TODO - write to file system and update environments
            _servers = _servers.Where(s => s.Slug != slug).Append(value);
        }

        #region Loading Servers
        private IEnumerable<Server> loadFromFS(DirectoryInfo baseDirectory)
        {
            IEnumerable<Server> ret = new LinkedList<Server>();
            // TODO - errors?
            foreach (DirectoryInfo serverFolder in baseDirectory.EnumerateDirectories())
            {
                FileInfo[] file = serverFolder.GetFiles("server-info.json");

                if (file.Length > 0)
                {
                    ret = ret.Append(loadServerFromJSON(file[0]));
                }
                else
                {
                    ret = ret.Append(loadEmptyServer(serverFolder));
                }
            }

            return ret;
        }

        private Server loadServerFromJSON(FileInfo file)
        {

            ServerInfoFile sInfo = null;
            using (StreamReader r = new StreamReader(file.FullName))
            {
                sInfo = JsonConvert.DeserializeObject<ServerInfoFile>(r.ReadToEnd());
            }
            if (sInfo != null)
            {

                return new Server()
                {
                    Slug = file.Directory.Name,
                    Name = sInfo.Name,
                    Description = sInfo.Description,
                    MajorVersion = sInfo.MajorVersion.GetValueOrDefault(17),
                    MinorVersion = sInfo.MinorVersion,
                    Port = null,
                };
            }
            else
            {
                return loadEmptyServer(file.Directory);
            }
        }

        private Server loadEmptyServer(DirectoryInfo serverFolder)
        {
            return new Server()
            {
                Slug = serverFolder.Name,
                Name = serverFolder.Name,
                Description = "*** TODO ***",
                Port = null,
            };
        }

        private IEnumerable<Mod> loadServerMods(DirectoryInfo serverFolder)
        {
            DirectoryInfo[] modFolder = serverFolder.GetDirectories("mods");

            // TODO - load the mods folder

            return new LinkedList<Mod>();
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
