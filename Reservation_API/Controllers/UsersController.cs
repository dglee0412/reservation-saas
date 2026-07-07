using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reservation.Application.Abstractions.Repositories;
using Reservation_API.DTOs;

namespace Reservation_API.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository m_UserRepository;

        public UsersController(IUserRepository userRepository)
        {
            m_UserRepository = userRepository;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var users = await m_UserRepository.GetAllAsync(ct);
            var response = users.Select(u => new UserResponse(u.Id, u.Email, u.TenantId)).ToList();

            return Ok(response);
        }
    }
}
