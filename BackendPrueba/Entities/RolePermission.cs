using System.ComponentModel.DataAnnotations;

namespace BackendPrueba.Entities
{
    public class RolePermission
    {
        [Key]
        public Guid IdRolePermission { get; set; }
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual Permission Permission { get; set; } = null!;
    }
}
