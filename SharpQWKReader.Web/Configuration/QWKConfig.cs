using Microsoft.Extensions.Configuration;

namespace SharpQWKReader.Web.Configuration
{
    public class QWKConfig : IQWKConfig
    {
        private readonly IConfiguration _configuration;

        public QWKConfig(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string TempDirectory => _configuration["QWK:TempDirectory"] ?? "uploads/qwk_temp/";
    }
}
