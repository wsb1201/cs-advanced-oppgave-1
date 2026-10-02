namespace LibrarySystem.Api.Repositories;

using LibrarySystem.Api.Data;
using LibrarySystem.Domain;
using LibrarySystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

public class EfLibraryRepository(LibraryDbContext db) : ILibraryRepository
{
	public async Task<Guid> RegisterBook(string title, string author)
	{
		var book = new Book(title, author);
		await db.Books.AddAsync(book);
		await db.SaveChangesAsync();
		return book.Id;
	}

	public async Task<Book?> LookupBook(Guid bookId) => await db.Books.FindAsync(bookId);

	public async Task<Guid?> RegisterLoan(Guid bookId, Guid userId, DateTimeOffset expiryDate)
	{
		var user = await GetUserById(userId);
		if (user is null)
			return null;

		var book = await LookupBook(bookId);
		if (book is null)
			return null;

		book.Borrow();

		var loan = new Loan(bookId, userId, expiryDate);
		await db.Loans.AddAsync(loan);
		await db.SaveChangesAsync();
		return loan.Id;
	}

	public async Task<Loan?> DeleteLoan(Guid loanId)
	{
		var loan = await db.Loans.FindAsync(loanId);
		if (loan is null)
			return null;

		var book = await LookupBook(loan.BookId);
		book?.MakeAvailable();
		db.Loans.Remove(loan);
		await db.SaveChangesAsync();
		return loan;
	}

	public async Task<IEnumerable<Loan>> GetLoans(User user) =>
		(await db.Loans.Where(loan => loan.UserId == user.Id).ToListAsync()).AsEnumerable();

	public async Task<bool> AddUser(string name, string hash, bool librarian)
	{
		if (await db.Users.AnyAsync(usr => usr.Username == name))
			return false;
		await db.Users.AddAsync(
			new User
			{
				Username = name,
				Password = hash,
				IsLibrarian = librarian,
			}
		);
		await db.SaveChangesAsync();
		return true;
	}

	public Task<User?> GetUserByName(string name) =>
		db.Users.FirstOrDefaultAsync(usr => usr.Username == name);

	public async Task<User?> GetUserById(Guid userId) => await db.Users.FindAsync(userId);
}
