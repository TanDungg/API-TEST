using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using TestAPI.Data;
using TestAPI.Models;

namespace TestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetList()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountDetails(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("Tài khoản không tồn tại!");
            }

            return Ok(user);
        }

        [HttpPost()]
        public async Task<IActionResult> Register(Account user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(user.Password) || !HasSpecialCharacter(user.Password))
            {
                return BadRequest("Mật khẩu phải có 1 ký tự đặc biệt!");
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);
            if (existingUser != null)
            {
                return BadRequest("Tên đăng nhập đã tồn tại!");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password); // Hash password
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Đăng ký tài khoản thành công!");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Account updatedUser)
        {
            if (id != updatedUser.Id)
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(updatedUser.Password) || !HasSpecialCharacter(updatedUser.Password))
            {
                return BadRequest("Mật khẩu phải có ít nhất 1 ký tự đặc biêt!");
            }

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound("Tài khoản không tồn tại!");
            }

            existingUser.UserName = updatedUser.UserName;
            existingUser.Email = updatedUser.Email;
            existingUser.Password = BCrypt.Net.BCrypt.HashPassword(updatedUser.Password); // Mã hóa mật khẩu

            _context.Entry(existingUser).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok("Chỉnh sửa tài khoản thành công!");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("Tài khoản không tồn tại!");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("Xóa tài khoản thành công!");
        }


        private bool HasSpecialCharacter(string input)
        {
            var specialCharacterPattern = @"[!@#$%^&*(),.?""':{}|<>]";
            return Regex.IsMatch(input, specialCharacterPattern);
        }
    }
}
