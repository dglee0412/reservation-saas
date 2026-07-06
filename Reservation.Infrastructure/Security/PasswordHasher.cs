using Microsoft.AspNetCore.Identity;
using Reservation.Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reservation.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasher<object> m_Hasher = new();
        private static readonly Object s_Dummy = new();

        public string Hash(string password)
        {
            return m_Hasher.HashPassword(s_Dummy, password);
        }

        public bool Verify(string password, string passwordHash) 
        {
            var result = m_Hasher.VerifyHashedPassword(s_Dummy, passwordHash, password);
            return result != PasswordVerificationResult.Failed;
        }
    }
}
