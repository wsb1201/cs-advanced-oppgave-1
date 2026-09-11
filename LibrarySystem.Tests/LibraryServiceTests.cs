namespace LibrarySystem.Tests;

using LibrarySystem.Domain;

public class LibraryServiceTests
{
	[Fact]
	public void Test_RegisterBook()
	{
		// Given
		var library = new LibraryService();

		// When
		var bookId = library.RegisterBook("Dune", "Frank Herbert");
		var book = library.LookupBook(bookId);

		// Then
		Assert.NotNull(book);
		Assert.Equal("Dune", book.Title);
		Assert.Equal("Frank Herbert", book.Author);
		Assert.Equal(bookId, book.Id);
		Assert.False(book.IsBorrowed);
	}

	[Fact]
	public void Test_BorrowBook()
	{
		// Given
		var library = new LibraryService();
		var bookId = library.RegisterBook("Foundation", "Isaac Asimov");
		var book = library.LookupBook(bookId);

		// When
		book?.Borrow();

		// Then
		Assert.NotNull(book);
		Assert.True(book.IsBorrowed);
	}

	[Fact]
	public void Test_BorrowedBook_Unavailable()
	{
		// Given
		var library = new LibraryService();
		var bookId = library.RegisterBook("Ringworld", "Larry Niven");
		var book = library.LookupBook(bookId);

		// When
		book?.Borrow();

		// Then
		Assert.NotNull(book);
		Assert.Throws<Book.AlreadyBorrowedException>(book.Borrow);
	}

	[Fact]
	public void Test_RegisterLoan()
	{
		// Given
		var library = new LibraryService();
		var bookId = library.RegisterBook("The Silmarillion", "J. R. R. Tolkien");
		var book = library.LookupBook(bookId)!;

		// When
		library.RegisterLoan(book.Id, "Jarod Whited", DateTimeOffset.UtcNow.AddDays(7));

		// Then
		Assert.True(book.IsBorrowed);
	}

	[Fact]
	public void Test_DeleteLoan()
	{
		// Given
		var library = new LibraryService();
		var bookId = library.RegisterBook("Types and Programming Languages", "Benjamin C. Pierce");
		var book = library.LookupBook(bookId)!;
		var loanId = library.RegisterLoan(
			book.Id,
			"Emmett Swoyer",
			DateTimeOffset.UtcNow.AddDays(7)
		)!;

		// When
		library.DeleteLoan((Guid)loanId);

		// Then
		Assert.False(book.IsBorrowed);
	}
}
