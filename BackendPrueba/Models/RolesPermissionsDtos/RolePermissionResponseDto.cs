namespace BackendPrueba.Models.RolesPermissionsDtos
{
    public class RolePermissionResponseDto
    {
        public Guid IdRolePermission { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public Guid PermissionId { get; set; }
        public string PermissionName { get; set; } = string.Empty;

    }
}
