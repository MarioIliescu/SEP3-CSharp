using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiContracts.Dtos.Auth;
using ApiContracts.Dtos.Authetication;
using ApiContracts.Enums;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Services.Auth;

namespace FleetWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IConfiguration config, IAuthService authService)
    : ControllerBase
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
                return Ok(new { token });
            }
            else if (user is Dispatcher dispatcher)
            {
                string token = GenerateJwt(dispatcher);
                return Ok(new { token });
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
        var key = Encoding.UTF8.GetBytes(config["Jwt:Key"] ?? "");

        List<Claim> claims = GenerateClaims(user);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            Issuer = config["Jwt:Issuer"],
            Audience = config["Jwt:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    [HttpPost("refresh")]
    public ActionResult Refresh()
    {
        var user = User;
        if (user.Identity?.IsAuthenticated != true)
            return Unauthorized();

        var roleClaim = user.FindFirst("Role")?.Value;

        if (roleClaim == "Dispatcher")
        {
            var rebuiltDispatcher = RebuildUserFromClaims(user);
            var newToken = GenerateJwt(rebuiltDispatcher);
            return Ok(new { token = newToken });
        }
        else if (roleClaim == "Driver")
        {
            var rebuiltDriver = RebuildUserFromClaims(user);
            var newToken = GenerateJwt(rebuiltDriver);
            return Ok(new { token = newToken });
        }

        return Unauthorized();
    }


    private List<Claim> GenerateClaims(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, "FleetForward"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()
                    .ToString(),
                ClaimValueTypes.Integer64),
            new Claim("Id", user.Id.ToString()),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName),
            new Claim("Email", user.Email),
            new Claim("PhoneNumber", user.PhoneNumber),
            new Claim("Role", user.Role.ToString()),
            new Claim("PhotoUrl", user.PhotoUrl ?? "images/reddot.png"),
        };
        if (user is Dispatcher dispatcher)
        {
            claims.Add(new Claim("CurrentRate",
                dispatcher.Current_Rate.ToString()));
        }

        if (user is Driver driver)
        {
            claims.Add(new Claim("CompanyRole", driver.CompanyRole.ToString()));
            claims.Add(new Claim("CompanyMC", driver.McNumber));
            claims.Add(new Claim("DriverStatus", driver.Status.ToString()));
            claims.Add(new Claim("TrailerType",
                driver.Trailer_type.ToString()));
            claims.Add(new Claim("Location", driver.Location_State));
            claims.Add(new Claim("LocationZip",
                driver.Location_Zip_Code.ToString()));
        }

        return claims;
    }

    private User RebuildUserFromClaims(ClaimsPrincipal claims)
    {
        var idClaim = claims.FindFirst("Id")?.Value;
            var id = int.Parse(idClaim);
        var roleClaim = claims.FindFirst("Role")?.Value;
        var role = Enum.TryParse<UserRole>(roleClaim, out var parsedRole)
            ? parsedRole
            : throw new UnauthorizedAccessException(
                "Role claim missing or invalid");

        if (role == UserRole.Driver)
        {
            return new Driver.Builder()
                .SetId(id)
                .SetFirstName(claims.FindFirst("FirstName")?.Value ?? "")
                .SetLastName(claims.FindFirst("LastName")?.Value ?? "")
                .SetEmail(claims.FindFirst("Email")?.Value ?? "")
                .SetPhoneNumber(claims.FindFirst("PhoneNumber")?.Value ?? "")
                .SetCompanyRole(
                    Enum.TryParse<DriverCompanyRole>(
                        claims.FindFirst("CompanyRole")?.Value, out var cr)
                        ? cr
                        : DriverCompanyRole.Driver)
                .SetMcNumber(claims.FindFirst("CompanyMC")?.Value ?? "")
                .SetStatus(
                    Enum.TryParse<DriverStatus>(
                        claims.FindFirst("DriverStatus")?.Value, out var ds)
                        ? ds
                        : DriverStatus.Available)
                .SetTrailerType(
                    Enum.TryParse<TrailerType>(
                        claims.FindFirst("TrailerType")?.Value, out var tt)
                        ? tt
                        : TrailerType.Dry_van)
                .SetLocationState(claims.FindFirst("Location")?.Value ?? "")
                .SetLocationZip(
                    int.Parse(claims.FindFirst("LocationZip")?.Value ??
                              "35010"))
                .Build();
        }
        else if (role == UserRole.Dispatcher)
        {
            var rateClaim = claims.FindFirst("CurrentRate")?.Value;
            double rate = 0;
            if (!string.IsNullOrEmpty(rateClaim))
                double.TryParse(rateClaim, out rate);

            return new Dispatcher.Builder()
                .SetId(id)
                .SetFirstName(claims.FindFirst("FirstName")?.Value ?? "")
                .SetLastName(claims.FindFirst("LastName")?.Value ?? "")
                .SetEmail(claims.FindFirst("Email")?.Value ?? "")
                .SetPhoneNumber(claims.FindFirst("PhoneNumber")?.Value ?? "")
                .SetCurrentRate(rate)
                .Build();
        }
        else
        {
            throw new UnauthorizedAccessException("Unknown role");
        }
    }
}