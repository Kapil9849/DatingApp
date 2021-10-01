using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    
    public class AccountController: BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _token;
        public AccountController(DataContext context, ITokenService token)
        {
            _token = token;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto dto)
        {
            if (await UserExsts(dto.userName))
                return BadRequest("Username is taken");
            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                userName = dto.userName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                userName=user.userName,
                Token=_token.CreateToken(user)
            };
        }

        
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto dto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.userName == dto.userName);
            if (user == null)
                return Unauthorized("Invalid User");
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedhash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));

            for (int i = 0; i < computedhash.Length; i++)
            {
                if (computedhash[i] != user.PasswordHash[i])
                    return Unauthorized("Ivalid Password");
            }

             return new UserDto
            {
                userName=user.userName,
                Token=_token.CreateToken(user)
            };
        }

        private async Task<bool> UserExsts(string username)
        {
            return await _context.Users.AnyAsync(x => x.userName == username.ToLower());
        }
    }
}