namespace BackendPrueba.Models.Roles
{
    public class PermissionWithModuleDto
    {
        public Guid IdPermission { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid? ModuleId { get; set; }
        public string? ModuleName { get; set; }
    }
}
