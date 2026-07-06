using Reservation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reservation.Application.Abstractions.Repositories
{
    public interface IRefreshTokenRepository
    {
        //RefreshToken 저장
        Task AddAsync(RefreshToken token, CancellationToken ct = default);

        //토큰 문자열로 찾기(갱신, 로그아웃 시)
        Task<RefreshToken?> FindByTokenAsync(string token, CancellationToken ct = default);

        //변경사항 저장(Revoke 후 등)
        Task SaveChangesAsync(CancellationToken ct = default);

        //재사용 탐지시: 특정 유저의 모든(유효한) 토큰 무효화
        Task RevokeAllByUserIdAsync(Guid userID, CancellationToken ct = default);
    }
}
