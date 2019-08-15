using Factorio.Models;
using Factorio.Services.Interfaces;
using Factorio.Services.Persistence.LocalInstanceProvider;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Factorio.Test
{
    public class TestWithInstanceProvider : TestWithTempDir
    {
        protected IInstanceProvider _provider;

        public TestWithInstanceProvider() : base()
        {
            IOptions<LocalInstanceOptions> options = Options.Create(new LocalInstanceOptions { BaseDirectory = FullPath });
            ILogger<LocalInstanceProvider> logger = new Mock<ILogger<LocalInstanceProvider>>().Object;
            _provider = new LocalInstanceProvider(options, logger);
        }

        protected GameInstance GetGameInstance(string key)
        {
            return _provider.GetById(key);
        }
    }
}
