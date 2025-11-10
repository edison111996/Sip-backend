using AutoMapper;
using BackendPrueba.Entities;
using BackendPrueba.Models.PermissionDtos;
using BackendPrueba.Models.Roles;

namespace BackendPrueba.Mappings
{
    public class RolesProfile : Profile
    {
             public RolesProfile()
        {
            // Role -> RolesResponseDto
            CreateMap<Role, RolesResponseDto>();

            // Role -> RoleDetailResponseDto
            CreateMap<Role, RoleDetailResponseDto>()
                .ForMember(dest => dest.Modules, opt => opt.Ignore()); // Se maneja manualmente

            // CreateRoleDto -> Role
            CreateMap<CreateRoleDto, Role>()
                .ForMember(dest => dest.IdRole, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsSystem, opt => opt.Ignore())
                .ForMember(dest => dest.RolePermissions, opt => opt.Ignore());

            // Permission -> PermissionDto
            CreateMap<Permission, PermissionDto>()
                .ForMember(dest => dest.IsAssigned, opt => opt.Ignore());
        }

    }
                        
}
