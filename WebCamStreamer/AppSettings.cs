using System.Collections.Generic;

namespace WebCamStreamer
{
    public class AppSettings
    {
        public List<WebCamSetting> WebCamSettings { get; set; } = new List<WebCamSetting>();
        public string VideoSourcePipeline { get; set; }
        public string VideoSinkPipeline { get; set; }
        public string AudioSourcePipeline { get; set; }
        public string AudioSinkPipeline { get; set; }
        public string ServiceHostBaseAddress { get; set; }
    }
}
