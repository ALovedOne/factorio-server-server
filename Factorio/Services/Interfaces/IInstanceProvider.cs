using Factorio.Models;
using System.Collections.Generic;

namespace Factorio.Services.Interfaces
{
    public interface IInstanceProvider
    {
        IEnumerable<GameInstance> GetAll();
        GameInstance GetById(string key);
        void UpdateServer(string key, GameInstance value);
        bool IdExists(string slug);

        bool TryAddServer(GameInstance value, out string newId);
    }
}