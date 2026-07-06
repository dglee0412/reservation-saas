using Reservation.Application.Abstractions;
using Reservation.Application.Abstractions.Repositories;
using Reservation.Application.Results;
using Reservation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reservation.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository m_userRepository;
        private readonly IPasswordHasher m_PasswordHasher;
        private readonly IJwtTokenGenerator m_JwtTokenGenerator;
        private readonly IRefreshTokenGenerator m_RefreshTokenGenerator;
        private readonly IRefreshTokenRepository m_RefreshTokenRepository;

        public AuthService(
            IUserRepository userRepository, 
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            IRefreshTokenGenerator refreshTokenGenerator,
            IRefreshTokenRepository refreshTokenRepository)
        {
            m_userRepository = userRepository;
            m_PasswordHasher = passwordHasher;
            m_JwtTokenGenerator = jwtTokenGenerator;
            m_RefreshTokenGenerator = refreshTokenGenerator;
            m_RefreshTokenRepository = refreshTokenRepository;
        }

        public async Task<Guid> RegisterAsync(Guid tenantId, string email, string password, CancellationToken ct = default)
        {
            //1. 이미 가입된 이메일인지 확인
            var existingUser = await m_userRepository.FindByEmailAsync(email, ct);
            if (existingUser is not null)
            {
                throw new InvalidOperationException("이미 존재하는 이메일입니다.");
            }

            //2. 비밀번호 해싱
            var passwordHash = m_PasswordHasher.Hash(password);
            //3. User 엔티티 생성(해시된 비번으로)
            var newUser = new User(tenantId, email, passwordHash);
            //4. DB에 저장
            await m_userRepository.AddAsync(newUser, ct);

            //성공하면 생성된 UserId 반환
            return newUser.Id; 
        }

        public async Task<AuthResult> LoginAsync(string email, string password, CancellationToken ct = default)
        {
            //1. 이메일로 User 찾기
            var user = await m_userRepository.FindByEmailAsync(email, ct);
            if (user is null)
            {
                throw new InvalidOperationException("이메일 또는 비밀번호가 올바르지 않습니다.");
            }
            //2. 비밀번호 검증
            var isPasswordValid = m_PasswordHasher.Verify(password, user.PasswordHash);
            if (!isPasswordValid)
            {
                throw new InvalidOperationException("이메일 또는 비밀번호가 올바르지 않습니다.");
            }
            //3. Access Token 생성
            var accessToken = m_JwtTokenGenerator.GenerateAccessToken(user);
            //4. Refresh Token 생성
            var refreshTokenValue = m_RefreshTokenGenerator.Generate();
            //5. Refresh Token DB에 저장
            var refreshToken = new RefreshToken(user.Id, refreshTokenValue, DateTime.UtcNow.AddDays(14));
            await m_RefreshTokenRepository.AddAsync(refreshToken, ct);

            //6. AuthResult 반환
            return new AuthResult(accessToken, refreshToken.Token);
        }

        public async Task<AuthResult> RefreshAsync(string refreshToken, CancellationToken ct = default)
        {
            //1. DB에서 Refresh Token 조회
            var stored = await m_RefreshTokenRepository.FindByTokenAsync(refreshToken, ct);
            if (stored is null)
                throw new InvalidOperationException("유효하지 않은 토큰입니다.");

            //2. 재사용탐지: 이미 폐기된 토큰이 다시 왔다 = 탈취 의심
            if(stored.IsRevoked)
            {
                //해당 유저의 모든 토큰 폐기(공격자, 정상유저 다 끊고 재로그인 유도
                await m_RefreshTokenRepository.RevokeAllByUserIdAsync(stored.UserId, ct);
                throw new InvalidOperationException("토큰이 재사용되었습니다. 다시 로그인 해주세요.");
            }

            //3. 만료검사
            if(!stored.IsActive())
            {
                throw new InvalidOperationException("만료된 토큰입니다. 다시 로그인 해주세요.");
            }

            //4. 유저 찾기(새 Access 발급에 필요)
            var user = await m_userRepository.FindByIdAsync(stored.UserId, ct);
            if (user is null)
                throw new InvalidOperationException("사용자를 찾을 수 없습니다.");

            //5. 회전: 옛 Refresh 폐기
            stored.Revoke();

            //6. 새 Access + 새 Refresh 발급
            var newAccessToken = m_JwtTokenGenerator.GenerateAccessToken(user);
            var newRefreshValue = m_RefreshTokenGenerator.Generate();
            var newRefreshToken = new RefreshToken(user.Id, newRefreshValue, DateTime.UtcNow.AddDays(14));

            //7. 새 Refresh 저장 + 옛 것 폐기를 함께 반영
            await m_RefreshTokenRepository.AddAsync(newRefreshToken, ct);
            await m_RefreshTokenRepository.SaveChangesAsync(ct);

            //8. 반환
            return new AuthResult(newAccessToken, newRefreshToken.Token);
        }

        public async Task LogoutAsync(string refreshToken, CancellationToken ct = default)
        {
            //1. DB에서 Refresh Token 조회
            var stored = await m_RefreshTokenRepository.FindByTokenAsync(refreshToken, ct);
            if (stored is null && !stored.IsRevoked)
            {
                //throw new InvalidOperationException("유효하지 않은 토큰입니다.");
                stored.Revoke();
                await m_RefreshTokenRepository.SaveChangesAsync(ct);
            }
        }
    }
}
