using Ardalis.ApiEndpoints;
using ProLab.Api.Responses.CouriersResponses;
using ProLab.Data;

namespace ProLab.Api.Endpoints;

public class GetCourier : EndpointBaseAsync
    .WithoutRequest
    .WithActionResult<List<CourierResponse>>
{
    private readonly ApplicationDbContext ctx;

    public GetCourier(ApplicationDbContext ctx)
    {
        this.ctx = ctx;
    }

    [HttpGet($"api/{Constants.COURIERS}/get")]
    [OpenApiTag(Constants.COURIERS)]
    [OpenApiOperation(Constants.COURIERS + "_get")]
    [ProducesResponseType(typeof(List<CourierResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult<List<CourierResponse>>> HandleAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await ctx.Couriers
                .Select(c => new CourierResponse
                {
                    CourierId = c.Id,
                    FullName = c.FullName,
                    CompletedOrdersCount = ctx.Orders.Count(o => o.CourierId == c.Id),
                    CreatedAt = c.CreatedAt,
                    ActiveOrdersCount = ctx.Orders.Count(o => o.CourierId == c.Id && o.Status == Data.Enums.OrderStatus.Pending),
                })
                .ToListAsync(cancellationToken);

            return Ok(response);
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