using System;
using System.IO;
// using GLib;
using Gst;
using Microsoft.Extensions.Configuration;

namespace WebCamStreamer
{
    public class WebCamController : IWebCamController
    {
        Element _sink = null;
        string _currentWebcamName = "WC1";


        public WebCamController()
        {
            var appSettings = GetAppSettings();

            Gst.Application.Init();
            return;

            _sink = Parse.Launch(
            "uvch264src device=/dev/video0 initial-bitrate=3000000 average-bitrate=3000000 iframe-period=1000 name=uvcsrc1 auto-start=true uvcsrc1.vidsrc ! video/x-h264, framerate=30/1, width=1280, height=720 ! queue ! s.sink_0" + System.Environment.NewLine +
            "uvch264src device=/dev/video1 initial-bitrate=3000000 average-bitrate=3000000 iframe-period=1000 name=uvcsrc2 auto-start=true uvcsrc2.vidsrc ! video/x-h264, framerate=30/1, width=1280, height=720 ! queue ! s.sink_1" + System.Environment.NewLine +
            "videotestsrc pattern=black is-live=true ! x264enc pass=5 quantizer=25 speed-preset=6 ! video/x-h264, framerate=30/1, width=1920, height=1080 ! queue ! s.sink_2" + System.Environment.NewLine +
            "input-selector name=s sync-mode=1 ! rtph264pay config-interval=1 pt=96 ! udpsink host=127.0.0.1 port=8004");
        }

        private string GetFormattedPipelineSource(AppSettings appSettings, WebCamSetting webCamSetting)
        {
            var s = String.Format(appSettings.SourcePipeline, webCamSetting.DeviceId, webCamSetting.Name);
            return s;
        }

        private string GetFormattedPipelineSink(AppSettings appSettings, WebCamSetting webCamSetting)
        {
            var s = String.Format(appSettings.SinkPipeline, webCamSetting.Name);
            return s;
        }

        public string ChangeWebCam(string webCamName)
        {
            var @switch = ((Bin)_sink).GetByName("s");
            var newpad = @switch.GetStaticPad("sink_" + webCamName);
            @switch["active-pad"] = newpad;
            return webCamName;
        }

        private static Element GetBaseElement(Element element, string elementName)
        {
            return ((Bin)element).GetByName(elementName);
        }

        public bool StartStreaming()
        {
            _sink.ChangeState(StateChange.NullToReady);
            _sink.ChangeState(StateChange.ReadyToPaused);
            _sink.ChangeState(StateChange.PausedToPlaying);

            return true;
        }

        public bool StopStreaming()
        {
            _sink.ChangeState(StateChange.PlayingToPaused);
            _sink.ChangeState(StateChange.PausedToReady);
            _sink.ChangeState(StateChange.ReadyToNull);

            return true;
        }

        private static AppSettings GetAppSettings()
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