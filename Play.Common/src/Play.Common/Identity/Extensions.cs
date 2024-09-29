using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace Play.Common.Identity;

public static class Extensions
{
    public static AuthenticationBuilder AddJwtBearerAuthentication(this IServiceCollection services) 
    {
        return services
            // Registers the ConfigureJwtBearerOptions class to configure JwtBearerOptions.
            // This ensures that JwtBearerOptions are properly set up according to the configuration provided in the ConfigureJwtBearerOptions class.
            .ConfigureOptions<ConfigureJwtBearerOptions>()

            // Adds authentication services to the ASP.NET Core application.
            // Specifies that JWT Bearer tokens will be used for authentication, using the default authentication scheme "Bearer".
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

            // Configures the JWT Bearer token authentication middleware.
            // This middleware will handle the authentication process for incoming requests that include a JWT Bearer token.
            .AddJwtBearer();
    }
}