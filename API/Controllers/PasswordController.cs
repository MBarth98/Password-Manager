using System.Security.Claims;
using Application;
using Application.DTOs;
using Application.helper;
using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class PasswordController : ControllerBase
{
    private readonly PasswordService _passwordService;
    private readonly UserService _userService;

    public PasswordController(PasswordService passwordService, UserService userService)
    {
        _passwordService = passwordService;
        _userService = userService;
    }
    
    [AllowAnonymous]
    [HttpPost]
    [Route("generate")]
    public async Task<IActionResult> Generate([FromBody] GeneratePasswordRequest filter)
    {
        try
        {
            if (filter.minLength > filter.maxLength)
            {
                filter.maxLength = filter.minLength;
            }
            
            if (!filter.useLowercase && !filter.useUppercase && !filter.useNumbers && !filter.useSpecialCharacters)
            {
                return BadRequest("At least one character type must be selected.");
            }
            
            if (filter.minLength < 1)
            {
                return BadRequest("Minimum length must be greater than 0.");
            }
            
            return Ok(PasswordGenerator.GeneratePassword(filter.minLength, filter.maxLength, filter.useLowercase, filter.useUppercase, filter.useNumbers, filter.useSpecialCharacters));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create([FromBody] PasswordCreate create)
    {
        try
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("User is not authenticated.");
            }

            var user = await _userService.GetByIdAsync(userId);

            if (user.PasswordHash != new PasswordHasher<User>().HashPassword(user, create.MasterPassword))
            {
                // do not announce that the master password is incorrect to avoid brute force attacks (with stolen JWT tokens)
                return Unauthorized("Master password is incorrect.");
            }
            
            var password = PasswordEncryption.Encrypt(create.ServicePassword, create.MasterPassword + userId);
            var result = await _passwordService.CreateAsync(new Password()
            {
                UserId = userId, 
                ServiceName = create.ServiceName, 
                ServiceUsername = create.ServiceUsername, 
                ServicePassword = password.Cipher,
                IV = password.IV
            });
            
            return Ok(new PasswordResponse()
            {
                Id = result.Id,
                ServiceUsername = result.ServiceUsername,
                ServicePassword = result.ServicePassword,
                ServiceName = result.ServiceName
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet]
    [Route("getAllByUser")]
    public async Task<IActionResult> GetAllByUser()
    {
        try
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized("User is not authenticated.");
            }
            
            var passwords = await _passwordService.GetAllByUserAsync(userId);
            
            return Ok(passwords.Select(x => new PasswordResponse()
            {
                Id = x.Id,
                ServiceName = x.ServiceName,
                ServicePassword = x.ServicePassword,
                ServiceUsername = x.ServiceUsername
            }));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost]
    [Route("unlock")]
    public async Task<IActionResult> UnlockAll([FromBody] UnlockRequest request)
    {
        try
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized("User is not authenticated.");
            }
            
            var user = await _userService.GetByIdAsync(userId);

            if (user.PasswordHash != new PasswordHasher<User>().HashPassword(user, request.MasterPassword))
            {
                // do not announce that the master password is incorrect to avoid brute force attacks (with stolen JWT tokens)
                return Unauthorized("Master password is incorrect.");
            }
            
            var passwords = await _passwordService.GetAllByUserAsync(userId);
            
            foreach (var password in passwords)
            {
                password.ServicePassword = PasswordEncryption.Decrypt(password.ServicePassword, request.MasterPassword + userId, password.IV);
            }

            return Ok(passwords.Select(x => new PasswordResponse()
            {
                Id = x.Id,
                ServiceName = x.ServiceName,
                ServicePassword = x.ServicePassword,
                ServiceUsername = x.ServiceUsername
            }));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] PasswordUpdate update)
    {
        try
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                return Unauthorized("User is not authenticated.");
            }
            
            var existing = await _passwordService.GetByIdAsync(update.Id);
            
            if (existing.UserId != userId)
            {
                return Unauthorized("User is not authorized to update this password.");
            }
            
            var user = await _userService.GetByIdAsync(userId);

            if (user.PasswordHash != new PasswordHasher<User>().HashPassword(user, update.MasterPassword))
            {
                // do not announce that the master password is incorrect to avoid brute force attacks (with stolen JWT tokens)
                return Unauthorized("Master password is incorrect.");
            }
            
            var password = PasswordEncryption.Encrypt(update.ServicePassword, update.MasterPassword + userId);
            
            existing.ServiceName = update.ServiceName;
            existing.ServicePassword = password.Cipher;
            existing.IV = password.IV;
            
            await _passwordService.UpdateAsync(existing);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpDelete]
    [Route("delete/{request}")]
    public async Task<IActionResult> Delete([FromRoute] int request)
    {
        try
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized("User is not authenticated.");
            }

            var password = await _passwordService.GetByIdAsync(request);
            
            if (password.UserId != userId)
            {
                return Unauthorized("User is not authorized to delete this password.");
            }
            
            await _passwordService.DeleteAsync(request);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}