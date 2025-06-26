using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Users.CreateUser;
using TravelBooking.Application.Users.SignIn;
using TravelBooking.Application.Users.SignOut;
using TravelBooking.Web.Extensions;
using TravelBooking.Web.Requests.Users;

namespace TravelBooking.Web.Controllers;

[Route("api/accounts")]
[ApiController]
public class AccountController:ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    public AccountController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp(CreateUserRequest request)
    {
        var command = _mapper.Map<CreateUserRequest, CreateUserCommand>(request);
        var result = await _sender.Send(command);
        return result.Match(data => Ok(data), this.HandleFailure);
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn(SignInRequest request)
    {
        var command = _mapper.Map<SignInRequest, SignInCommand>(request);
        var result = await _sender.Send(command);
        return result.Match(data => Ok(data), this.HandleFailure);
    }

    [HttpGet("sign-out")]
    [Authorize]
    public async Task<IActionResult> SignOutUser()
    {
        var jtiClaim = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);
        var result=await _sender.Send(new SignOutCommand(jtiClaim!.Value));
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