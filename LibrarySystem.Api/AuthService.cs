namespace LibrarySystem.Api;

using BCrypt.Net;
using LibrarySystem.Domain;
using LibrarySystem.Domain.Repositories;

public class AuthService(ILibraryRepository repo)
{
	public async Task<bool> RegisterUser(string username, string password)
	{
		return await repo.AddUser(username, BCrypt.HashPassword(password));
	}

	public async Task<User?> VerifyUser(string username, string password)
	{
		var user = await repo.GetUserByName(username);
		return user != null && BCrypt.Verify(password, user.Password) ? user : null;
	}
}
