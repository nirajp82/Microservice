using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Play.Identity.Service.Entities;
using System.Collections.Generic;
using Play.Identity.Service.Dtos;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Duende.IdentityServer;

namespace Play.Identity.Service.Controller;

[ApiController]
[Route("users")]
//Enforce a specific authorization policy in ASP.NET Core applications that are integrated with Duende IdentityServer.
//It protects resources by requiring that the incoming requests have valid tokens containing the LocalApi scope.
[Authorize(Policy = IdentityServerConstants.LocalApi.PolicyName, Roles = Roles.Admin)]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> Get()
    {
        var users = _userManager.Users
                    .ToList()
                    .Select(u => u.AsDto());

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetByIdAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user.AsDto());
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> PutAsync(Guid id, ApplicationUser appUser)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return NotFound();
        }
        user.Email = appUser.Email;
        user.UserName = appUser.Email;
        user.Gil = appUser.Gil;

        await _userManager.UpdateAsync(user);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return NotFound();
        }
        await _userManager.DeleteAsync(user);
        return NoContent();
    }
}