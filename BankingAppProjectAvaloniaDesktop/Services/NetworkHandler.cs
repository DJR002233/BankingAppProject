using System;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace BankingAppProjectAvaloniaDesktop.Services
{
    public class NetworkErrorHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                return await base.SendAsync(request, cancellationToken);
            }
            catch (HttpRequestException ex) when (ex.InnerException is SocketException socketEx)
            {
                if (socketEx.SocketErrorCode == SocketError.ConnectionRefused)
                {
                    Console.WriteLine("❌ Server is down or refusing connections.");
                    // You could also trigger a UI notification or custom response
                    return new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable)
                    {
                        ReasonPhrase = "Server unavailable"
                    };
                }

                if (socketEx.SocketErrorCode == SocketError.TimedOut)
                {
                    Console.WriteLine("⚠️ Request timed out.");
                    return new HttpResponseMessage(System.Net.HttpStatusCode.RequestTimeout)
                    {
                        ReasonPhrase = "Request timed out"
                    };
                }

                throw; // Re-throw if not a handled case
            }
            catch (TaskCanceledException)
            {
                // This usually means a timeout
                Console.WriteLine("⏱️ Request canceled or timed out.");
                return new HttpResponseMessage(System.Net.HttpStatusCode.RequestTimeout)
                {
                    ReasonPhrase = "Request canceled"
                };
            }
        }
    }
}
