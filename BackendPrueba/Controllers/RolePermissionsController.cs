using BackendPrueba.Models.RolesPermissionsDtos;
using BackendPrueba.Services.RolePermissionsService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendPrueba.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolePermissionsController(IRolePermissionService rolePermissionService) : ControllerBase
    {
        // 🔹 Obtener todos los RolePermissions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await rolePermissionService.GetAllAsync();
            return Ok(list);
        }

        // 🔹 Obtener un RolePermission por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await rolePermissionService.GetByIdAsync(id);
            if (result is null) return NotFound("RolePermission not found.");
            return Ok(result);
        }

        // 🔹 Crear un nuevo RolePermission
        [HttpPost]
        public async Task<IActionResult> Create(RolePermissionDto request)
        {
            var created = await rolePermissionService.CreateAsync(request);
            if (created is null) return BadRequest("RolePermission already exists or invalid data.");
            return Ok(created);
        }

        // 🔹 Eliminar un RolePermission
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            bool deleted = await rolePermissionService.DeleteAsync(id);
            if (!deleted) return NotFound("RolePermission not found.");
            return Ok("RolePermission deleted successfully.");
        }
    }
}
