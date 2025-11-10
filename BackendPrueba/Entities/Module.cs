using System.ComponentModel.DataAnnotations;

namespace BackendPrueba.Entities
{
    public class Module
    {
        [Key]
        public Guid IdModule { get; set; }

        [Required]
        public string Name { get; set; } = null!; // Ej: "Usuarios", "Operadores"

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relación con permisos
        public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    }
}
