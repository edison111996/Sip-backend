namespace BackendPrueba.Models.Roles
{
    // DTO de respuesta simple
    public class RolesResponseDto
    {
        public Guid IdRole { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsSystem { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<PermissionWithModuleDto> Permissions { get; set; } = new();
    }
}
