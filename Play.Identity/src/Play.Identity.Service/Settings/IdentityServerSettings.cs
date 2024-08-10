
using System;
using System.Collections.Generic;
using Duende.IdentityServer.Models;

namespace Play.Identity.Service;

public class IdentityServerSettings
{
    public IReadOnlyCollection<ApiScope> ApiScopes { get; init; } = Array.Empty<ApiScope>();
    public IReadOnlyCollection<Client> Clients { get; init; } = Array.Empty<Client>();
}