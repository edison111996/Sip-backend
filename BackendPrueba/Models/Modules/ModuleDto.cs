namespace BackendPrueba.Models.Modules
{
    public class ModuleDto
    {
        public Guid IdModule { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
