using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProLab.Api.Responses.CouriersResponses
{
    public class CourierResponse
    {
        public long CourierId { get; set; }
        public string FullName { get; set; }
        public int CompletedOrdersCount { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}