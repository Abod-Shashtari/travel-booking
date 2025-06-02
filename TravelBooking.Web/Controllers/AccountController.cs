using System.Security.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Users.CreateUser;
using TravelBooking.Application.Users.SignIn;
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
}