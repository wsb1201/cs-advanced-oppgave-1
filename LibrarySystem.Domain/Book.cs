namespace LibrarySystem.Domain;

public class Book(string Title, string Author)
{
	public Guid Id { get; } = Guid.NewGuid();
	public string Title { get; } = Title;
	public string Author { get; } = Author;
	public bool IsBorrowed { get; private set; }

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
