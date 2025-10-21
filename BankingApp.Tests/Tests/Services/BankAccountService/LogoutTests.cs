using System.Net;
using System.Net.Http.Json;
using BankingApp.Services;
using BankingAppProjectAvaloniaDesktop.Interfaces;
using BankingAppProjectAvaloniaDesktop.Models;
using BankingAppProjectAvaloniaDesktop.Services.Auth;
using Moq;
using Moq.Protected;
using Xunit;

namespace BankingApp.Tests.Services;


public class LogoutAsync
{
    [Fact]
    public async Task LogoutAsync_ShouldReturnSuccess_WhenServerRespondsSuccessfully()
    {
        // Arrange
        var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new ApiResponseModel<object?>
            {
                StatusMessage = "Success",
            })
        };

        HttpClient httpClient = new FakeHttpMessageHandler(fakeResponse).FakeHttpClient();

        var mockCredentialsStore = new Mock<ICredentialsStore>();
        mockCredentialsStore.Setup(x => x.DeleteAsync());
        var sessionManager = new SessionManager(httpClient, mockCredentialsStore.Object);

        // Act
        var result = await sessionManager.TerminateSession();

        // Assert
        Assert.Equal("Success", result.StatusMessage);
        mockCredentialsStore.Verify(x => x.DeleteAsync(), Times.Once);
    }

    [Fact]
    public async Task LogoutAsync_ShouldReturnFailed_WhenServerRespondsWithFailure()
    {
        // Arrange
        var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new ApiResponseModel<AuthModel>
            {
                StatusMessage = "Failed",
                Message = "Token Expired!\nPlease login again..."
            })
        };

        HttpClient httpClient = new FakeHttpMessageHandler(fakeResponse).FakeHttpClient();

        var mockCredentialsStore = new Mock<ICredentialsStore>();
        mockCredentialsStore.Setup(x => x.DeleteAsync());
        var sessionManager = new SessionManager(httpClient, mockCredentialsStore.Object);

        // Act
        var result = await sessionManager.TerminateSession();

        // Assert
        Assert.Equal("Failed", result.StatusMessage);
        Assert.Equal("Token Expired!\nPlease login again...", result.Message);
        mockCredentialsStore.Verify(x => x.DeleteAsync(), Times.Once);
    }

    [Fact]
    public async Task LogoutAsync_ShouldReturnError_WhenExceptionIsThrown()
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
        var mockCredentialsStore = new Mock<ICredentialsStore>();
        mockCredentialsStore.Setup(x => x.DeleteAsync());
        var sessionManager = new SessionManager(httpClient, mockCredentialsStore.Object);

        // Act
        var result = await sessionManager.TerminateSession();

        // Assert
        Assert.Equal("Error", result.StatusMessage);
        Assert.Contains("Server down", result.Message);
        mockCredentialsStore.Verify(x => x.DeleteAsync(), Times.Never);
    }

}