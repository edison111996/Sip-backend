using BackendPrueba.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendPrueba.Data
{
    public class VideoGameDbContext(DbContextOptions<VideoGameDbContext> options) : DbContext(options)
    {
        public DbSet<VideoGame> VideoGames => Set<VideoGame>();

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Module> Module { get; set; }

        // 👇 ESTE BLOQUE CONFIGURA LAS RELACIONES
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔹 Clave compuesta para la tabla intermedia RolePermission
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => rp.IdRolePermission); 

            // 🔹 Relación Role ↔ RolePermission
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔹 Relación Permission ↔ RolePermission
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔹 Relación Module ↔ Permission
            modelBuilder.Entity<Permission>()
                .HasOne(p => p.Module)
                .WithMany(m => m.Permissions)
                .HasForeignKey(p => p.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        }


    }
}
