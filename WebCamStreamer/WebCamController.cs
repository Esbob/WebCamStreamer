using System;
using Gst;

namespace WebCamStreamer
{
    public class WebCamController : IWebCamController
    {
        private readonly Element _videoPipeline;
        private readonly Element _audioPipeline;
        private readonly AppSettings _appSettings;
        private const string SelectorName = "selector";
        private const string SinkSelector = "sink_{0}";

        public WebCamController()
        {
            _appSettings = AppSettingsReader.GetAppSettings();
            Application.Init();
            _videoPipeline = Parse.Launch(CreateVideoPipeline());
            _audioPipeline = Parse.Launch(CreateAudioPipeline());
        }

        private string CreateVideoPipeline()
        {
            var videoPipeline = String.Empty;

            for (var index = 0; index < _appSettings.WebCamSettings.Count; index++)
            {
                var webCamSetting = _appSettings.WebCamSettings[index];
                videoPipeline += GetFormattedVideoPipelineSource(webCamSetting, index) + Environment.NewLine;
            }

            videoPipeline += _appSettings.VideoSinkPipeline;
            return videoPipeline;
        }

        private string CreateAudioPipeline()
        {
            var audioPipeline = String.Empty;

            for (var index = 0; index < _appSettings.WebCamSettings.Count; index++)
            {
                var webCamSetting = _appSettings.WebCamSettings[index];
                audioPipeline += GetFormattedAudioPipelineSource(webCamSetting, index) + Environment.NewLine;
            }

            audioPipeline += _appSettings.AudioSinkPipeline;
            return audioPipeline;
        }

        private string GetFormattedVideoPipelineSource(WebCamSetting webCamSetting, int index)
        {
            var s = String.Format(_appSettings.VideoSourcePipeline, webCamSetting.VideoDeviceId, webCamSetting.Name, index);
            return s;
        }

        private string GetFormattedAudioPipelineSource(WebCamSetting webCamSetting, int index)
        {
            var s = String.Format(_appSettings.AudioSourcePipeline, webCamSetting.AudioDeviceId, index);
            return s;
        }

        public string ChangeWebCam(string webCamName)
        {
            var videoSelector = ((Bin)_videoPipeline).GetByName(SelectorName);
            var audioSelector = ((Bin)_audioPipeline).GetByName(SelectorName);

            var padName = String.Format(SinkSelector, ConvertWebCamNameToSinkIndex(webCamName));

            var newVideoPad = videoSelector.GetStaticPad(padName);
            var newAudioPad = audioSelector.GetStaticPad(padName);

            videoSelector["active-pad"] = newVideoPad;
            audioSelector["active-pad"] = newAudioPad;

            return webCamName;
        }

        private int ConvertWebCamNameToSinkIndex(string webCamName)
        {
            return _appSettings.WebCamSettings.FindIndex(setting => setting.Name == webCamName);
        }

        public bool StartStreaming()
        {
            _videoPipeline.ChangeState(StateChange.NullToReady);
            _videoPipeline.ChangeState(StateChange.ReadyToPaused);
            _videoPipeline.ChangeState(StateChange.PausedToPlaying);

            _audioPipeline.ChangeState(StateChange.PausedToPlaying);

            return true;
        }

        public bool StopStreaming()
        {
            _videoPipeline.ChangeState(StateChange.PlayingToPaused);
            _videoPipeline.ChangeState(StateChange.PausedToReady);
            _videoPipeline.ChangeState(StateChange.ReadyToNull);

            _audioPipeline.ChangeState(StateChange.PlayingToPaused);
            _audioPipeline.ChangeState(StateChange.PausedToReady);
            _audioPipeline.ChangeState(StateChange.ReadyToNull);

            return true;
        }
    }
}