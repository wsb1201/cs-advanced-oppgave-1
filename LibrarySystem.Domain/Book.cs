namespace LibrarySystem.Domain;

public class Book(string title, string author)
{
	public Guid Id { get; private init; } = Guid.NewGuid();
	public string Title { get; private init; } = title;
	public string Author { get; private init; } = author;
	public bool IsBorrowed { get; private set; }

	public Book()
		: this(string.Empty, string.Empty) { }

	public void Borrow()
	{
		if (IsBorrowed)
			throw new AlreadyBorrowedException();

		IsBorrowed = true;
	}

	public void MakeAvailable()
	{
		if (!IsBorrowed)
			throw new InvalidOperationException("Book is not borrowed.");
		IsBorrowed = false;
	}

	public class AlreadyBorrowedException : InvalidOperationException
	{
		public AlreadyBorrowedException()
			: base("Book is already borrowed.") { }
	}
}
