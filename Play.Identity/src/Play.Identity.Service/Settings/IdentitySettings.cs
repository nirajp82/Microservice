
using System;
using System.Collections.Generic;
using Duende.IdentityServer.Models;

namespace Play.Identity.Service;

public class IdentitySettings
{
    public string AdminUserEmail { get; init; }

    /*  Note:
     *  During the development password was stored in secrets (PlainText) file using following two commands.
     *  Path: C:\Users\Edu4K\AppData\Roaming\Microsoft\UserSecrets\55bfb04b-b4ca-4016-a143-a2b73aef81d6\secrets.json
        >> dotnet user-secrets init
        >> dotnet user-secrets set "IdentitySettings:AdminUserPassword" "{{YOUR_PASSWORD_VALUE}}"
     */
    public string AdminUserPassword { get; init; }

    public decimal StartingGil { get; init; }
}