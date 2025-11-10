namespace BackendPrueba.Models.PermissionDtos
{
    public class PermissionResponseDto
    {
        public Guid IdPermission { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public Guid ModuleId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
