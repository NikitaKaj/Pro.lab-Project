using Ardalis.ApiEndpoints;
using ProLab.Api.Responses.CouriersResponses;
using ProLab.Data;

namespace ProLab.Api.Endpoints;

public class GetCourier : EndpointBaseAsync
    .WithoutRequest
    .WithActionResult<List<GetCourierResponse>>
{
    private readonly ApplicationDbContext ctx;

    public GetCourier(ApplicationDbContext ctx)
    {
        this.ctx = ctx;
    }

    [HttpGet($"api/{Constants.COURIERS}/get")]
    [OpenApiTag(Constants.COURIERS)]
    [OpenApiOperation(Constants.COURIERS + "_get")]
    [ProducesResponseType(typeof(List<GetCourierResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult<List<GetCourierResponse>>> HandleAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await ctx.Couriers
                .Select(c => new CourierResponse
                {
                    CourierId = c.Id,
                    FullName = c.FullName,
                    CompletedOrdersCount = ctx.Orders.Count(o => o.CourierId == c.Id),
                    CreatedAt = c.CreatedAt
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

public class GetCourierResponse
{
    public long CourierId { get; set; }
    public string FullName { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}