using System.IO;
using Microsoft.Extensions.Configuration;

namespace WebCamStreamer
{
    public class AppSettingsReader
    {
        public static AppSettings GetAppSettings()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var appSettings = new AppSettings();
            config.Bind(appSettings);
            return appSettings;
        }
    }
}
