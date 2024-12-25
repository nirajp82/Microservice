using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Play.Trading.Service.Contracts;
using Play.Trading.Service.Dtos;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Play.Trading.Service.Controllers;

[ApiController]
[Route("Purchase")]
[Authorize]
public class PurchaseController : ControllerBase
{
    public readonly IPublishEndpoint publishEndpoint;

    public PurchaseController(IPublishEndpoint publishEndpoint)
    {
        this.publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync(SubmitPurchaseDto purchase)
    {
        var userId = User.FindFirstValue("sub");
        var correlationId = Guid.NewGuid();

        var message = new PurchaseRequested(
            Guid.Parse(userId),
            purchase.ItemId.Value,
            purchase.Quantity,
            correlationId
        );

        await publishEndpoint.Publish(message);

        return Accepted();
    }
}
