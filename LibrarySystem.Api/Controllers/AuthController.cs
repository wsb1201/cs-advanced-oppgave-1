namespace LibrarySystem.Api.Controllers;

using System.IdentityModel.Tokens.Jwt;
using LibrarySystem.Domain;
using LibrarySystem.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

public record struct TokenInfoDto(object Header, IEnumerable<TokenClaimDto> Claims);

public record struct TokenClaimDto(string Type, string Value);

public record struct LoginRequestDto(string Username, string Password);

[ApiController]
[Route("auth")]
[ProducesResponseType(StatusCodes.Status201Created)]
public class AuthController(ILibraryRepository repo, IConfiguration config) : ControllerBase
{
	[HttpPost("register")]
	public async Task<IActionResult> Register(LoginRequestDto request)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		return await new AuthService(repo).RegisterUser(request.Username, request.Password)
			? CreatedAtAction(nameof(Login), "New user has been registered.")
			: BadRequest("Username is already taken.");
	}

	[HttpPost("login")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> Login(LoginRequestDto request)
	{
		var user = await new AuthService(repo).VerifyUser(request.Username, request.Password);
		if (user is null)
			return Unauthorized("Invalid username or password");

		var key = config["Jwt:Key"];
		return string.IsNullOrEmpty(key)
			? StatusCode(500, "JWT key is missing.")
			: Ok(new { token = JwtService.CreateToken(user, key) });
	}

	[HttpGet("token")]
	[ProducesResponseType(typeof(TokenInfoDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public ActionResult<TokenInfoDto> GetTokenInfo()
	{
		JwtSecurityToken jwt;
		try
		{
			jwt = JwtService.ParseAuthHeader(HttpContext.Request.Headers.Authorization.ToString());
		}
		catch (InvalidAuthHeaderException ex)
		{
			return Unauthorized(ex.Message);
		}
		catch (ArgumentException)
		{
			return Unauthorized("Malformed JWT");
		}
		return Ok(
			new TokenInfoDto(
				jwt.Header,
				Claims: jwt.Claims.Select(c => new TokenClaimDto(c.Type, c.Value))
			)
		);
	}
}
