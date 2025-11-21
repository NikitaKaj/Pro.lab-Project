using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using ProLab.Api.Responses.DashboardResponses;
using ProLab.Data;

namespace ProLab.Api.Endpoints.DashboardEndpoints
{
    public class GetDashboardStats : EndpointBaseAsync
        .WithoutRequest
        .WithActionResult<DashboardResponse>
    {
        private readonly ILogger<GetDashboardStats> logger;
        private readonly ApplicationDbContext context;
        public GetDashboardStats(ILogger<GetDashboardStats> logger, ApplicationDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        [AllowAnonymous]
        [HttpGet($"api/dashboard/getstats")]
        [OpenApiTag(Constants.DASHBOARD)]
        [OpenApiOperation(Constants.DASHBOARD + "_" + nameof(GetDashboardStats))]
        public override async Task<ActionResult<DashboardResponse>> HandleAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var orders = context.Orders;

                var completed = await orders.Where(x => x.Status == Data.Enums.OrderStatus.Completed).CountAsync(cancellationToken);
                var inRoute = await orders.Where(x => x.Status == Data.Enums.OrderStatus.InRoute).CountAsync(cancellationToken);
                var pending = await orders.Where(x => x.Status == Data.Enums.OrderStatus.Pending).CountAsync(cancellationToken);
                var cancelled = await orders.Where(x => x.Status == Data.Enums.OrderStatus.Cancelled).CountAsync(cancellationToken);

                var stats = new DashboardResponse
                {
                    CompletedOrdersCount = completed,
                    InProgresOrdersCount = inRoute + pending,
                    CancelledOrdersCount = cancelled,
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get dashboard stats");
                return BadRequest();
            }
        }
    }
}