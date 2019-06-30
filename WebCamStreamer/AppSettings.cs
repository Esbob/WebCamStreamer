using System.Collections.Generic;

namespace WebCamStreamer
{
    public class AppSettings
    {
        public List<WebCamSetting> WebCamSettings { get; set; } = new List<WebCamSetting>();
        public string SourcePipeline { get; set; }
        public string SinkPipeline { get; set; }
        public string ServiceHostBaseAddress { get; set; }
    }
}
