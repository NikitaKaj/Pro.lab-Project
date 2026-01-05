using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProLab.Data.Entities.Clients;
using ProLab.Data.Entities.Routes;
using ProLab.Data.Enums;
using ProLab.Data.Shared;

namespace ProLab.Data.Entities.Orders
{
    public class Order : BaseEntity<long>
    {
        public long ClientId { get; set; }
        public string? CustomerName { get; set; }
        public long CourierId { get; set; }
        public long? RouteId { get; set; }
        public string Address { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime StartTimeLog { get; set; }
        public DateTime EndTimeLog { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
    }
}