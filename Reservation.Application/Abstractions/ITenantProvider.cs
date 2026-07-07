using System;
using System.Collections.Generic;
using System.Text;

namespace Reservation.Application.Abstractions
{
    public interface ITenantProvider
    {
        //현재 요청의 테넌트 ID를 반환(없으면 null)
        Guid? GetCurrentTenantId();
    }
}
