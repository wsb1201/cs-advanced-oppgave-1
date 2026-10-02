namespace LibrarySystem.Domain;

public class Loan(Guid bookId, Guid userId, DateTimeOffset expiryDate)
{
	public Guid Id { get; private init; } = Guid.NewGuid();
	public Guid BookId { get; private init; } = bookId;
	public Guid UserId { get; private init; } = userId;
	public DateTimeOffset ExpiryDate { get; private init; } = expiryDate;

	public Loan()
		: this(Guid.Empty, Guid.Empty, DateTimeOffset.UnixEpoch) { }
}
