namespace BackendPrueba.Models.Roles
{
    // DTO de respuesta con permisos agrupados por módulo
    public class RoleDetailResponseDto
    {
        public Guid IdRole { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsSystem { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<ModulePermissionsDto> Modules { get; set; } = new();
    }
}
