using System;
using Gst;

namespace WebCamStreamer
{
    public class WebCamController : IWebCamController
    {
        private readonly Element _pipeline;
        private readonly AppSettings _appSettings;
        private const string SelectorName = "selector";
        private const string SinkSelector = "sink_{0}";

        public WebCamController()
        {
            _appSettings = AppSettingsReader.GetAppSettings();
            Application.Init();
            _pipeline = Parse.Launch(CreatePipeline());
        }

        private string CreatePipeline()
        {
            var pipeline = String.Empty;

            for (var index = 0; index < _appSettings.WebCamSettings.Count; index++)
            {
                var webCamSetting = _appSettings.WebCamSettings[index];
                pipeline += GetFormattedPipelineSource(webCamSetting, index) + Environment.NewLine;
            }

            pipeline += _appSettings.SinkPipeline;
            return pipeline;
        }

        private string GetFormattedPipelineSource(WebCamSetting webCamSetting, int index)
        {
            var s = String.Format(_appSettings.SourcePipeline, webCamSetting.DeviceId, webCamSetting.Name, index);
            return s;
        }
        
        public string ChangeWebCam(string webCamName)
        {
            var selector = ((Bin)_pipeline).GetByName(SelectorName);
            var padName = String.Format(SinkSelector, ConvertWebCamNameToSinkIndex(webCamName));
            var newPad = selector.GetStaticPad(padName);
            selector["active-pad"] = newPad;
            return webCamName;
        }

        private int ConvertWebCamNameToSinkIndex(string webCamName)
        {
            return _appSettings.WebCamSettings.FindIndex(setting => setting.Name == webCamName);
        }

        public bool StartStreaming()
        {
            _pipeline.ChangeState(StateChange.NullToReady);
            _pipeline.ChangeState(StateChange.ReadyToPaused);
            _pipeline.ChangeState(StateChange.PausedToPlaying);

            return true;
        }

        public bool StopStreaming()
        {
            _pipeline.ChangeState(StateChange.PlayingToPaused);
            _pipeline.ChangeState(StateChange.PausedToReady);
            _pipeline.ChangeState(StateChange.ReadyToNull);

            return true;
        }
    }
}