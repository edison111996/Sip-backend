using System.ComponentModel.DataAnnotations;

namespace BackendPrueba.Entities
{
    public class Permission
    {
        [Key]
        public Guid IdPermission { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public Guid ModuleId { get; set; }
        public virtual Module Module { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
