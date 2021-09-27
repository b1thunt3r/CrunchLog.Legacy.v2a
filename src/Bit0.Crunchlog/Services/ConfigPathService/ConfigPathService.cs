using Bit0.CrunchLog.Sdk.Services.ConfigService;

namespace Bit0.CrunchLog.Services.ConfigPathService
{
    public class ConfigPathService
    {
        private readonly ILogger<ConfigPathService> _logger;
        private readonly ConfigService _configService;

        public ConfigPathService(ILogger<ConfigPathService> logger, ConfigService configService)
        {
            _logger = logger;
            _configService = configService;
        }
    }
}
