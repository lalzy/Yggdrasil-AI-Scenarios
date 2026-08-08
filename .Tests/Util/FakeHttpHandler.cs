// FakeHttpHandler.cs

using System.Text;

namespace Yggdrasil.Tests.Util;

public class FakeHttpHandler : HttpMessageHandler
{
    public HttpRequestMessage? PreviousRequest { get; private set; }
    public string Response { get; set; } = "";

    ///<summary>Mocks an HTTP Request and returns a fake JSON response</summary>
    ///<param name="request">The HTTP Request object</param>
    ///<param name="cancellationToken">Unused</param>
    ///<returns>The (mocked) response.</returns>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        PreviousRequest = request;
        var response = new HttpResponseMessage(HttpStatusCode.OK);
        response.Content = new StringContent(Response, Encoding.UTF8, "application/json");
        return Task.FromResult(response);
        }
}
