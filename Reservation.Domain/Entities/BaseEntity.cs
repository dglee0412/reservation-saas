using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Reservation.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } 
        public DateTime CreatedAt { get; set; }

        [Timestamp]
        public uint Version { get; set; }
    }
}
