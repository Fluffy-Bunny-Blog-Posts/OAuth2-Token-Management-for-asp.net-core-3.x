using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace PublicLibraryServices.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddBookService(this IServiceCollection services)
        {
            services.AddSingleton<IBookService, BookService>();
            return services;
        }
    }
}
