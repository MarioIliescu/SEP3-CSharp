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
            var user = await authService.LoginAsync(new User.Builder()
                .SetEmail(userLoginDto.Email)
                .SetPassword(userLoginDto.Password)
                .Build()) ?? throw new InvalidOperationException();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (user is Driver driver)
            {
                string token = GenerateJwt(driver);
                return Ok(new{token});
            }
            else if (user is Dispatcher dispatcher)
            {
                string token = GenerateJwt(dispatcher);
                return Ok(new{token});
            }
            throw new Exception("Something went wrong. Try again");
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
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, "FleetForward"),      
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, 
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), 
                ClaimValueTypes.Integer64),
            new Claim("Id", user.Id.ToString()),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName),
            new Claim("Email", user.Email),
            new Claim("PhoneNumber", user.PhoneNumber),
            new Claim("Role", user.Role.ToString())
        };
        if (user is Dispatcher dispatcher)
        {
            claims.Add(new Claim("CurrentRate", dispatcher.Current_Rate.ToString()));
        }

        if (user is Driver driver)
        {
            claims.Add(new Claim("CompanyRole", driver.CompanyRole.ToString()));
            claims.Add(new Claim("CompanyMC", driver.McNumber));
            claims.Add(new Claim("DriverStatus", driver.Status.ToString()));
            claims.Add(new Claim("TrailerType", driver.Trailer_type.ToString()));
            claims.Add(new Claim("Location",driver.Location_State));
            claims.Add(new Claim("LocationZip",driver.Location_Zip_Code.ToString()));
        }

        return claims;
    }

}
