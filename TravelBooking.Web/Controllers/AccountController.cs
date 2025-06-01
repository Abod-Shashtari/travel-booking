using MediatR;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Users.CreateUser;
using TravelBooking.Domain.Exceptions;

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
        catch (EmailAlreadyUsed e)
        {
            return BadRequest(e.Message);
        }
    }
}