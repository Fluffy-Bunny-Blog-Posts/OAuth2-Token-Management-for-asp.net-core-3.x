using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PublicLibraryServices;

namespace WebApp.Pages
{
    public class EditBookModel : PageModel
    {
        private IBookService _bookService;

        public EditBookModel(IBookService bookService)
        {
            _bookService = bookService;
        }
        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            [Required]
            public string ISBN { get; set; }

            [Required]
            public string Author { get; set; }

            [Required]
            public string Title { get; set; }
        }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            var book = await _bookService.GetBookAsync(id);
            if(book == null)
            {
                return RedirectToPage("./Books");
            }
            Input = new InputModel
            {
                ISBN = book.ISBN,
                Author = book.Author,
                Title = book.Title
            };
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            await _bookService.UpsertBookAsync(new Book()
            {
                ISBN = Input.ISBN,
                Author = Input.Author,
                Title = Input.Title
            });
            return RedirectToPage("./Books");

        }
    }
}
