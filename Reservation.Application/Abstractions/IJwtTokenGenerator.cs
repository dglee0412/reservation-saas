using Reservation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reservation.Application.Abstractions
{
    public interface IJwtTokenGenerator
    {
        //User 정보를 받아 서명된 Access Token(Jwt 문자열)을 만든다.
        string GenerateAccessToken(User user);
    }
}
