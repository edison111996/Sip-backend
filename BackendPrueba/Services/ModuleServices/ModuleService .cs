using AutoMapper;
using AutoMapper.QueryableExtensions;
using BackendPrueba.Data;
using BackendPrueba.Entities;
using BackendPrueba.Models.Modules;
using Microsoft.EntityFrameworkCore;

namespace BackendPrueba.Services.ModuleServices
{
    public class ModuleService(VideoGameDbContext context, IMapper mapper) : IModuleService
    {
        public async Task<IEnumerable<ModuleDto>> GetAllAsync()
        {
            return await context.Module
                .ProjectTo<ModuleDto>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<ModuleDto?> GetByIdAsync(Guid id)
        {
            var module = await context.Module.FindAsync(id);
            if (module == null) return null;

            return mapper.Map<ModuleDto>(module);
        }

        public async Task<ModuleDto> CreateAsync(CreateModuleDto dto)
        {
            var module = mapper.Map<Module>(dto);

            module.IdModule = Guid.NewGuid();
            module.CreatedAt = DateTime.UtcNow;

            context.Module.Add(module);
            await context.SaveChangesAsync();

            return mapper.Map<ModuleDto>(module);
        }

        public async Task<ModuleDto?> UpdateAsync(Guid id, UpdateModuleDto dto)
        {
            var module = await context.Module.FindAsync(id);
            if (module == null) return null;
            mapper.Map(dto, module);

            await context.SaveChangesAsync();

            return mapper.Map<ModuleDto>(module);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var module = await context.Module.FindAsync(id);
            if (module == null) return false;

            context.Module.Remove(module);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
