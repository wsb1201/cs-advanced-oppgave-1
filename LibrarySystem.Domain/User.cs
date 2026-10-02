namespace LibrarySystem.Domain;

using System.ComponentModel.DataAnnotations;

public class User
{
	public Guid Id { get; private init; } = Guid.NewGuid();
	public required string Username { get; set; } = string.Empty;

	[Required]
	[DataType(DataType.Password)]
	public required string Password { get; set; } = string.Empty;

	public bool IsLibrarian { get; set; } = false;
}
