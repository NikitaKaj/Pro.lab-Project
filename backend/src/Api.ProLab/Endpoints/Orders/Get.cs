using Ardalis.ApiEndpoints;
using ProLab.Data;
using ProLab.Data.Enums;

namespace ProLab.Api.Endpoints;

public class GetOrderData : EndpointBaseAsync
    .WithRequest<GetOrderRequest>
    .WithActionResult<GetOrderResponse>
{
    private readonly ApplicationDbContext ctx;

    public GetOrderData(ApplicationDbContext ctx)
    {
        this.ctx = ctx;
    }

    [HttpGet($"api/{Constants.ORDERS}/get")]
    [OpenApiTag(Constants.ORDERS)]
    [OpenApiOperation(Constants.ORDERS + "_get")]
    [ProducesResponseType(typeof(GetOrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult<GetOrderResponse>> HandleAsync([FromQuery] GetOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Id <= 0)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Nepareizs vaicajums",
                Detail = "Id nav numurs",
                Status = StatusCodes.Status400BadRequest
            });
        }
        try
        {
            var order = await ctx.Orders.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (order == null)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Nepareizs vaicajums",
                    Detail = "Nav tada kurjera",
                    Status = StatusCodes.Status400BadRequest
                });
            }
            var response = new GetOrderResponse()
            {
                OrderId = order.Id,
                ClientId = order.ClientId,
                CourierId = order.CourierId,
                RouteId = (long)order.RouteId,
                Address = order.Address,
                Status = order.Status,
                StartTimeLog = order.StartTimeLog,
                EndTimeLog = order.EndTimeLog,
                StartPoint = order.StartPoint,
                EndPoint = order.EndPoint,
                CreatedAt = order.CreatedAt,
                UpdatedAt = (DateTimeOffset)order.UpdatedAt
            };

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

public class GetOrderRequest
{
    [FromQuery]
    public long Id { get; set; }
}
public class GetOrderResponse
{
    public long OrderId { get; set; }
    public long ClientId { get; set; }
    public long CourierId { get; set; }
    public long RouteId { get; set; }
    public string Address { get; set; }
    public OrderStatus Status { get; set; }
    public DateTimeOffset StartTimeLog { get; set; }
    public DateTimeOffset EndTimeLog { get; set; }
    public string StartPoint { get; set; }
    public string EndPoint { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    
}