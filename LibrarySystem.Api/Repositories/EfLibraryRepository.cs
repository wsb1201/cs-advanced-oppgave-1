namespace LibrarySystem.Api.Repositories;

using LibrarySystem.Api.Data;
using LibrarySystem.Domain;
using LibrarySystem.Domain.Repositories;

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

	public async Task<Guid?> RegisterLoan(Guid bookId, string patron, DateTimeOffset expiryDate)
	{
		var book = await LookupBook(bookId);
		if (book is null)
			return null;

		book.Borrow();

		var loan = new Loan(bookId, patron, expiryDate);
		await db.Loans.AddAsync(loan);
		await db.SaveChangesAsync();
		return loan.Id;
	}

	public async Task<Loan?> DeleteLoan(Guid loanId)
	{
		var loan = db.Loans.Find(loanId);
		if (loan is null)
			return null;

		var book = await LookupBook(loan.BookId);
		book?.MakeAvailable();
		db.Loans.Remove(loan);
		await db.SaveChangesAsync();
		return loan;
	}
}
