
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Play.Common.Settings;

namespace Play.Common.Identity;

// This class is responsible for configuring the JwtBearerOptions used by the ASP.NET Core middleware
// to handle JWT (JSON Web Token) authentication. 
//IConfigureNamedOptions<JwtBearerOptions>: This interface is used to configure options that have a specific name.
//                  In this case, it’s used to configure JwtBearerOptions, which are used for JWT authentication.
public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly IConfiguration _configuration;

    public ConfigureJwtBearerOptions(IConfiguration configuration)
    {
        this._configuration = configuration;
    }

    // Configures JwtBearerOptions based on the given name. This method is invoked when configuring named options.
    // It ensures that only the JwtBearerOptions with the default authentication scheme (i.e., "Bearer") are configured.
    public void Configure(string? name, JwtBearerOptions jwtBearerOptions)
    {
        // Only configure if the provided name matches the default authentication scheme name.
        if (name != JwtBearerDefaults.AuthenticationScheme)
            return;

        var serviceSettings = _configuration.GetSection(nameof(ServiceSettings))
                                            .Get<ServiceSettings>()!;

        // Sets the Authority property for JwtBearerOptions. This is the URL of the token issuer or authorization server.
        jwtBearerOptions.Authority = serviceSettings.Authority;

        // Sets the Audience property for JwtBearerOptions. This represents the intended recipient of the token.
        jwtBearerOptions.Audience = serviceSettings.ServiceName;

        // Disables automatic mapping of inbound claims from the token to the claims in the application's identity.
        // This can be useful to prevent unexpected claims mapping or to handle claims in a custom way.
        jwtBearerOptions.MapInboundClaims = false;

        // Configures token validation parameters, which define how tokens are validated.
        jwtBearerOptions.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            // Specifies the claim type used for the user's name. The default is usually "sub" (subject), but here it is overridden.
            NameClaimType = "name",

            // Specifies the claim type used for user roles. This is useful for role-based authorization.
            RoleClaimType = "role"
        };
    }

    // Overloaded Configure method that calls the named version of the Configure method with default options name.
    // This provides a default configuration setup when no specific name is provided.
    public void Configure(JwtBearerOptions jwtBearerOptions)
    {
        Configure(Options.DefaultName, jwtBearerOptions);
    }
}
