using lockbox_user_service.Models;
using lockbox_user_service.Services;
using Microsoft.AspNetCore.Mvc;

namespace lockbox_user_service.Controllers;

[ApiController]
[Route("/api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("admin")]
    public async Task<ActionResult<string?>> GetUserById(string id)
    {
        return await _userService.GetUserById(id);
    }

    [HttpPost("admin")]
    public async Task<ActionResult<bool>> DeleteUserById(string id)
    {
        return await _userService.DeleteUserById(id);
    }
}