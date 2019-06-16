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
            string baseaddr = "http://localhost:9999/WebCamController/";
            Uri baseAddress = new Uri(baseaddr);

            // Create the ServiceHost.
            using (var host = new WebServiceHost(typeof(WebCamController), baseAddress))
            {
                // Enable metadata publishing.
                var smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                //smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                host.Description.Behaviors.Add(smb);

                host.AddServiceEndpoint(typeof(IWebCamController), new WebHttpBinding(), baseaddr);
                host.AddServiceEndpoint(typeof(IWebCamController), new WebHttpBinding(), baseaddr + "StartStreaming");
                host.AddServiceEndpoint(typeof(IWebCamController), new WebHttpBinding(), baseaddr + "ChangeWebCam");
                host.AddServiceEndpoint(typeof(IWebCamController), new WebHttpBinding(), baseaddr + "StopStreaming");

                //for some reason a default endpoint does not get created here
                host.Open();

                Console.WriteLine("The service is ready at {0}", baseAddress);
                Console.WriteLine("Press <Enter> to stop the service.");
                Console.ReadLine();

                // Close the ServiceHost.
                host.Close();
            }
        }
    }
}
