using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using ProLab.Data;
using ProLab.Data.Enums;

namespace ProLab.Api.Endpoints.Orders
{
    public class UpdateOrderStatus : EndpointBaseAsync
    .WithRequest<UpdateOrderStatusRequest>
    .WithActionResult
{
    private readonly ApplicationDbContext ctx;

    public UpdateOrderStatus(ApplicationDbContext ctx)
    {
        this.ctx = ctx;
    }

    [HttpPatch($"api/{Constants.ORDERS}/update")]
    [OpenApiTag(Constants.ORDERS)]
    [OpenApiOperation(Constants.ORDERS + "_update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult> HandleAsync([FromQuery] UpdateOrderStatusRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var order = await ctx.Orders.FindAsync(request.Id, cancellationToken);
            if (order == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Nepareizs vaicajums",
                    Detail = "Nav tada pasūtījuma",
                    Status = StatusCodes.Status404NotFound
                });
            }
            order.Status = request.Status;
            await ctx.SaveChangesAsync(cancellationToken);

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = $"Kluda: {ex.Message}",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }
}
}
    public class UpdateOrderStatusRequest
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public OrderStatus Status { get; set; }
    }