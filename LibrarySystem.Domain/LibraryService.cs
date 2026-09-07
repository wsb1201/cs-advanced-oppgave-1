namespace LibrarySystem.Domain;

public class LibraryService
{
	private readonly Dictionary<Guid, Book> _bookRegistry = [];

	public Guid RegisterBook(string title, string author)
	{
		var book = new Book(title, author);
		_bookRegistry.Add(book.Id, book);
		return book.Id;
	}

	public Book Lookup(Guid id)
	{
		return _bookRegistry[id];
	}
}
