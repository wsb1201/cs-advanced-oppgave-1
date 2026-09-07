namespace LibrarySystem.Domain;

public class Book(string Title, string Author)
{
	public Guid Id { get; } = Guid.NewGuid();
	public string Title { get; } = Title;
	public string Author { get; } = Author;
	public bool IsBorrowed { get; private set; }

	public void Borrow()
	{
		IsBorrowed = true;
	}
}
