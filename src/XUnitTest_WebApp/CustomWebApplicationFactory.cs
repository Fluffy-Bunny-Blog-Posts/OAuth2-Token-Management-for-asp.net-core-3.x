using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PublicLibraryServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XUnitTest_WebApp
{
    public class CustomWebApplicationFactory<TStartup>: 
        WebApplicationFactory<TStartup> 
        where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var bookService = A.Fake<IBookService>();
                A.CallTo(() => bookService.GetBookAsync("1234"))
                        .Returns(Task.FromResult(
                            new Book
                            {
                                ISBN = "123",
                                Author = "Author McAuthorFace",
                                Title = "Down with the ship"
                            }

                            ));
                var books = new List<Book>()
                    {
                        new Book
                        {
                            ISBN = "123",
                            Author = "Author McAuthorFace",
                            Title = "Down with the ship"
                        }
                    };
                A.CallTo(() => bookService.GetBooksAsync())
                    .Returns(Task.FromResult(books.AsEnumerable()));
                services.AddSingleton<IBookService>(bookService);


                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    
                }
            });
        }
    }
}
