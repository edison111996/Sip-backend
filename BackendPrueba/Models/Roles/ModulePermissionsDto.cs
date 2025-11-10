using BackendPrueba.Models.PermissionDtos;

namespace BackendPrueba.Models.Roles
{
    // DTO para módulo con sus permisos
    public class ModulePermissionsDto
    {
        public Guid IdModule { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public string? ModuleDescription { get; set; }
        public List<PermissionDto> Permissions { get; set; } = new();
    }
}
