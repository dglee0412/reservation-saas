using Microsoft.EntityFrameworkCore;
using Reservation.Application.Abstractions.Repositories;
using Reservation.Domain.Entities;
using Reservation.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reservation.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ReservationDbContext m_Db;

        public RefreshTokenRepository(ReservationDbContext db)
        {
            m_Db = db;
        }

        public async Task AddAsync(RefreshToken token, CancellationToken ct = default)
        {
            m_Db.RefreshTokens.Add(token);
            await m_Db.SaveChangesAsync(ct);
        }

        public async Task<RefreshToken?> FindByTokenAsync(string token, CancellationToken ct = default)
        {
            return await m_Db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await m_Db.SaveChangesAsync(ct);
        }

        public async Task RevokeAllByUserIdAsync(Guid userId, CancellationToken ct = default) 
        { 
            var tokens = await m_Db.RefreshTokens.Where(t => t.UserId == userId && !t.IsRevoked).ToListAsync(ct);

            foreach (var token in tokens)
            {
                token.Revoke();
            }

            await m_Db.SaveChangesAsync(ct);
        }
    }
}
