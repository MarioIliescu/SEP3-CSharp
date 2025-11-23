using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiContracts.Dtos.Auth;
using ApiContracts.Dtos.Authetication;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Services.Auth;

namespace FleetWebApi.Controllers;
[ApiController]
[Route("[controller]")]
public class AuthController(IConfiguration config, IAuthService authService) : ControllerBase
{

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest userLoginDto)
    {
        try
        {
            User user = await authService.LoginAsync(new User.Builder()
                .SetEmail(userLoginDto.Email)
                .SetPassword(userLoginDto.Password)
                .Build()) as User ?? throw new InvalidOperationException();
            string token = GenerateJwt(user);

            return Ok(new{token});
        }
        catch (Exception e)
        {
            return BadRequest(new { error = e.Message });
        }
    }

    private string GenerateJwt(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(config["Jwt:Key"] ?? "");

        List<Claim> claims = GenerateClaims(user);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = config["Jwt:Issuer"],
            Audience = config["Jwt:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private List<Claim> GenerateClaims(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "FleetForward"),      
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, 
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), 
                ClaimValueTypes.Integer64),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName),
            new Claim("Email", user.Email),
            new Claim("PhoneNumber", user.PhoneNumber),
            new Claim("Role", user.Role.ToString())
        };

        return claims.ToList();
    }

}
