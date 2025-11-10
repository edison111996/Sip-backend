namespace BackendPrueba.Models.Roles
{
    // DTO para crear rol
    public class CreateRoleDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<Guid> PermissionIds { get; set; } = new(); // IDs de permisos a asignar
    }
}
