using Factorio.Models;
using System.Collections.Generic;

namespace Factorio.Services.Interfaces
{
    public interface IInstanceProvider
    {
        IEnumerable<GameInstance> GetAll();
        GameInstance GetById(string key);
        GameInstance UpdateServer(GameInstance value);
        GameInstance TryAddServer(GameInstance value);
        bool IdExists(string key);
    }
}