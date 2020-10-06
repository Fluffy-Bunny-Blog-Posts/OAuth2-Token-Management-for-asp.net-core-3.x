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
    public class NewBookModel : PageModel
    {
        private IBookService _bookService;

        public NewBookModel(IBookService bookService)
        {
            _bookService = bookService;
        }
        public async Task OnGetAsync()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            await _bookService.UpsertBookAsync(new Book()
            {
                Author = Input.Author,
                Title = Input.Title
            });
            return RedirectToPage("./Books");
 
        }
        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            [Required]
            public string Author { get; set; }

            [Required]
            public string Title { get; set; }
        }
    }
}
