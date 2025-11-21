using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProLab.Data.Enums;

namespace ProLab.Api.Responses.OrdersResponse
{
    public class OrderResponse
    {
        public long OrderId { get; set; }
        public string ClientName { get; set; }
        public string Address { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}