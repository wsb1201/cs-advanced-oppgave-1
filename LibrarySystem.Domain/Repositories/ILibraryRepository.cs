namespace LibrarySystem.Domain.Repositories;

public interface ILibraryRepository
{
	Task<Guid> RegisterBook(string title, string author);
	Task<Book?> LookupBook(Guid id);
	Task<Guid?> RegisterLoan(Guid bookId, Guid userId, DateTimeOffset expiryDate);
	Task<IEnumerable<Loan>> GetLoans(User user);
	Task<Loan?> DeleteLoan(Guid loanId);

	Task<bool> AddUser(string name, string hash, bool librarian = false);
	Task<User?> GetUserByName(string name);
	Task<User?> GetUserById(Guid id);
}
