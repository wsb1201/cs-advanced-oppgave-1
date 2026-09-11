namespace LibrarySystem.Domain;

public class Loan(Guid BookId, string Patron, DateTimeOffset ExpiryDate)
{
	public Guid Id { get; } = Guid.NewGuid();
	public Guid BookId { get; } = BookId;
	public string Patron { get; } = Patron;
	public DateTimeOffset ExpiryDate { get; } = ExpiryDate;
}
