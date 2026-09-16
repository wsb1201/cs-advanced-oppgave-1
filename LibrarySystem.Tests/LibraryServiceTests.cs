namespace LibrarySystem.Tests;

using LibrarySystem.Domain;
using LibrarySystem.Domain.Repositories;

public class LibraryServiceTests
{
	[Fact]
	public async Task Test_RegisterBook()
	{
		// Given
		var library = new LibraryService(new MemLibraryRepository());

		// When
		var bookId = await library.RegisterBook("Dune", "Frank Herbert");
		var book = await library.LookupBook(bookId);

		// Then
		Assert.NotNull(book);
		Assert.Equal("Dune", book.Title);
		Assert.Equal("Frank Herbert", book.Author);
		Assert.Equal(bookId, book.Id);
		Assert.False(book.IsBorrowed);
	}

	[Fact]
	public async Task Test_BorrowBook()
	{
		// Given
		var library = new LibraryService(new MemLibraryRepository());
		var bookId = await library.RegisterBook("Foundation", "Isaac Asimov");
		var book = await library.LookupBook(bookId);

		// When
		book?.Borrow();

		// Then
		Assert.NotNull(book);
		Assert.True(book.IsBorrowed);
	}

	[Fact]
	public async Task Test_BorrowedBook_Unavailable()
	{
		// Given
		var library = new LibraryService(new MemLibraryRepository());
		var bookId = await library.RegisterBook("Ringworld", "Larry Niven");
		var book = await library.LookupBook(bookId);

		// When
		book?.Borrow();

		// Then
		Assert.NotNull(book);
		Assert.Throws<Book.AlreadyBorrowedException>(book.Borrow);
	}

	[Fact]
	public async Task Test_RegisterLoan()
	{
		// Given
		var library = new LibraryService(new MemLibraryRepository());
		var bookId = await library.RegisterBook("The Silmarillion", "J. R. R. Tolkien");
		var book = (await library.LookupBook(bookId))!;

		// When
		await library.RegisterLoan(book.Id, "Jarod Whited", DateTimeOffset.UtcNow.AddDays(7));

		// Then
		Assert.True(book.IsBorrowed);
	}

	[Fact]
	public async Task Test_DeleteLoan()
	{
		// Given
		var library = new LibraryService(new MemLibraryRepository());
		var bookId = await library.RegisterBook(
			"Types and Programming Languages",
			"Benjamin C. Pierce"
		);
		var book = (await library.LookupBook(bookId))!;
		var loanId = await library.RegisterLoan(
			book.Id,
			"Emmett Swoyer",
			DateTimeOffset.UtcNow.AddDays(7)
		);

		// When
		await library.DeleteLoan((Guid)loanId!);

		// Then
		Assert.False(book.IsBorrowed);
	}
}
