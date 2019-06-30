using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;

namespace WebCamStreamer
{
    public class WebServiceHostRunner
    {
        public WebServiceHostRunner()
        {
            var baseAddressString = AppSettingsReader.GetAppSettings().ServiceHostBaseAddress;
            var baseAddressUri = new Uri(baseAddressString);

            using (var host = new WebServiceHost(typeof(WebCamController), baseAddressUri))
            {
                // Enable metadata publishing.
                var smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                //smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                host.Description.Behaviors.Add(smb);

                host.AddServiceEndpoint(typeof(IWebCamController), new WebHttpBinding(), baseAddressString);
                host.AddServiceEndpoint(typeof(IWebCamController), new WebHttpBinding(), baseAddressString + "StartStreaming");
                host.AddServiceEndpoint(typeof(IWebCamController), new WebHttpBinding(), baseAddressString + "ChangeWebCam");
                host.AddServiceEndpoint(typeof(IWebCamController), new WebHttpBinding(), baseAddressString + "StopStreaming");
                
                host.Open();

                Console.WriteLine("The service is ready at {0}", baseAddressUri);
                Console.WriteLine("Press <Enter> to stop the service.");
                Console.ReadLine();

                host.Close();
            }
        }
    }
}