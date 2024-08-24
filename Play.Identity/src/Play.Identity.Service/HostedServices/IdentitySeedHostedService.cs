using Automatonymous;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Play.Identity.Service.Entities;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Duende.IdentityServer.Telemetry.Metrics;
using static MassTransit.MessageHeaders;
using static System.Net.Mime.MediaTypeNames;
using System.Timers;
using System;

namespace Play.Identity.Service.HostedServices;

/// <summary>
/// A IdentitySeedHostedService service responsible for seeding roles and an initial admin user into the identity system.
/// 
/// In .NET, a hosted service is a background service that runs alongside the main application. 
/// 
/// It's designed to perform long-running operations or background tasks that need to be executed independently of the web request 
/// pipeline. Hosted services are part of the ASP.NET Core framework and are managed by the IHostedService interface.
/// 
/// When the application starts, the .NET runtime creates and starts all registered hosted services.
/// This involves calling the StartAsync method of each IHostedService implementation.
/// 
/// Run:
///     Once started, the hosted service runs independently of the main application.It can perform background tasks, 
///     such as processing queues, handling scheduled jobs, or monitoring system events.
///     For periodic or recurring tasks, the service may use timers or scheduled tasks to perform work at specified intervals.
/// </summary>
public class IdentitySeedHostedService : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IdentitySettings _settings;

    public IdentitySeedHostedService(IServiceScopeFactory serviceScopeFactory,
        IOptions<IdentitySettings> settings)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _settings = settings.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await CreateRoleIfNotExistsAsync(roleManager, Roles.Admin);
        await CreateRoleIfNotExistsAsync(roleManager, Roles.Player);

        await CreateUserIfNotExistsAsync(userManager);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static async Task CreateRoleIfNotExistsAsync(RoleManager<ApplicationRole> appRoleManager, string role)
    {
        var roleExists = await appRoleManager.RoleExistsAsync(role);
        if (roleExists)
            return;

        await appRoleManager.CreateAsync(new ApplicationRole { Name = role });
    }

    private async Task CreateUserIfNotExistsAsync(UserManager<ApplicationUser> userManager)
    {
        var adminUser = await userManager.FindByEmailAsync(_settings.AdminUserEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = _settings.AdminUserEmail,
                Email = _settings.AdminUserEmail,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            await userManager.CreateAsync(adminUser, _settings.AdminUserPassword);
            await userManager.AddToRoleAsync(adminUser, Roles.Admin);
        }
    }
}
