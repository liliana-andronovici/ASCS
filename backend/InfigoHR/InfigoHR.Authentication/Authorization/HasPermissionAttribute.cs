using InfigoHR.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace InfigoHR.Authentication.Authorization;

public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(Permission permission)
        : base(permission.ToString())
    {
    }
}