using Factorio.Persistence.Models;
using System.Collections.Generic;

namespace Factorio.Persistence.Interfaces
{
    public interface IInstanceProvider
    {
        IEnumerable<IInstance> getAll();
        IInstance getById(string key);
        void updateServer(string key, IInstance value);
        bool idExists(string slug);

        bool tryAddServer(IInstance value, out string newId);
    }
}