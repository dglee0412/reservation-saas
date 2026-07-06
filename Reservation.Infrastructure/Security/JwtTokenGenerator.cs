using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Reservation.Application.Abstractions;
using Reservation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Reservation.Infrastructure.Security
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration m_Config;

        public JwtTokenGenerator(IConfiguration config)
        {
            m_Config = config;
        }

        public string GenerateAccessToken(User user)
        {
            // 1. 설정값 읽기(secrets.json의 Jwt 섹션)
            var secretKey = m_Config["Jwt:SecretKey"]!;
            var issuer = m_Config["Jwt:Issuer"];
            
            // 2. 서명 자격증명 만들기(비밀키 + HS256)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //3. 토큰에 담을 정보(claims)
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim("tenantId", user.TenantId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            //4. 토큰 조립(헤더 + 페이로드 + 만료 + 서명)
            var token = new JwtSecurityToken(
                issuer: issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials
            );

            //5. 문자열로 직렬화해서 반환
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
