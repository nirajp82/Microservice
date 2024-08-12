using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Play.Common.Settings;

namespace Play.Common.Identity;

public static class Extensions
{
    public static AuthenticationBuilder AddJwtBearerAuthentication(this IServiceCollection services) 
    {
        return services.ConfigureOptions<ConfigureJwtBearerOptions>()
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();
    }
}