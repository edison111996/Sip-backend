using BackendPrueba.Models;
using BackendPrueba.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendPrueba.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await userService.GetByIdAsync(id);
            if (user is null) return NotFound();
            return Ok(new
            {
                user.IdUser,
                user.Username,
                user.Role // o user.Roles si es una lista
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserDto request)
        {
            try
            {
                var user = await userService.CreateAsync(request);

                if (user is null)
                    return BadRequest("El nombre de usuario ya existe.");

                return Ok(user);
            }
            catch (Exception ex)
            {
                // Maneja el error si el rol no existe o si ocurre un problema en la BD
                return BadRequest(new
                {
                    message = ex.Message,
                    error = "No se pudo crear el usuario. Verifica el RoleId o los datos enviados."
                });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateUserDto request)
        {
            var user = await userService.UpdateAsync(id, request);
            if (user is null) return NotFound();
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await userService.DeleteAsync(id);
            if (!result) return NotFound();
            return Ok("User deleted successfully");
        }


    }
}
