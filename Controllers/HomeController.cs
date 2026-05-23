using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;
using WriterApp.Data;
using WriterApp.Models;
using static System.Reflection.Metadata.BlobBuilder;

namespace WriterApp.Controllers
{
    public class HomeController : Controller
    {
        public Guid MainId;
        ICollectionsRepository collectionsRepository;
        IBookRepository bookRepository;
        List<Collection> collections;
        List<Book> books;

        public HomeController(ICollectionsRepository collectRep, IBookRepository bookRep)
        {
            this.collectionsRepository = collectRep;
            this.bookRepository = bookRep;
            collections = collectionsRepository.GetAll();
            books = bookRepository.GetAll();
        }

        public IActionResult Index()
        {
            ViewBag.Books = books;
            return View(collections);
        }


        public IActionResult AddNew(string Name, string Author, string Genre, bool IsDone, string? Description, string? PathImage)
        {
            Book book = new Book(Name,Author, Genre, IsDone, Description);
            if(!string.IsNullOrEmpty(PathImage))  book.PathImage= PathImage;
            
            books.Add(book);
            if (IsDone)
            {
                collections[1].Books.Add(book.Id, book.Name);
                collections[1].Amount++;
            }
            else
            {
                collections[0].Books.Add(book.Id, book.Name);
                collections[0].Amount++;
            }

            var options = new JsonSerializerOptions { WriteIndented = true };

            string newbook = JsonSerializer.Serialize(books, options);
            string newcoll = JsonSerializer.Serialize(collections, options);
            System.IO.File.WriteAllText("Data/books.json", newbook);
            System.IO.File.WriteAllText("Data/collections.json", newcoll);

            return RedirectToAction("Index");
        }

    }
}
