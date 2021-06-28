using jwtplay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace jwtplay.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        public AccountController(IConfiguration configuration)
        {
            _config = configuration;
        }
        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var resp = new Dictionary<string, string>();
            if(!(request.UserName == "admin" && request.Password == "1111"))
            {
                resp.Add("Error", "Invalid username or password");
                return BadRequest(resp);
            }

            var roles = new string[] { "Role1", "Role2" };
            var token = GenerateJwtToken(request.UserName, roles.ToList());
            return Ok(new LoginResponse()
            {
                Access_Token = token,
                UserName = request.UserName
            });
        }

        private string GenerateJwtToken(string username, List<string> roles)
        {
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, username)
        };

            roles.ForEach(role =>
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            });

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_config["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                _config["JwtIssuer"],
                _config["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
