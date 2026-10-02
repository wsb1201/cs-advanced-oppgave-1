namespace LibrarySystem.Domain.Repositories;

public class MemLibraryRepository : ILibraryRepository
{
	private readonly Dictionary<Guid, Book> _bookRegistry = [];
	private readonly Dictionary<Guid, User> _userRegistry = [];
	private readonly Dictionary<Guid, Loan> _loanRegistry = [];

	public Task<Guid> RegisterBook(string title, string author)
	{
		var book = new Book(title, author);
		_bookRegistry.Add(book.Id, book);
		return Task.FromResult(book.Id);
	}

	public Task<Book?> LookupBook(Guid bookId) =>
		Task.FromResult(_bookRegistry.TryGetValue(bookId, out var book) ? book : null);

	public async Task<Guid?> RegisterLoan(Guid bookId, Guid userId, DateTimeOffset expiryDate)
	{
		var book = await LookupBook(bookId);
		if (book is null)
			return null;

		book.Borrow();

		var loan = new Loan(bookId, userId, expiryDate);
		_loanRegistry.Add(loan.Id, loan);
		return loan.Id;
	}

	public async Task<Loan?> DeleteLoan(Guid loanId)
	{
		if (!_loanRegistry.TryGetValue(loanId, out var loan))
			return null;

		var book = await LookupBook(loan.BookId);
		book?.MakeAvailable();

		return _loanRegistry.Remove(loan.Id) ? loan : null;
	}

	public Task<IEnumerable<Loan>> GetLoans(User user) => throw new NotImplementedException();

	public Task<bool> AddUser(string name, string hash, bool librarian) =>
		throw new NotImplementedException();

	public Task<User?> GetUserByName(string name) => throw new NotImplementedException();

	public Task<User?> GetUserById(Guid userId) => throw new NotImplementedException();
}
