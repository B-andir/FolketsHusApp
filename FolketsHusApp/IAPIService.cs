using System.Security.Cryptography.X509Certificates;

namespace FolketsHusApp;

public class Response {
    public Response() {
        response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
        responseString = string.Empty;
    }

    public Response(HttpResponseMessage response, string responseString) {
        this.response = response;
        StatusCode = response.StatusCode;
        this.responseString = responseString;
    }

    public System.Net.HttpStatusCode StatusCode { get; private set; }
    public HttpResponseMessage response { get; private set; }
    public string responseString { get; private set; }
}

public interface IAPIService {

    Response post(string route, object body, bool shouldRedirect = true);
    Response postProtected(string route, object body);

}
