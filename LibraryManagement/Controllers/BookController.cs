using LibraryManagement.Models;
using LibraryManagement.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
namespace LibraryManagement.Controllers
{
    [Authorize]


    public class BookController : Controller
    {
        private readonly IBookRepository _bookRepository;

        public BookController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public IActionResult Index()
        {
            var books = _bookRepository.GetAllBooks();
            return View(books);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Book book, IFormFile PdfFile, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (PdfFile != null && PdfFile.Length > 0)
                    {
                        var pdfPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdfs", PdfFile.FileName);
                        using (var stream = new FileStream(pdfPath, FileMode.Create))
                        {
                            await PdfFile.CopyToAsync(stream);
                        }
                        book.PdfPath = "/pdfs/" + PdfFile.FileName;
                    }

                    if (Image != null && Image.Length > 0)
                    {
                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Pic", Image.FileName);
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await Image.CopyToAsync(stream);
                        }
                        book.ImagePath = "/Pic/" + Image.FileName;
                    }

                    _bookRepository.AddBook(book);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the book. Please try again.");
                }
            }

            return View(book);
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var book = _bookRepository.GetBookById(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Book book, IFormFile PdfFile)
        {
            if (ModelState.IsValid)
            {
                if (PdfFile != null && PdfFile.Length > 0)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdfs", PdfFile.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await PdfFile.CopyToAsync(stream);
                    }
                    book.PdfPath = "/pdfs/" + PdfFile.FileName;
                }

                _bookRepository.UpdateBook(book);
                return RedirectToAction("Index");
            }
            return View(book);
        }

        public IActionResult Details(int id)
        {
            var book = _bookRepository.GetBookById(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var book = _bookRepository.GetBookById(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _bookRepository.DeleteBook(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Rate(int bookId, int rating)
        {
            var book = _bookRepository.GetBookById(bookId);
            if (book != null)
            {
                book.Rating = rating;
                _bookRepository.UpdateBook(book);
            }
            return RedirectToAction("Index");
        }
    }
}
