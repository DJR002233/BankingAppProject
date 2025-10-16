using System.Net;
using System.Net.Http.Json;
using BankingApp.Services;
using BankingAppProjectAvaloniaDesktop.Models;
using BankingAppProjectAvaloniaDesktop.Services;
using BankingAppProjectAvaloniaDesktop.Services.Auth;
using Moq;
using Moq.Protected;
using Xunit;

namespace BankingApp.Tests.Services;

public class LoginTests
{
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
                    Expiration = DateTime.UtcNow.AddDays(7),
                    RefreshToken = "fake_refresh",
                }
            })
        };

        HttpClient httpClient = new FakeHttpMessageHandler(fakeResponse).FakeHttpClient();

        var mockSessionManager = new Mock<ISessionManager>();
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

        HttpClient httpClient = new FakeHttpMessageHandler(fakeResponse).FakeHttpClient();

        var mockSessionManager = new Mock<ISessionManager>();
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

        var httpClient = new HttpClient(handler.Object)
        {
            BaseAddress = new Uri("http://localhost:5000/")
        };
        var mockSessionManager = new Mock<ISessionManager>();
        var authService = new AuthService(httpClient, mockSessionManager.Object);

        // Act
        var result = await authService.LoginAsync("any", "any");

        // Assert
        Assert.Equal("Error", result.StatusMessage);
        Assert.Contains("Server down", result.Message);
    }
    /**/
}