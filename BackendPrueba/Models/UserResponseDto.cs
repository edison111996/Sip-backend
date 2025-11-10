namespace BackendPrueba.Models
{
    public class UserResponseDto
    {
        public Guid IdUser { get; set; }
        public Guid IdRole { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
