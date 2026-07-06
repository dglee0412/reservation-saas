using Microsoft.EntityFrameworkCore;
using Reservation.Application.Abstractions.Repositories;
using Reservation.Domain.Entities;
using Reservation.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reservation.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ReservationDbContext m_db;

        public UserRepository(ReservationDbContext db)
        {
            m_db = db;
        }

        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            m_db.Users.Add(user);
            await m_db.SaveChangesAsync(ct);
        }

        public async Task<User?> FindByEmailAsync(string email, CancellationToken ct = default)
        {
            var normalized = email.Trim().ToLowerInvariant();
            return await m_db.Users.FirstOrDefaultAsync(u => u.Email == normalized, ct);
        }
        
        public async Task<User?> FindByIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await m_db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        }
    }
}
