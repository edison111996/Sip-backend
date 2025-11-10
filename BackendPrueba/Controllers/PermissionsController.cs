using BackendPrueba.Models.PermissionDtos;
using BackendPrueba.Services.PermissionService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendPrueba.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController(IPermissionService permissionService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var permissions = await permissionService.GetAllPermissionsAsync();
            return Ok(permissions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var permission = await permissionService.GetPermissionByIdAsync(id);
            if (permission is null) return NotFound();
            return Ok(permission);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PermissionDto request)
        {
            var created = await permissionService.CreatePermissionAsync(request);
            if (created is null) return BadRequest("Permission already exists.");
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, PermissionDto request)
        {
            var updated = await permissionService.UpdatePermissionAsync(id, request);
            if (updated is null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await permissionService.DeletePermissionAsync(id);
            if (!deleted) return NotFound();
            return Ok("Permission deleted successfully.");
        }
    }
}

