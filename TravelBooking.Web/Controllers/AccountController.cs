using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Users.CreateUser;
using TravelBooking.Application.Users.SignIn;
using TravelBooking.Application.Users.SignOut;
using TravelBooking.Domain.Users.Exceptions;

namespace TravelBooking.Web.Controllers;

[Route("api/accounts")]
[ApiController]
public class AccountController:ControllerBase
{
    private readonly ISender _sender;
    public AccountController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp(CreateUserCommand command)
    {
        try
        {
            var userId=await _sender.Send(command);
            return Ok(userId);
        }
        catch (EmailAlreadyUsedException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn(SignInCommand command)
    {
        try
        {
            var token= await _sender.Send(command);
            return Ok(token);
        }
        catch (InvalidCredentialException e)
        {
            return Unauthorized(e.Message);
        }
    }

    [HttpGet("sign-out")]
    [Authorize]
    public async Task<IActionResult> SignOut()
    {
        var jtiClaim = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);
        if (jtiClaim == null) return BadRequest("Token does not contain a JTI.");
        await _sender.Send(new SignOutCommand(jtiClaim.Value));
        return NoContent();
    }
    
    [HttpGet("sign-out/all-devices")]
    [Authorize]
    public async Task<IActionResult> SignOutAllDevices()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userId == null) return BadRequest("Token does not contain a User ID.");
        await _sender.Send(new SignOutAllDevicesCommand(Guid.Parse(userId.Value)));
        return NoContent();
    }
    
}