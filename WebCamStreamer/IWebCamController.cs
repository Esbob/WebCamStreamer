using System.ServiceModel;
using System.ServiceModel.Web;

namespace WebCamStreamer
{
    [ServiceContract]
    public interface IWebCamController
    {
        [OperationContract]
        [WebInvoke(
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            ResponseFormat = WebMessageFormat.Json)]
        bool StartStreaming();

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            UriTemplate = "ChangeWebCam/{webCamName}",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            ResponseFormat = WebMessageFormat.Json)]
        string ChangeWebCam(string webCamName);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            ResponseFormat = WebMessageFormat.Json)]
        bool StopStreaming();
    }
}
