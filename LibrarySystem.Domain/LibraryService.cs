namespace LibrarySystem.Domain;

public class LibraryService
{
	private readonly Dictionary<Guid, Book> _bookRegistry = [];
	private readonly Dictionary<Guid, Loan> _loanRegistry = [];

	public Guid RegisterBook(string title, string author)
	{
		var book = new Book(title, author);
		_bookRegistry.Add(book.Id, book);
		return book.Id;
	}

	public Book? LookupBook(Guid bookId)
	{
		return _bookRegistry.TryGetValue(bookId, out var book) ? book : null;
	}

	public Guid? RegisterLoan(Guid bookId, string patron, DateTimeOffset expiryDate)
	{
		var book = LookupBook(bookId);
		book?.Borrow();
		if (book is null)
			return null;

		var loan = new Loan(bookId, patron, expiryDate);
		_loanRegistry.Add(loan.Id, loan);
		return loan.Id;
	}

	public Loan? DeleteLoan(Guid loanId)
	{
		var loan = _loanRegistry[loanId];
		var book = LookupBook(loan.BookId);
		book?.MakeAvailable();

		return _loanRegistry.Remove(loan.Id) ? loan : null;
	}
}
