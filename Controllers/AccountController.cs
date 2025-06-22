using lockbox_user_service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lockbox_user_service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private IAccountService _accountService;

    public AccountController(ILogger<AccountController> logger, IAccountService accountService)
    {
        _logger = logger;
        _accountService = accountService;
    }

    [HttpPost("delete-account")]
    [Authorize]
    public ActionResult DeleteAccount([FromBody] string accountId)
    {
        _logger.LogInformation("Account deletion requested for user id: {}", accountId);
        try
        {
            _accountService.DeleteUserAccount(accountId);
        }
        catch (Exception e)
        {
            _logger.LogError("An error occured when trying to delete an account: {}", e.Message);
            return BadRequest();
        }
        
        return Ok();
    }
}