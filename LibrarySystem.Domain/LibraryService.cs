namespace LibrarySystem.Domain;

using Repositories;

public class LibraryService(ILibraryRepository repo)
{
	public Task<Guid> RegisterBook(string title, string author) => repo.RegisterBook(title, author);

	public Task<Book?> LookupBook(Guid bookId) => repo.LookupBook(bookId);

	public Task<Guid?> RegisterLoan(Guid bookId, string patron, DateTimeOffset expiryDate) =>
		repo.RegisterLoan(bookId, patron, expiryDate);

	public Task<Loan?> DeleteLoan(Guid loanId) => repo.DeleteLoan(loanId);
}
