using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Reservation.Domain.Entities
{
    public class User : TenantEntity
    {
        [MaxLength(256)]
        public string Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;
        
        // EF Core 전용(DB에서 객체를 복원할 때 사용)
        private User() { }
        //실제 코드에서 새 유저를 만들 떄 쓰는 생성자
        public User(Guid p_TenantId, string p_Email, string p_PasswordHash)
        {
            if (p_TenantId == Guid.Empty)
                throw new ArgumentException("TenantId는 필수입니다.", nameof(p_TenantId));
            if (string.IsNullOrWhiteSpace(p_Email))
                throw new ArgumentException("이메일은 필수입니다.", nameof(p_Email));
            if (string.IsNullOrWhiteSpace(p_PasswordHash))
                throw new ArgumentException("비밀번호는 필수입니다.", nameof(p_PasswordHash));

            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            TenantId = p_TenantId;
            Email = p_Email.Trim().ToLowerInvariant();
            PasswordHash = p_PasswordHash;
        }
    
    }
}
