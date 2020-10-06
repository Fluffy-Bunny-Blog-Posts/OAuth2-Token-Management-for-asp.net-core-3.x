using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PublicLibraryServices;

namespace WebApp.Pages
{
    public class DisplayBookModel : PageModel
    {
        private IBookService _bookService;

        public DisplayBookModel(IBookService bookService)
        {
            _bookService = bookService;
        }

        public Book Book { get; private set; }

        public async Task OnGetAsync(string id)
        {
            Book = await _bookService.GetBookAsync(id);
        }
    }
}
