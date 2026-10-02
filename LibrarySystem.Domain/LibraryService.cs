namespace LibrarySystem.Domain;

using Repositories;

public class LibraryService(ILibraryRepository repo)
{
	public Task<Guid> RegisterBook(string title, string author, Guid userId)
	{
		return repo.RegisterBook(title, author);
	}

	public Task<Book?> LookupBook(Guid bookId) => repo.LookupBook(bookId);

	public Task<Guid?> RegisterLoan(Guid bookId, Guid userId, DateTimeOffset expiryDate) =>
		repo.RegisterLoan(bookId, userId, expiryDate);

	public Task<Loan?> DeleteLoan(Guid loanId) => repo.DeleteLoan(loanId);

	public Task<IEnumerable<Loan>> GetLoans(User user) => repo.GetLoans(user);

	public Task<User?> GetUserById(Guid userId) => repo.GetUserById(userId);
}
