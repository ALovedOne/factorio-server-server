using Factorio.Models;
using Factorio.Persistence;
using Factorio.Services.Interfaces;
using Factorio.Services.Persistence.FileSystems;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Xunit;

namespace Factorio.Test
{
    public class Version
    {

        [Theory]
        [InlineData(0, 6, 6)]
        [InlineData(1, 4, 6)]
        [InlineData(1, 5, 4)]
        [InlineData(1, 5, 5)]
        public void Version_Specific_IsLessThanOrEqual(int major, int minor, int patch)
        {
            SpecificVersion a = new SpecificVersion { Major = major, Minor = minor, Patch = patch };
            SpecificVersion b = new SpecificVersion { Major = 1, Minor = 5, Patch = 5 };

            Assert.True(a <= b);
        }

        [Theory]
        [InlineData(2, 4, 4)]
        [InlineData(1, 6, 4)]
        [InlineData(1, 5, 6)]
        public void Version_Specific_IsNotLessThanOrEqual(int major, int minor, int patch)
        {
            SpecificVersion a = new SpecificVersion { Major = major, Minor = minor, Patch = patch };
            SpecificVersion b = new SpecificVersion { Major = 1, Minor = 5, Patch = 5 };

            Assert.False(a <= b);
        }

        [Fact]
        public void Verion_Fuzzy_FuzzyIsNotLessThanSpecific()
        {
            FuzzyVersion a = new FuzzyVersion { Major = 1, Minor = 5, Patch = null };
            SpecificVersion b = new SpecificVersion { Major = 1, Minor = 5, Patch = 5 };

            Assert.False(a <= b);
        }


        [Fact]
        public void Verion_Fuzzy_SpecificIsLessThanFuzzy()
        {
            SpecificVersion a = new SpecificVersion { Major = 1, Minor = 5, Patch = 5 };
            FuzzyVersion b = new FuzzyVersion { Major = 1, Minor = 5, Patch = null };

            Assert.True(a <= b);
        }


        [Theory]

        [InlineData(2, 4, 4)]
        [InlineData(1, 6, 4)]
        [InlineData(1, 5, 6)]
        [InlineData(1, 5, 5)]
        public void Version_Specific_IsGreaterThanOrEqual(int major, int minor, int patch)
        {
            SpecificVersion a = new SpecificVersion { Major = major, Minor = minor, Patch = patch };
            SpecificVersion b = new SpecificVersion { Major = 1, Minor = 5, Patch = 5 };

            Assert.True(a >= b);
        }

        [Theory]
        [InlineData(0, 6, 6)]
        [InlineData(1, 4, 6)]
        [InlineData(1, 5, 4)]
        public void Version_Specific_IsNotGreaterThanOrEqual(int major, int minor, int patch)
        {
            SpecificVersion a = new SpecificVersion { Major = major, Minor = minor, Patch = patch };
            SpecificVersion b = new SpecificVersion { Major = 1, Minor = 5, Patch = 5 };

            Assert.False(a >= b);
        }

        [Fact]
        public void Verion_Fuzzy_FuzzyIsGreaterThanSpecific()
        {
            FuzzyVersion a = new FuzzyVersion { Major = 1, Minor = 5, Patch = null };
            SpecificVersion b = new SpecificVersion { Major = 1, Minor = 5, Patch = 5 };

            Assert.True(a >= b);
        }


        [Fact]
        public void Verion_Fuzzy_SpecificIsNotGreaterThanFuzzy()
        {
            SpecificVersion a = new SpecificVersion { Major = 1, Minor = 5, Patch = 5 };
            FuzzyVersion b = new FuzzyVersion { Major = 1, Minor = 5, Patch = null };

            Assert.False(a >= b);
        }


    }
}
