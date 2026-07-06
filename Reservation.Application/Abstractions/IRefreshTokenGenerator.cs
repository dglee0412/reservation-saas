using System;
using System.Collections.Generic;
using System.Text;

namespace Reservation.Application.Abstractions
{
    public interface IRefreshTokenGenerator
    {
        //추측 불가능한 랜덤 토큰 문자열을 만든다.
        string Generate();
    }
}
