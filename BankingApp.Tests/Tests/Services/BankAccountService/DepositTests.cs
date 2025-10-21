
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

public class DepositAsync
{
    [Fact]
    public async Task DepositAsync_ShouldReturnSuccess_WhenServerRespondsSuccessfully()
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

        var mockSessionManager = new Mock<ISessionManager>();
        var bankService = new BankAccountService(httpClient, mockSessionManager.Object);

        // Act
        var result = await bankService.DepositAsync(123.12m);

        // Assert
        Assert.Equal("Success", result.StatusMessage);
    }

    [Fact]
    public async Task DepositAsync_ShouldReturnError_WhenExceptionIsThrown()
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
        var bankService = new BankAccountService(httpClient, mockSessionManager.Object);

        // Act
        var result = await bankService.DepositAsync(123.12m);

        // Assert
        Assert.Equal("Error", result.StatusMessage);
        Assert.Contains("Server down", result.Message);
    }
    
}