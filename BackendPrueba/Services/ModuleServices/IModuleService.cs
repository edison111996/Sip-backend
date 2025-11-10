using BackendPrueba.Models.Modules;

namespace BackendPrueba.Services.ModuleServices
{
    public interface IModuleService
    {
        Task<IEnumerable<ModuleDto>> GetAllAsync();
        Task<ModuleDto?> GetByIdAsync(Guid id);
        Task<ModuleDto> CreateAsync(CreateModuleDto dto);
        Task<ModuleDto?> UpdateAsync(Guid id, UpdateModuleDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
