namespace BackendPrueba.Models
{
    public class UpdateUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? Password { get; set; } // opcional

        public Guid RoleId { get; set; }
    }
}
