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
		var book = library.Lookup(bookId);

		// Then
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
		var book = library.Lookup(bookId);

		// When
		book.Borrow();

		// Then
		Assert.True(book.IsBorrowed);
	}

	[Fact]
	public void Test_BorrowedBook_Unavailable()
	{
		// Given
		var library = new LibraryService();
		var bookId = library.RegisterBook("Foundation", "Isaac Asimov");
		var book = library.Lookup(bookId);

		// When
		book.Borrow();

		// Then
		Assert.Throws<InvalidOperationException>(book.Borrow);
	}
}
