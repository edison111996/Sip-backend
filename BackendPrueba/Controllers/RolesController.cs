using BackendPrueba.Models.Roles;
using BackendPrueba.Services.Roles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendPrueba.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController(IRoleService roleService) : ControllerBase
    {
        // GET: api/roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolesResponseDto>>> GetAll()
        {
            var roles = await roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        // GET: api/roles/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDetailResponseDto>> GetById(Guid id)
        {
            var role = await roleService.GetRoleByIdAsync(id);
            if (role is null) return NotFound();
            return Ok(role);
        }

        // GET: api/roles/modules-permissions?roleId={roleId}
        [HttpGet("modules-permissions")]
        public async Task<ActionResult<List<ModulePermissionsDto>>> GetModulesWithPermissions([FromQuery] Guid? roleId = null)
        {
            var modules = await roleService.GetAllModulesWithPermissionsAsync(roleId);
            return Ok(modules);
        }

        // POST: api/roles
        [HttpPost]
        public async Task<ActionResult<RolesResponseDto>> Create([FromBody] CreateRoleDto request)
        {
            var role = await roleService.CreateRoleAsync(request);
            if (role is null) return BadRequest("Role name already exists or invalid permissions");

            return CreatedAtAction(nameof(GetById), new { id = role.IdRole }, role);
        }

        // PUT: api/roles/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<RolesResponseDto>> Update(Guid id, [FromBody] UpdateRoleDto request)
        {
            var role = await roleService.UpdateRoleAsync(id, request);
            if (role is null) return BadRequest("Role not found, name already exists, or invalid permissions");

            return Ok(role);
        }

        // DELETE: api/roles/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await roleService.DeleteRoleAsync(id);
            if (!result) return NotFound("Role not found or cannot delete system roles");

            return NoContent();
        }
    }
}
