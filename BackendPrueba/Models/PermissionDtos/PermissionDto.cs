namespace BackendPrueba.Models.PermissionDtos
{
    public class PermissionDto
    {
        public Guid IdPermission { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public Guid ModuleId { get; set; }

        public bool IsAssigned { get; set; }
    }
}
