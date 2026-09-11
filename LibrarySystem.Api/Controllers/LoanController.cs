namespace LibrarySystem.Api.Controllers;

using LibrarySystem.Domain;
using Microsoft.AspNetCore.Mvc;

public record struct RegisterLoanDto(Guid BookId, string Patron, DateTimeOffset ExpiryDate);

public record struct DeleteLoanDto(string Patron, bool Late);

[ApiController]
[Route("loans")]
public class LoanController(LibraryService service) : ControllerBase
{
	[HttpPost]
	[ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status409Conflict)]
	public async Task<ActionResult<Guid>> RegisterLoan([FromBody] RegisterLoanDto request)
	{
		throw new NotImplementedException();
	}

	[HttpDelete("{id:guid}")]
	[ProducesResponseType(typeof(DeleteLoanDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<DeleteLoanDto>> DeleteLoan(Guid id)
	{
		throw new NotImplementedException();
	}
}
