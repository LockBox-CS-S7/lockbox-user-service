using Microsoft.AspNetCore.Authorization;

namespace lockbox_file_service.Requirement;

class RbacRequirement : IAuthorizationRequirement
{
    public string Permission { get; }
    
    public RbacRequirement(string permission)
    {
        Permission = permission ?? throw new ArgumentNullException(nameof(permission));
    }
}