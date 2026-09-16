namespace LibrarySystem.Domain;

public class Loan(Guid bookId, string patron, DateTimeOffset ExpiryDate)
{
	public Guid Id { get; private init; } = Guid.NewGuid();
	public Guid BookId { get; private init; } = bookId;
	public string Patron { get; private init; } = patron;
	public DateTimeOffset ExpiryDate { get; private init; } = ExpiryDate;

	public Loan()
		: this(Guid.Empty, string.Empty, DateTimeOffset.UnixEpoch) { }
}
