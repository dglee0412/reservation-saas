using Reservation.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reservation.Infrastructure.Tenancy
{
    public class TenantProvider : ITenantProvider
    {
        private readonly IHttpContextAccessor m_httpContextAccessor;

        public TenantProvider(IHttpContextAccessor httpContextAccessor)
        {
            m_httpContextAccessor = httpContextAccessor;
        }

        public Guid? GetCurrentTenantId()
        {
            //현재 요청의 사용자(JWT claims)
            var user = m_httpContextAccessor.HttpContext?.User;
            if (user is null)
                return null;

            //tenantId claim 가져오기
            var tenantIdClaim = user.FindFirst("tenantId");
            if (tenantIdClaim is null)
                return null;

            //문자열 -> Guid 변환
            if(Guid.TryParse(tenantIdClaim.Value, out var tenantId))
            {
                return tenantId;
            }

            return null;
        }
    }
}
