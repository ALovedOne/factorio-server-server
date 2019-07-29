using factorio.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace factorio.Persistence
{
    public interface IInstanceProvider
    {
        IEnumerable<Server> getAll();
        Server getById(string  slug);
        void updateServer(string slug, Server value);
        bool idExists(string slug);
        bool tryAddServer(Server value, out string newId);
    }
}