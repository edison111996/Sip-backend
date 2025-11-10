using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendPrueba.Entities
{
    public class User
    {
        [Key]
        public Guid IdUser { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        // ✅ Un usuario = 1 rol
        [ForeignKey(nameof(RoleNavigation))]
        public Guid RoleId { get; set; }

        public virtual Role RoleNavigation { get; set; } = null!;

        // Campos adicionales útiles
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

    }
}
