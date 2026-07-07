using Reservation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reservation.Application.Abstractions.Repositories
{
    public interface IUserRepository
    {
        //새 User 추가
        Task AddAsync(User user, CancellationToken ct = default);

        //이메일로 User 찾기(없으면 null) - 로그인, 중복확인에 씀
        Task<User?> FindByEmailAsync(string email, CancellationToken ct = default);

        Task<User?> FindByIdAsync(Guid userId, CancellationToken ct = default);

        Task<List<User>> GetAllAsync(CancellationToken ct = default);
    }
}
