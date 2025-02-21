using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Vardabit.Domain.DTOs;
using Vardabit.Domain.Models;
using Vardabit.Service.Interfaces;

namespace Vardabit.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null) 
                return NotFound("Kullanıcı bulunamadı.");

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            var user = new User
            {
                Name = dto.Name,
                Surname = dto.Surname,
                UserName = dto.UserName,
                PasswordHash = dto.Password
            };

            await _userService.AddAsync(user);

            return StatusCode(201);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            if (id != dto.Id) 
                return BadRequest("ID eşleşmedi.");

            var user = new User
            {
                Id = dto.Id,
                Name = dto.Name,
                Surname = dto.Surname,
                UserName = dto.UserName,
                PasswordHash = dto.Password
            };

            await _userService.UpdateAsync(user);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var token = await _userService.LoginAsync(dto.UserName, dto.Password);

            if (token == null) 
                return Unauthorized("Kullanıcı veya şifre hatalı.");

            return Ok(new { Token = token });
        }
    }
}
