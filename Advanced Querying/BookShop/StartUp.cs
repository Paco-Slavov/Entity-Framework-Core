namespace BookShop;

using BookShop.Models.Enums;
using Data;
using Initializer;
using Microsoft.EntityFrameworkCore;
using System.Text;

public class StartUp
{
    public static void Main()
    {
        using var db = new BookShopContext();
        //DbInitializer.ResetDatabase(db);


        string result = string.Empty;

        //02. Age Restriction

        //string command = Console.ReadLine().ToLower();
        //result = GetBooksByAgeRestriction(db, command);

        //03. Golden Books
        //result = GetGoldenBooks(db);

        //04. Books by Price
        //result = GetBooksByPrice(db);

        //05. Not Released In
        //int year = int.Parse(Console.ReadLine());
        //result = GetBooksNotReleasedIn(db, year);

        //06. Book Titles By Category
        //string categories = Console.ReadLine();
        //result = GetBooksByCategory(db, categories);

        //07. Released Before Date
        //string date = Console.ReadLine();
        //result = GetBooksReleasedBefore(db, date);

        //08. Author Search
        //string endingCharacters = Console.ReadLine();
        //result = GetAuthorNamesEndingIn(db, endingCharacters);

        //09. Book Search
        //string stringSearched = Console.ReadLine();
        //result = GetBookTitlesContaining(db, stringSearched);

        //10. Book Search By Author
        //string stringSearched = Console.ReadLine();
        //result = GetBooksByAuthor(db, stringSearched);

        //11. Count Books
        //int length = int.Parse(Console.ReadLine());
        //int countBooks = CountBooks(db, length);
        //Console.WriteLine(countBooks);

        //12. Total Book Copies
        //result = CountCopiesByAuthor(db);

        //13. Profit By Category
        //result = GetTotalProfitByCategory(db);

        //14. Most Recent Books
        //result = GetMostRecentBooks(db);

        //15. Increase Prices
        //IncreasePrices(db);

        //16. Remove Books
        //int removedBooksCount = RemoveBooks(db);
        //Console.WriteLine(removedBooksCount);

        //Console.WriteLine(result);
    }

    public static string GetBooksByAgeRestriction(BookShopContext context, string command)
    {
        var enumValue = Enum.Parse<AgeRestriction>(command, true);

        var books = context.Books
            .Where(b => b.AgeRestriction == enumValue)
            .Select(b => b.Title)
            .OrderBy(t => t)
            .ToArray();

        return String.Join(Environment.NewLine, books);
    }

    public static string GetGoldenBooks(BookShopContext context)
    {
        var editionType = Enum.Parse<EditionType>("Gold", false);

        var goldenBooks = context.Books
        .Where(gb => gb.EditionType == editionType && gb.Copies < 5000)
        .Select(gb => new { gb.BookId, gb.Title })
        .OrderBy(gb => gb.BookId)
        .ToArray();

        return string.Join(Environment.NewLine, goldenBooks.Select(gb => gb.Title));
    }

    public static string GetBooksByPrice(BookShopContext context)
    {
        var books = context.Books
            .Where(b => b.Price > 40)
            .Select(b => new { b.Title, b.Price })
            .OrderByDescending(b => b.Price)
            .ToArray();

        StringBuilder sb = new StringBuilder();

        foreach (var book in books)
        {
            sb.AppendLine($"{book.Title} - ${book.Price:f2}");
        }
        return sb.ToString().TrimEnd();
    }

    public static string GetBooksNotReleasedIn(BookShopContext context, int year)
    {
        var books = context.Books
            .Where(b => b.ReleaseDate.Value.Year != year)
            .Select(b => new { b.BookId, b.Title })
            .OrderBy(b => b.BookId)
            .ToArray();

        return String.Join(Environment.NewLine, books.Select(b => b.Title));
    }

    public static string GetBooksByCategory(BookShopContext context, string input)
    {
        string[] categories = input.ToLower().Split().ToArray();

        var books = context.BooksCategories
            .Where(bc => categories.Contains(bc.Category.Name.ToLower()))
            .Select(bc => bc.Book.Title)
            .OrderBy(t => t)
            .ToArray();

        return String.Join(Environment.NewLine, books);
    }

    public static string GetBooksReleasedBefore(BookShopContext context, string date)
    {
        DateTime givenDate = DateTime.ParseExact(date, "dd-MM-yyyy", null);

        var books = context.Books
                .Where(b => b.ReleaseDate < givenDate)
                .Select(b => new { b.Title, b.EditionType, b.Price, b.ReleaseDate })
                .OrderByDescending(b => b.ReleaseDate);

        StringBuilder sb = new StringBuilder();

        foreach (var book in books)
        {
            sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:f2}");
        }
        return sb.ToString().TrimEnd();
    }

    public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
    {
        var authors = context.Authors
            .Where(a => a.FirstName.EndsWith(input))
            .Select(a => new { FullName = a.FirstName + " " + a.LastName })
            .OrderBy(a => a.FullName)
            .ToArray();

        return string.Join(Environment.NewLine, authors.Select(a => a.FullName));
    }

    public static string GetBookTitlesContaining(BookShopContext context, string input)
    {
        var books = context.Books
            .Where(b => b.Title.ToLower().Contains(input.ToLower()))
            .Select(b => b.Title)
            .OrderBy(t => t)
            .ToArray();

        return string.Join(Environment.NewLine, books);
    }

    public static string GetBooksByAuthor(BookShopContext context, string input)
    {
        var books = context.Books
            .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
            .Select(b => new
            {
                b.Title,
                AuthorName = b.Author.FirstName + " " + b.Author.LastName,
                b.BookId
            })
            .OrderBy(b => b.BookId)
            .ToArray();

        StringBuilder sb = new StringBuilder();

        foreach (var book in books)
        {
            sb.AppendLine($"{book.Title} ({book.AuthorName})");
        }

        return sb.ToString().TrimEnd();
    }

    public static int CountBooks(BookShopContext context, int lengthCheck)
    {
        int booksCount = context.Books
            .Where(b => b.Title.Length > lengthCheck)
            .Count();

        return booksCount;
    }

    public static string CountCopiesByAuthor(BookShopContext context)
    {
        var authorsInfo = context.Authors
            .AsNoTracking()
            .Select(a => new
            {
                FullName = a.FirstName + " " + a.LastName,
                BooksCopiesCount = a.Books.Sum(b => b.Copies)
            })
            .OrderByDescending(a => a.BooksCopiesCount)
            .ToList();

        StringBuilder sb = new StringBuilder();

        foreach (var author in authorsInfo)
        {
            sb.AppendLine($"{author.FullName} - {author.BooksCopiesCount}");
        }

        return sb.ToString().TrimEnd();
    }

    public static string GetTotalProfitByCategory(BookShopContext context)
    {
        var categoriesInfo = context.Categories
            .AsNoTracking()
            .Select(c => new
            {
                CategoryName = c.Name,
                Profit = c.CategoryBooks.Sum(bc => bc.Book.Copies * bc.Book.Price)
            })
            .OrderByDescending(c => c.Profit)
            .ThenBy(c => c.CategoryName);

        StringBuilder sb = new StringBuilder();

        foreach (var category in categoriesInfo)
        {
            sb.AppendLine($"{category.CategoryName} ${category.Profit:f2}");
        }

        return sb.ToString().TrimEnd();
    }

    public static string GetMostRecentBooks(BookShopContext context)
    {
        var categoriesInfo = context.Categories
            .AsNoTracking()
            .Select(c => new
            {
                CategoryName = c.Name,
                Books = c.CategoryBooks.Select(cb => new
                {
                    BookName = cb.Book.Title,
                    ReleaseDate = cb.Book.ReleaseDate
                })
                .OrderByDescending(b => b.ReleaseDate)
                .Take(3)
                .ToArray()
            })
            .OrderBy(c => c.CategoryName)
            .ToArray();

        StringBuilder sb = new StringBuilder();
        
        foreach (var category in categoriesInfo)
        {
            sb.AppendLine($"--{category.CategoryName}");

            foreach (var book in category.Books)
            {
                sb.AppendLine($"{book.BookName} ({book.ReleaseDate.Value.Year})");
            }
        }

        return sb.ToString().TrimEnd();
    }

    public static void IncreasePrices(BookShopContext context)
    {
        var books = context.Books
            .Where(b => b.ReleaseDate.Value.Year < 2010)
            .ToArray();

        foreach (var book in books)
        {
            book.Price += 5;
        }

        context.SaveChanges();
    }

    public static int RemoveBooks(BookShopContext context)
    {
        var booksCategoriesToRemove = context.BooksCategories
            .Where(bc => bc.Book.Copies < 4200)
            .ToArray();

        var booksToRemove = context.Books
            .Where(b => b.Copies < 4200)
            .ToArray();

        int removedBooks = booksToRemove.Count();

        context.BooksCategories.RemoveRange(booksCategoriesToRemove);
        context.Books.RemoveRange(booksToRemove);
        context.SaveChanges();

        return removedBooks;
    }
}