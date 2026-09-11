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
		try
		{
			var loanId = service.RegisterLoan(request.BookId, request.Patron, request.ExpiryDate);
			return (loanId is null) ? NotFound() : Created(null as string, loanId);
		}
		catch (Book.AlreadyBorrowedException)
		{
			return Conflict();
		}
	}

	[HttpDelete("{id:guid}")]
	[ProducesResponseType(typeof(DeleteLoanDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<DeleteLoanDto>> DeleteLoan(Guid id)
	{
		var loan = service.DeleteLoan(id);
		return (loan is null)
			? NotFound()
			: Ok(new DeleteLoanDto(loan.Patron, loan.ExpiryDate < DateTime.Now));
	}
}
