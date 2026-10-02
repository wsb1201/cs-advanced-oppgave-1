namespace LibrarySystem.Api.Controllers;

using System.IdentityModel.Tokens.Jwt;
using LibrarySystem.Domain;
using Microsoft.AspNetCore.Mvc;

public record struct RegisterLoanDto(Guid UserId, Guid BookId, DateTimeOffset ExpiryDate);

public record struct GetLoanDto(Guid LoanId, Guid BookId, DateTimeOffset ExpiryDate);

public record struct DeleteLoanDto(Guid UserId, bool Late);

[ApiController]
[Route("loans")]
public class LoanController(LibraryService service, IConfiguration config) : ControllerBase
{
	[HttpPost]
	[ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status409Conflict)]
	public async Task<ActionResult<Guid>> RegisterLoan([FromBody] RegisterLoanDto request)
	{
		var key = config["Jwt:Key"];
		if (string.IsNullOrEmpty(key))
			return StatusCode(500, "JWT key is missing.");

		JwtSecurityToken jwt;
		try
		{
			jwt = JwtService.ValidateAuthHeader(
				authHeader: HttpContext.Request.Headers.Authorization.ToString(),
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
			return Unauthorized("User ID could not be found");

		var user = await service.GetUserById(userId.Value);
		if (user is null)
			return StatusCode(500, "User does not exist");
		if (!user.IsLibrarian)
			return StatusCode(403, "User is not a librarian");

		try
		{
			var loanId = await service.RegisterLoan(
				request.BookId,
				request.UserId,
				request.ExpiryDate
			);
			return (loanId is null) ? NotFound() : Created(null as string, loanId);
		}
		catch (Book.AlreadyBorrowedException)
		{
			return Conflict();
		}
	}

	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<ActionResult<IEnumerable<GetLoanDto>>> GetLoans()
	{
		var key = config["Jwt:Key"];
		if (string.IsNullOrEmpty(key))
			return StatusCode(500, "JWT key is missing.");

		JwtSecurityToken jwt;
		try
		{
			jwt = JwtService.ValidateAuthHeader(
				authHeader: HttpContext.Request.Headers.Authorization.ToString(),
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

		return Ok(
			(await service.GetLoans(user)).Select(l => new GetLoanDto(l.Id, l.BookId, l.ExpiryDate))
		);
	}

	[HttpDelete("{id:guid}")]
	[ProducesResponseType(typeof(DeleteLoanDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<DeleteLoanDto>> DeleteLoan(Guid id)
	{
		var key = config["Jwt:Key"];
		if (string.IsNullOrEmpty(key))
			return StatusCode(500, "JWT key is missing.");

		JwtSecurityToken jwt;
		try
		{
			jwt = JwtService.ValidateAuthHeader(
				authHeader: HttpContext.Request.Headers.Authorization.ToString(),
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

		var loan = await service.DeleteLoan(id);
		return (loan is null)
			? NotFound()
			: Ok(new DeleteLoanDto(loan.UserId, loan.ExpiryDate < DateTime.Now));
	}
}
