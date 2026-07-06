using System;
using System.Collections.Generic;
using System.Text;

namespace Reservation.Application.Results
{
    public record AuthResult(string AccessToken, string RefreshToken);
}
