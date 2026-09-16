namespace LibrarySystem.Domain.Repositories;

public interface ILibraryRepository
{
	Task<Guid> RegisterBook(string title, string author);
	Task<Book?> LookupBook(Guid id);
	Task<Guid?> RegisterLoan(Guid bookId, string patron, DateTimeOffset expiryDate);
	Task<Loan?> DeleteLoan(Guid loanId);
}
