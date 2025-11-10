namespace BackendPrueba.Models.Roles
{

    // DTO para actualizar rol
    public class UpdateRoleDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<Guid> PermissionIds { get; set; } = new(); // IDs de permisos a asignar
    }
}
