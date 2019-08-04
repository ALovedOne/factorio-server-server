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

        [Theory]
        [InlineData(0, 6, 6)]
        [InlineData(1, 4, 6)]
        [InlineData(1, 5, 5)]
        public void Valid_Version_FullyDefined_IsValid(int major, int minor, int patch)
        {
            GameInstance g = new GameInstance
            {
                LastSave = new GameSave(major, minor, patch),
                TargetMajorVersion = 1,
                TargetMinorVersion = 5,
                TargetPatchVersion = 5
            };

            Assert.True(g.Valid);
        }


        [Theory]
        [InlineData(2, 4, 4)]
        [InlineData(1, 6, 4)]
        [InlineData(1, 5, 6)]
        public void Valid_Version_FullyDefined_IsNotValid(int major, int minor, int patch)
        {
            GameInstance g = new GameInstance
            {
                LastSave = new GameSave(major, minor, patch),
                TargetMajorVersion = 1,
                TargetMinorVersion = 5,
                TargetPatchVersion = 5
            };

            Assert.False(g.Valid);
        }

        [Theory]
        [InlineData(0,6,10)]
        [InlineData(1,5,10)]
        public void Valid_Version_LatestPatch_IsValid(int major, int minor, int patch)
        {
            GameInstance g = new GameInstance
            {
                LastSave = new GameSave(major, minor, patch),
                TargetMajorVersion = 1,
                TargetMinorVersion = 5,
                TargetPatchVersion = null
            };

            Assert.True(g.Valid);
        }

        [Theory]
        [InlineData(2,4,4)]
        [InlineData(1,6,4)]
        public void Valid_version_latestPatch_IsNotValid(int major, int minor, int patch)
        {
            GameInstance g = new GameInstance
            {
                LastSave = new GameSave(major, minor, patch),
                TargetMajorVersion = 1,
                TargetMinorVersion = 5,
                TargetPatchVersion = null
            };

            Assert.False(g.Valid);
        }
    }
}
