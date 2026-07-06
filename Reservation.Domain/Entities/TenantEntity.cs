using System;
using System.Collections.Generic;
using System.Text;

namespace Reservation.Domain.Entities
{
    public abstract class TenantEntity : BaseEntity
    {
        public Guid TenantId { get; set; }
    }
}
