using System;
using System.Collections.Generic;
using System.Text;

namespace Reservation.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string Token { get; private set; } = string.Empty;
        public DateTime ExpiresAt { get; private set; }
        public bool IsRevoked { get; private set; }

        //EF Core 전용
        private RefreshToken() { }

        public RefreshToken(Guid p_UserId, string p_Token, DateTime p_ExpiresAt)
        {
            if(p_UserId == Guid.Empty)
                throw new ArgumentException("UserId는 필수압니다", nameof(p_UserId));
            if(string.IsNullOrWhiteSpace(p_Token))
                throw new ArgumentException("Token은 필수압니다", nameof(p_Token));

            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            UserId = p_UserId;
            Token = p_Token;
            ExpiresAt = p_ExpiresAt;
            IsRevoked = false;
        }

        public void Revoke()
        {
            IsRevoked = true;
        }

        //현재 유효한가?(폐기 안 됬고 + 만료 안 됬고)
        public bool IsActive()
        {
            return !IsRevoked && DateTime.UtcNow < ExpiresAt;
        }
    }
}
