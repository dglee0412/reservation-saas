using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Reservation.Domain.Entities
{
    public class Tenant : BaseEntity
    {
        [MaxLength(200)]
        public string Name { get; private set; } = null!;
        public ICollection<User> Users { get; private set; } = new List<User>();
        
        // EF Core 전용(DB에서 객체를 복원할 때 사용)
        private Tenant() { }

        //실제 코드에서 새 테넌트를 만들 떄 쓰는 생성자
        public Tenant(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("테넌트 이름은 필수입니다.", nameof(name));

            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            Name = name.Trim();
        }
        
        //이름 변경도 메서드로만 (규칙을 통과해야 바뀜)
        public void Rename(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("테넌트 이름은 필수입니다.", nameof(newName));
            Name = newName.Trim();
        }
    }
}
