using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PublicLibraryServices
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetBooksAsync();
        Task<Book> GetBookAsync(string isbn);
        Task UpsertBookAsync(Book book);
        Task DeleteBookAsync(string isbn);
    }
}
