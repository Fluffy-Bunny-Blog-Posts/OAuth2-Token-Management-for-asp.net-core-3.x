using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace PublicLibraryServices
{
    public class BookService : IBookService
    {
        private string GuidS => Guid.NewGuid().ToString();
        private ILogger<BookService> _logger;
        private List<Book> _books;
        public BookService(ILogger<BookService> logger)
        {
            _logger = logger;
        }
        public async Task<Book> GetBookAsync(string isdn)
        {
            var books = await GetBooksAsync();

            var query = from item in books
                        where item.ISBN == isdn
                        select item;
            return query.FirstOrDefault();
        }

        public async Task<IEnumerable<Book>> GetBooksAsync()
        {
            if (_books == null)
            {
                _books = new List<Book>
                {
                    new Book()
                    {
                        ISBN = GuidS,
                        Author = "Bob Johnson",
                        Title = "Drumming Made Easy"
                    },
                    new Book()
                    {
                        ISBN = GuidS,
                        Author = "Todd Gorder",
                        Title = "The Bass Player Just Needs To Look Good"
                    }
                };
            }
            return _books;
        }

        public async Task UpsertBookAsync(Book book)
        {
            var books = await GetBooksAsync();
            var ori = from item in books
                      where item.ISBN == book.ISBN
                      select item;
            var bb = ori.FirstOrDefault();
            if(bb != null)
            {
                bb.Author = book.Author;
                bb.Title = book.Title;
            }
            else
            {
                _books.Add(new Book()
                {
                    ISBN = GuidS,
                    Author = book.Author,
                    Title = book.Title
                });
            }
        }

        public async Task DeleteBookAsync(string isbn)
        {
            var books = await GetBooksAsync();
            var newBooks = from item in books
                           where item.ISBN != isbn
                           select item;
            _books = newBooks.ToList();
        }
    }

}
