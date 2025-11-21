using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProLab.Data.Entities.Orders;
using ProLab.Data.Shared;

namespace ProLab.Data.Entities.Routes
{
    public class Route : BaseEntity<long>
    {
        public long? OrderId { get; set; }
        public string StartPoint { get; set; }

    }
}