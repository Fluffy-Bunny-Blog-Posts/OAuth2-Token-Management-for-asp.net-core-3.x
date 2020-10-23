using Microsoft.Extensions.DependencyInjection;
using PublicLibraryServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
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
