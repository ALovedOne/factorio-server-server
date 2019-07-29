using Factorio.Persistence.Models;
using System.Collections.Generic;

namespace Factorio.Persistence
{
    public interface IInstanceProvider
    {
        IEnumerable<IInstance> getAll();
        Instance getById(string  slug);
        void updateServer(string slug, Instance value);
        bool idExists(string slug);
        bool tryAddServer(Instance value, out string newId);
    }
}