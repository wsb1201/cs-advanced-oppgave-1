namespace LibrarySystem.Tests;

using System.Net;
using System.Net.Http.Json;
using LibrarySystem.Api.Controllers;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public sealed class LibraryServiceApiTests(WebApplicationFactory<Program> factory)
	: IClassFixture<WebApplicationFactory<Program>>
{
	private readonly HttpClient _client = factory.CreateClient();

	[Fact]
	public async Task RegisterBook_WithValidData_ReturnsCreated()
	{
		var book = new { Title = "Dune", Author = "Frank Herbert" };

		using var response = await _client.PostAsJsonAsync("/books", book);

		Assert.Equal(HttpStatusCode.Created, response.StatusCode);

		var bookId = await ReadRequiredAsync<Guid>(response);

		Assert.NotEqual(Guid.Empty, bookId);
	}

	[Fact]
	public async Task RegisterBook_WithInvalidData_ReturnsBadRequest()
	{
		var malformedBook = new { Sanity = 0, Nonsense = true };

		using var response = await _client.PostAsJsonAsync("/books", malformedBook);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task LookupBook_WhenBookExists_ReturnsOk()
	{
		var expectedBook = new { Title = "The Great Gatsby", Author = "F. Scott Fitzgerald" };

		var bookId = await RegisterBookAsync(expectedBook.Title, expectedBook.Author);

		using var response = await _client.GetAsync($"/books/{bookId}");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var book = await ReadRequiredAsync<LookupBookDto>(response);

		Assert.Equal(expectedBook.Title, book.Title);
		Assert.Equal(expectedBook.Author, book.Author);
		Assert.True(book.Available);
	}

	[Fact]
	public async Task LookupBook_WhenBookDoesNotExist_ReturnsNotFound()
	{
		var unknownBookId = Guid.NewGuid();

		using var response = await _client.GetAsync($"/books/{unknownBookId}");

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	[Fact]
	public async Task RegisterLoan_WithValidData_ReturnsCreated()
	{
		var bookId = await RegisterBookAsync(
			"Principles of Compiler Design",
			"Alfred Aho and Jeffrey Ullman"
		);

		var loan = new
		{
			BookId = bookId,
			ExpiryDate = DateTimeOffset.UtcNow.AddHours(1),
			Patron = "Ashley Otsman",
		};

		using var response = await _client.PostAsJsonAsync("/loans", loan);

		Assert.Equal(HttpStatusCode.Created, response.StatusCode);

		var loanId = await ReadRequiredAsync<Guid>(response);

		Assert.NotEqual(Guid.Empty, loanId);
	}

	[Fact]
	public async Task RegisterLoan_WhenBookIsAlreadyLoaned_ReturnsConflict()
	{
		var bookId = await RegisterBookAsync("1984", "George Orwell");

		var firstLoan = new
		{
			BookId = bookId,
			ExpiryDate = DateTimeOffset.UtcNow.AddHours(1),
			Patron = "Arnoldo Danielski",
		};

		using var firstResponse = await _client.PostAsJsonAsync("/loans", firstLoan);

		Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

		var secondLoan = new
		{
			BookId = bookId,
			ExpiryDate = DateTimeOffset.UtcNow.AddHours(1),
			Patron = "Pricilla Ruddle",
		};

		using var secondResponse = await _client.PostAsJsonAsync("/loans", secondLoan);

		Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
	}

	[Fact]
	public async Task DeleteLoan_WhenLoanExists_ReturnsDeletedLoan()
	{
		var bookId = await RegisterBookAsync("1984", "George Orwell");

		var loan = new
		{
			BookId = bookId,
			ExpiryDate = DateTimeOffset.UtcNow.AddHours(1),
			Patron = "Julian Hidvegi",
		};

		var loanId = await RegisterLoanAsync(loan);

		using var response = await _client.DeleteAsync($"/loans/{loanId}");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var removedLoan = await ReadRequiredAsync<DeleteLoanDto>(response);

		Assert.Equal(loan.Patron, removedLoan.Patron);
		Assert.False(removedLoan.Late);
	}

	private async Task<Guid> RegisterBookAsync(string title, string author)
	{
		var book = new { Title = title, Author = author };

		using var response = await _client.PostAsJsonAsync("/books", book);
		Assert.Equal(HttpStatusCode.Created, response.StatusCode);

		var bookId = await ReadRequiredAsync<Guid>(response);
		Assert.NotEqual(Guid.Empty, bookId);

		return bookId;
	}

	private async Task<Guid> RegisterLoanAsync(object loan)
	{
		using var response = await _client.PostAsJsonAsync("/loans", loan);
		Assert.Equal(HttpStatusCode.Created, response.StatusCode);

		var loanId = await ReadRequiredAsync<Guid>(response);
		Assert.NotEqual(Guid.Empty, loanId);

		return loanId;
	}

	private static async Task<T> ReadRequiredAsync<T>(HttpResponseMessage response)
	{
		var value = await response.Content.ReadFromJsonAsync<T>();
		Assert.NotNull(value);
		return value;
	}
}
