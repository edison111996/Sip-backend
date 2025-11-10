using BackendPrueba.Models.Modules;
using BackendPrueba.Services.ModuleServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendPrueba.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesController(IModuleService moduleService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var modules = await moduleService.GetAllAsync();
            return Ok(modules);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var module = await moduleService.GetByIdAsync(id);
            if (module == null) return NotFound();
            return Ok(module);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateModuleDto dto)
        {
            var module = await moduleService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = module.IdModule }, module);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateModuleDto dto)
        {
            var updated = await moduleService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await moduleService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
