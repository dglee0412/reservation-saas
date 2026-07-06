using Reservation.Application.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reservation.Application.Abstractions
{
    public interface IAuthService
    {
        //회원가입: 성공하면 생성된 UserId를 반환, 실패하면 null 반환
        Task<Guid> RegisterAsync(Guid tenantId, string email, string password, CancellationToken ct = default);

        //로그인: 성공하면 Access + Refresh 토큰 반환
        Task<AuthResult> LoginAsync(string email, string password, CancellationToken ct = default);

        Task<AuthResult> RefreshAsync(string refreshToken, CancellationToken ct = default);
        Task LogoutAsync(string refreshToken, CancellationToken ct = default);
    }
}
