
namespace BankingApp.Services;
public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpResponseMessage _fakeResponse;

    public FakeHttpMessageHandler(HttpResponseMessage response)
    {
        _fakeResponse = response;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        
        return Task.FromResult(_fakeResponse);
    }

    public HttpClient FakeHttpClient()
    {
        var handler = new FakeHttpMessageHandler(_fakeResponse);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5000/")
        };
        return httpClient;
    }

}