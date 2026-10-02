namespace LibrarySystem.Api.Controllers;

using System.IdentityModel.Tokens.Jwt;
using LibrarySystem.Domain;
using Microsoft.AspNetCore.Mvc;

public record struct RegisterBookDto(string Title, string Author);

public record struct LookupBookDto(string Title, string Author, bool Available);

[ApiController]
[Route("books")]
public class BookController(LibraryService service, IConfiguration config) : ControllerBase
{
	[HttpPost]
	[ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<Guid>> RegisterBook([FromBody] RegisterBookDto request)
	{
		var key = config["Jwt:Key"];
		if (string.IsNullOrEmpty(key))
			return StatusCode(500, "JWT key is missing.");

		JwtSecurityToken jwt;
		try
		{
			jwt = JwtService.ValidateAuthHeader(
				HttpContext.Request.Headers.Authorization.ToString(),
				key
			);
		}
		catch (Microsoft.IdentityModel.Tokens.SecurityTokenValidationException ex)
		{
			return Unauthorized(ex.Message);
		}
		catch (InvalidAuthHeaderException ex)
		{
			return Unauthorized(ex.Message);
		}
		catch (ArgumentException)
		{
			return Unauthorized("Malformed JWT");
		}

		var userId = JwtService.GetUserId(jwt);
		if (userId is null)
			return Unauthorized("Authorization token must contain a user ID");

		var user = await service.GetUserById(userId.Value);
		if (user is null)
			return StatusCode(500, "User does not exist");
		if (!user.IsLibrarian)
			return StatusCode(403, "User is not a librarian");

		var bookId = await service.RegisterBook(request.Title, request.Author, userId.Value);
		return CreatedAtAction(nameof(LookupBook), new { id = bookId }, bookId);
	}

	[HttpGet("{id:guid}")]
	[ProducesResponseType(typeof(LookupBookDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<LookupBookDto>> LookupBook(Guid id)
	{
		var book = await service.LookupBook(id);
		return book is null
			? NotFound()
			: Ok(new LookupBookDto(book.Title, book.Author, !book.IsBorrowed));
	}
}
