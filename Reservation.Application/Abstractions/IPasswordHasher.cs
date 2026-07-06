using System;
using System.Collections.Generic;
using System.Text;

namespace Reservation.Application.Abstractions
{
    public interface IPasswordHasher
    {
        // 평문 비밀번호 -> 해시 문자열
        string Hash(string password);

        // 입력한 평문이 저장된 해시와 일치하는지
        bool Verify(string password, string passwordHash);
    }
}
