using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using BankingAppProjectAvaloniaDesktop.Models;
using BankingAppProjectAvaloniaDesktop.Services;
using BankingAppProjectAvaloniaDesktop.Services.Auth;
using Moq;
using Moq.Protected;
using Xunit;

namespace BankingApp.Tests;

public class AuthServiceTests
{
    private class FakeHttpMessageHandler : HttpMessageHandler
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
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnSuccess_WhenServerRespondsSuccessfully()
    {
        // Arrange
        var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new ApiResponseModel<AuthModel>
            {
                StatusMessage = "Success",
                Data = new AuthModel
                {
                    AccessToken = "fake_token",
                    RefreshToken = "fake_refresh",
                }
            })
        };

        var handler = new FakeHttpMessageHandler(fakeResponse);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new System.Uri("http://localhost:5000/")
        };

        var mockSessionManager = new Mock<SessionManager>(null!, null!, null!);
        mockSessionManager.Setup(x => x.SetSession(It.IsAny<AuthModel>()));

        var authService = new AuthService(httpClient, mockSessionManager.Object);

        // Act
        var result = await authService.LoginAsync("test@test.com", "password123");

        // Assert
        Assert.Equal("Success", result.StatusMessage);
        mockSessionManager.Verify(x => x.SetSession(It.IsAny<AuthModel>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailed_WhenServerRespondsWithFailure()
    {
        // Arrange
        var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new ApiResponseModel<AuthModel>
            {
                StatusMessage = "Failed",
                Message = "Invalid credentials"
            })
        };

        var handler = new FakeHttpMessageHandler(fakeResponse);
        var httpClient = new HttpClient(handler);
        var mockSessionManager = new Mock<SessionManager>(null!, null!, null!);

        var authService = new AuthService(httpClient, mockSessionManager.Object);

        // Act
        var result = await authService.LoginAsync("wrong@test.com", "badpassword");

        // Assert
        Assert.Equal("Failed", result.StatusMessage);
        Assert.Equal("Invalid credentials", result.Message);
        mockSessionManager.Verify(x => x.SetSession(It.IsAny<AuthModel>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnError_WhenExceptionIsThrown()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected().Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
            .Throws(new HttpRequestException("Server down")
        );

        var httpClient = new HttpClient(handler.Object);
        var mockSessionManager = new Mock<SessionManager>(null!, null!, null!);
        var authService = new AuthService(httpClient, mockSessionManager.Object);

        // Act
        var result = await authService.LoginAsync("any", "any");

        // Assert
        Assert.Equal("Error", result.StatusMessage);
        Assert.Contains("Server down", result.Message);
    }
}
