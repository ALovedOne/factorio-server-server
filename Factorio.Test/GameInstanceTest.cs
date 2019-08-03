using Factorio.Models;
using Factorio.Persistence;
using Factorio.Services.Interfaces;
using Factorio.Services.Persistence.LocalInstanceProvider;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Xunit;

namespace Factorio.Test
{
    public class GameInstanceTest
    {

        [Fact]
        public void TestMajorVersion()
        {
            GameInstance g = new GameInstance
            {
                LastSave = null,
                TargetMajorVersion = 2,
                TargetMinorVersion = 5,
                TargetPatchVersion = 5
            };
            Assert.True(g.Valid, "Last Save null");

            g.LastSave = new GameSave(1, 5, 0, new List<Mod>());
            Assert.True(g.Valid, "Last Save = 1, target = 2");

            g.LastSave = new GameSave(2, 5, 0, new List<Mod>());
            Assert.True(g.Valid, "Last Save = 2, target = 2");

            g.LastSave = new GameSave(3, 5, 0, new List<Mod>());
            Assert.False(g.Valid, "Last Save = 3, target = 2");
        }

        [Fact]
        public void TestMinorVersion()
        {
            GameInstance g = new GameInstance
            {
                LastSave = null,
                TargetMajorVersion = 2,
                TargetMinorVersion = 5,
                TargetPatchVersion = 5
            };
            Assert.True(g.Valid, "Last Save null");

            g.LastSave = new GameSave(1, 6, 0, new List<Mod>());
            Assert.True(g.Valid, "Last save = 1.6, target = 2.5");

            g.LastSave = new GameSave(2, 4, 0, new List<Mod>());
            Assert.True(g.Valid, "Last save = 2.4, target = 2.5");

            g.LastSave = new GameSave(2, 5, 0, new List<Mod>());
            Assert.True(g.Valid, "Last save = 2.5, target = 2.5");

            g.LastSave = new GameSave(2, 6, 0, new List<Mod>());
            Assert.False(g.Valid, "Last save = 2.6, target = 2.5");
        }

        [Fact]
        public void TestPatchVersionNotNull()
        {
            GameInstance g = new GameInstance
            {
                LastSave = null,
                TargetMajorVersion = 2,
                TargetMinorVersion = 5,
                TargetPatchVersion = 5
            };

            Assert.True(g.Valid, "Last Save null");

            g.LastSave = new GameSave(2, 4, 6, new List<Mod>());
            Assert.True(g.Valid, "Last save 2.4.6, target = 2.5.5");

            g.LastSave = new GameSave(2, 5, 4, new List<Mod>());
            Assert.True(g.Valid, "Last save 2.5.4, target = 2.5.5");

            g.LastSave = new GameSave(2, 5, 5, new List<Mod>());
            Assert.True(g.Valid, "Last save 2.5.5, target = 2.5.5");

            g.LastSave = new GameSave(2, 5, 6, new List<Mod>());
            Assert.False(g.Valid, "Last save 2.5.6, target = 2.5.5");
        }

        [Fact]
        public void TestPatchVersionNull()
        {
            GameInstance g = new GameInstance
            {
                LastSave = null,
                TargetMajorVersion = 2,
                TargetMinorVersion = 5,
                TargetPatchVersion = null
            };
            Assert.True(g.Valid, "Last save null");

            g.LastSave = new GameSave(2, 5, 4, new List<Mod>());
        }

    }
}
