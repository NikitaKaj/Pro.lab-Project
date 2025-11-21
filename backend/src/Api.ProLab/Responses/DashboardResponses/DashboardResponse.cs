using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProLab.Api.Responses.DashboardResponses
{
    public class DashboardResponse
    {
        public int CompletedOrdersCount { get; set; }
        public int InProgresOrdersCount { get; set; }
        public int CancelledOrdersCount { get; set; }
    }
}