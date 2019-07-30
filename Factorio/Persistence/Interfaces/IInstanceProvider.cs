using Factorio.Persistence.Models;
using System.Collections.Generic;

namespace Factorio.Persistence.Interfaces
{
    public interface IInstanceProvider
    {
        IEnumerable<IInstance> GetAll();
        IInstance GetById(string key);
        void UpdateServer(string key, IInstance value);
        bool IdExists(string slug);

        bool TryAddServer(IInstance value, out string newId);
    }
}