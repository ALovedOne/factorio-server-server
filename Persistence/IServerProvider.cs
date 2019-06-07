using factorio.Models;
using System.Collections.Generic;

namespace factorio.Persistence
{
    public interface IServerProvider
    {
        IEnumerable<Server> getAll();
        Server getById(string  slug);
        string addServer(Server value);
        void updateServer(string slug, Server value);
    }
}