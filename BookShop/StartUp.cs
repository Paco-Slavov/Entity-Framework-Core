namespace BookShop;

using BookShop.Models.Enums;
using Data;
using Initializer;
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

        Console.WriteLine(result);
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

        return string.Join(Environment.NewLine,authors);
    }
}