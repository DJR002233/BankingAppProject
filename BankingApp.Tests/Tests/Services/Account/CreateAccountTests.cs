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

public class CreateAccountTests
{
    [Fact]
    public async Task CreateAccountAsync_ShouldReturnSuccess_WhenServerRespondsSuccessfully()
    {
        // Arrange
        var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new ApiResponseModel<AuthModel>
            {
                StatusMessage = "Success",
                Message = "Account Created Successfully!"
            })
        };

        HttpClient httpClient = new FakeHttpMessageHandler(fakeResponse).FakeHttpClient();

        var mockSessionManager = new Mock<ISessionManager>();

        var authService = new AuthService(httpClient, mockSessionManager.Object);

        // Act
        var result = await authService.CreateAccountAsync("name", "email@email.com", "password");

        // Assert
        Assert.Equal("Success", result.StatusMessage);
        Assert.Equal("Account Created Successfully!", result.Message);
    }
    
    [Fact]
    public async Task CreateAccountAsync_ShouldReturnFailed_WhenServerRespondsWithFailure()
    {
        // Arrange
        var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new ApiResponseModel<AuthModel>
            {
                StatusMessage = "Failed",
                Message = "Email already taken!"
            })
        };

        HttpClient httpClient = new FakeHttpMessageHandler(fakeResponse).FakeHttpClient();

        var mockSessionManager = new Mock<ISessionManager>();
        var authService = new AuthService(httpClient, mockSessionManager.Object);

        // Act
        var result = await authService.CreateAccountAsync("name", "email@email.com", "password");

        // Assert
        Assert.Equal("Failed", result.StatusMessage);
        Assert.Equal("Email already taken!", result.Message);
    }
    
    [Fact]
    public async Task CreateAccountAsync_ShouldReturnError_WhenExceptionIsThrown()
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
        var result = await authService.CreateAccountAsync("name", "email@email.com", "password");

        // Assert
        Assert.Equal("Error", result.StatusMessage);
        Assert.Contains("Server down", result.Message);
    }
    /**/
}