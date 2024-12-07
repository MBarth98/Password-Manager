using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/passwords")]
    [Authorize]
    public class PasswordController : ControllerBase
    {
        private readonly PasswordService _passwordService;

        public PasswordController(PasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        [HttpPost]
        public async Task<IActionResult> AddPassword([FromBody] AddPasswordDto addPasswordDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                await _passwordService.AddPasswordAsync(userId, addPasswordDto);
                return Ok("Password added successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPasswords()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var passwords = await _passwordService.GetPasswordsAsync(userId);
                return Ok(passwords);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
