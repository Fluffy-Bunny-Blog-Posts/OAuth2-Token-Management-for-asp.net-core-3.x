using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using PublicLibraryServices;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TestServices;
using WebApp.Pages;
using Xunit;

namespace XUnitTest_WebApp
{
    // https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-3.1
    // https://wellsb.com/csharp/aspnet/xunit-unit-test-razor-pages/

    public class UnitTest_BasicAuthRequest :
        IClassFixture<CustomWebApplicationFactory<WebApp.Startup>>
    {
        private readonly CustomWebApplicationFactory<WebApp.Startup> _factory;
        private readonly System.Net.Http.HttpClient _client;
        
        public UnitTest_BasicAuthRequest(CustomWebApplicationFactory<WebApp.Startup> factory)
        {
            _factory = factory;
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                });
            }).CreateClient();
        }
        [Theory]
        [InlineData("/")]
        [InlineData("/Index")]
        [InlineData("/Privacy")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
        [Fact]
        public async Task Get_SecurePageRedirectsAnUnauthenticatedUser()
        {
            // Arrange
            var client = _factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });

            // Act
            var response = await client.GetAsync("/RequiresAuth");

            // Assert
            response.StatusCode
                .Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location.OriginalString
                .Should().StartWith("http://localhost/Identity/Account/Login");
        }
        [Fact]
        public async Task Get_SecurePageIsReturnedForAnAuthenticatedUser()
        {
            //Act
            var response = await _client.GetAsync("/RequiresAuth");

            // Assert
            response.StatusCode
              .Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task Get_TestServicesAsync()
        {
            var serviceProvider = _factory.Server.Services;
            using (var scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<IScopedService>();

                var scopedService = _factory.Server.Services.GetRequiredService<IBookService>();
                scopedService.Should().NotBeNull();

            }
            var singletonService = serviceProvider.GetRequiredService<ISingletonService>();
            singletonService.Should().NotBeNull();
            var transientService = serviceProvider.GetRequiredService<ITransientService>();
            transientService.Should().NotBeNull();

        }
    }
}
