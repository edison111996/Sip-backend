using System.ComponentModel.DataAnnotations;

namespace BackendPrueba.Entities
{
    public class Role
    {
        [Key]
        public Guid IdRole { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsSystem { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // ✅ Solo relación con permisos
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
