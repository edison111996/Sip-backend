using AutoMapper;
using BackendPrueba.Entities;
using BackendPrueba.Models.Modules;

namespace BackendPrueba.Mappings
{
    public class ModuleProfile : Profile
    {
        public ModuleProfile()
        {
            CreateMap<Module, ModuleDto>();

            // ============================================
            // CreateModuleDto -> Entity (para POST)
            // ============================================
            // Name es requerido en CreateModuleDto (string no nullable)
            // Description es opcional (string? nullable)
            CreateMap<CreateModuleDto, Module>()
                .ForMember(dest => dest.IdModule, opt => opt.Ignore())     // Se genera en el servicio
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());   // Se asigna en el servicio

            // ============================================
            // UpdateModuleDto -> Entity (para PUT)
            // ============================================
            // Ambas propiedades son opcionales (nullable)
            // Solo mapea si el valor NO es null
            CreateMap<UpdateModuleDto, Module>()
                .ForMember(dest => dest.IdModule, opt => opt.Ignore())     // No se actualiza
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())    // No se actualiza
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        }
    }
}
