namespace LibrarySystem.Api.Data;

using LibrarySystem.Domain;
using Microsoft.EntityFrameworkCore;

public sealed class LibraryDbContext(DbContextOptions<LibraryDbContext> options)
	: DbContext(options)
{
	public required DbSet<Book> Books { get; set; }
	public required DbSet<User> Users { get; set; }
	public required DbSet<Loan> Loans { get; set; }
}
