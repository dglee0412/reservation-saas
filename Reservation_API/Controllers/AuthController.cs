
using Microsoft.AspNetCore.Mvc;
using Reservation.Application.Abstractions;
using Reservation_API.DTOs.Auth;

namespace Reservation_API.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService m_AuthService;

        public AuthController(IAuthService authService)
        {
            m_AuthService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserRequest request, CancellationToken ct)
        {
            try
            {
                var userId = await m_AuthService.RegisterAsync(request.TenantId, request.Email, request.Password, ct);

                return Ok(new { userId });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserRequest request, CancellationToken ct)
        {
            try
            {
                var authResult = await m_AuthService.LoginAsync(request.Email, request.Password, ct);

                //Refresh Token -> HttpOnyl 쿠키
                Response.Cookies.Append("refreshToken", authResult.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(14) // Refresh Token 만료 시간 설정
                });

                //Access Token -> 응답 Body
                return Ok(new LoginResponse(authResult.AccessToken));
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(CancellationToken ct)
        {
            //1.쿠키에서 Refresh Token 가져오기
            var refreshToken = Request.Cookies["refreshToken"];
            if(string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new { message = "Refresh Token이 없습니다." });

            try
            {
                //2. 갱신(회전 + 재사용 탐지)
                var result = await m_AuthService.RefreshAsync(refreshToken, ct);

                //3. 새 Refresh Token -> 새 HttpOnly 쿠키(기존 것 덮어씀)
                Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(14) // Refresh Token 만료 시간 설정
                });

                //4. 새 Access Token -> Body
                return Ok(new LoginResponse(result.AccessToken));
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(new { messsage = ex.Message });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if(!string.IsNullOrEmpty(refreshToken))
            {
                await m_AuthService.LogoutAsync(refreshToken, ct);
            }

            //쿠키 삭제
            Response.Cookies.Delete("refreshToken");

            return Ok(new { message = "로그아웃 되었습니다." });
        }
    }
}
