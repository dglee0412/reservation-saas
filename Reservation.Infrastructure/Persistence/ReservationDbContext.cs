using Microsoft.EntityFrameworkCore;
using Reservation.Application.Abstractions;
using Reservation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reservation.Infrastructure.Persistence
{
    public class ReservationDbContext : DbContext
    {
        private readonly ITenantProvider m_TenantProvider;
        public ReservationDbContext(DbContextOptions<ReservationDbContext> options,
            ITenantProvider tenantProvider) : base(options)
        {
            m_TenantProvider = tenantProvider;
        }

        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //멀티테넌시: 테넌트 소속 엔티티에 자동필터
            modelBuilder.Entity<User>().HasQueryFilter(u => u.TenantId == m_TenantProvider.GetCurrentTenantId());

            //UserL 이메일 전역 유니크(방식1 - 이메일 전역 유일 보장)
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            //// Tenant: xmin을 동시성 토큰으로 사용
            //modelBuilder.Entity<Tenant>().UseXminAsConcurrencyToken();

            //// User: xmin을 동시성 토큰으로 사용
            //modelBuilder.Entity<User>().UseXminAsConcurrencyToken();
        }
    }
}
