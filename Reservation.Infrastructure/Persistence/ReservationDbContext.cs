using Microsoft.EntityFrameworkCore;
using Reservation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reservation.Infrastructure.Persistence
{
    public class ReservationDbContext : DbContext
    {
        public ReservationDbContext(DbContextOptions<ReservationDbContext> options) : base(options)
        {

        }

        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //// Tenant: xmin을 동시성 토큰으로 사용
            //modelBuilder.Entity<Tenant>().UseXminAsConcurrencyToken();

            //// User: xmin을 동시성 토큰으로 사용
            //modelBuilder.Entity<User>().UseXminAsConcurrencyToken();
        }
    }
}
