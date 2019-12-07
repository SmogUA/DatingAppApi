using DatingApp.Data;
using DatingApp.Dtos;
using DatingApp.Models;
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

namespace DatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
           _repo = repo;
           _config = config;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto UserForRegDTO)
        {
            //validate request
            UserForRegDTO.Username = UserForRegDTO.Username.ToLower();
            if (await _repo.UserExists(UserForRegDTO.Username))
                return BadRequest("Username already exists");
            var userToCreate = new User
            {
                Username = UserForRegDTO.Username
            };
            var createdUser = await _repo.Register(userToCreate, UserForRegDTO.Password);
            return StatusCode(201);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDTO UserForLoginDTO)
        {
            var userFormRepo = await _repo.Login(UserForLoginDTO.Username.ToLower(), UserForLoginDTO.Password);
            if (userFormRepo == null) return Unauthorized();

            var claims = new[]
            {
              new Claim(ClaimTypes.NameIdentifier, userFormRepo.ID.ToString()),
              new Claim(ClaimTypes.Name, userFormRepo.Username)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { 
            token= tokenHandler.WriteToken(token)
            });
        }
    }
}
