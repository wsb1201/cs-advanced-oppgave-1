namespace LibrarySystem.Api.Controllers;

using LibrarySystem.Domain;
using Microsoft.AspNetCore.Mvc;

public record struct RegisterBookDto(string Title, string Author);

public record struct LookupBookDto(string Title, string Author, bool Available);

[ApiController]
[Route("books")]
public class BookController(LibraryService service) : ControllerBase
{
	[HttpPost]
	[ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<Guid>> RegisterBook([FromBody] RegisterBookDto request)
	{
		var bookId = service.RegisterBook(request.Title, request.Author);
		return CreatedAtAction(nameof(LookupBook), new { id = bookId }, bookId);
	}

	[HttpGet("{id:guid}")]
	[ProducesResponseType(typeof(LookupBookDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<LookupBookDto>> LookupBook(Guid id)
	{
		var book = service.LookupBook(id);
		return book is null
			? NotFound()
			: Ok(new LookupBookDto(book.Title, book.Author, !book.IsBorrowed));
	}
}
