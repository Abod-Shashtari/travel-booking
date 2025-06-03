using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Users.CreateUser;
using TravelBooking.Application.Users.SignIn;
using TravelBooking.Application.Users.SignOut;
using TravelBooking.Web.Extensions;

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
        var result = await _sender.Send(command);
        return result.Match(data => Ok(data), this.HandleFailure);
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn(SignInCommand command)
    {
        var result = await _sender.Send(command);
        return result.Match(data => Ok(data), this.HandleFailure);
    }

    [HttpGet("sign-out")]
    [Authorize]
    public async Task<IActionResult> SignOutUser()
    {
        var jtiClaim = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);
        if (jtiClaim == null) return BadRequest("Token does not contain a JTI.");
        var result=await _sender.Send(new SignOutCommand(jtiClaim.Value));
        return result.Match(NoContent, this.HandleFailure);
    }
    
    [HttpGet("sign-out/all-devices")]
    [Authorize]
    public async Task<IActionResult> SignOutAllDevices()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userId == null) return BadRequest("Token does not contain a User ID.");
        var result=await _sender.Send(new SignOutAllDevicesCommand(Guid.Parse(userId.Value)));
        return result.Match(NoContent, this.HandleFailure);
    }
    
}