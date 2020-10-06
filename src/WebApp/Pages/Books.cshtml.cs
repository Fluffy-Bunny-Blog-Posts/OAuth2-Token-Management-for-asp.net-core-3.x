using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PublicLibraryServices;

namespace WebApp.Pages
{
    public class BooksModel : PageModel
    {
        private IBookService _bookService;

        public BooksModel(IBookService bookService)
        {
            _bookService = bookService;
        }

        public IEnumerable<Book> Books { get; private set; }

        public async Task OnGetAsync()
        {
            Books = await _bookService.GetBooksAsync();
        }
        public async Task<ActionResult> OnGetDeleteAsync(string id)
        {
            if (id != null)
            {
                await _bookService.DeleteBookAsync(id);
            }
            Books = await _bookService.GetBooksAsync();
            return Page();
        }
    }
}
