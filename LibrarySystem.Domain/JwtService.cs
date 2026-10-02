namespace LibrarySystem.Domain;

using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public static class JwtService
{
	public static Guid? GetUserId(JwtSecurityToken token) =>
		Guid.TryParse(token.Subject, out Guid id) ? id : null;

	public static string CreateToken(User user, string key) =>
		new JwtSecurityTokenHandler().WriteToken(
			new JwtSecurityToken(
				claims:
				[
					new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
					new(JwtRegisteredClaimNames.UniqueName, user.Username),
				],
				expires: DateTime.UtcNow.AddMinutes(15),
				signingCredentials: new SigningCredentials(
					new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
					SecurityAlgorithms.HmacSha256
				)
			)
		);

	public static JwtSecurityToken ValidateAuthHeader(string authHeader, string key)
	{
		new JwtSecurityTokenHandler().ValidateToken(
			token: StripBearerPrefix(authHeader),
			validationParameters: new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
				ValidAlgorithms = [SecurityAlgorithms.HmacSha256],

				ValidateLifetime = true,
				RequireExpirationTime = true,
				ClockSkew = TimeSpan.FromMinutes(1),

				ValidateIssuer = false,
				ValidateAudience = false,
			},
			validatedToken: out var validatedToken
		);
		return (JwtSecurityToken)validatedToken;
	}

	public static JwtSecurityToken ParseAuthHeader(string? authHeader) =>
		string.IsNullOrWhiteSpace(authHeader)
			? throw new InvalidAuthHeaderException("Missing Authorization header.")
			: new JwtSecurityTokenHandler().ReadJwtToken(StripBearerPrefix(authHeader));

	private static string StripBearerPrefix(string authHeader)
	{
		const string BEARER_PREFIX = "Bearer ";
		if (!authHeader.StartsWith(BEARER_PREFIX))
			throw new InvalidAuthHeaderException(
				"Authorization header must use Bearer authentication"
			);
		return authHeader[BEARER_PREFIX.Length..].Trim();
	}
}

public sealed class InvalidAuthHeaderException(string? message) : Exception(message);
