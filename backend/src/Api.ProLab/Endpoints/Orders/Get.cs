using Ardalis.ApiEndpoints;
using ProLab.Data;
using ProLab.Data.Enums;

namespace ProLab.Api.Endpoints;

public class GetOrderData : EndpointBaseAsync
    .WithoutRequest
    .WithActionResult<List<GetOrderResponse>>
{
    private readonly ApplicationDbContext ctx;

    public GetOrderData(ApplicationDbContext ctx)
    {
        this.ctx = ctx;
    }

    [HttpGet($"api/{Constants.ORDERS}/get")]
    [OpenApiTag(Constants.ORDERS)]
    [OpenApiOperation(Constants.ORDERS + "_get")]
    [ProducesResponseType(typeof(List<GetOrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult<List<GetOrderResponse>>> HandleAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var orders = await ctx.Orders.ToListAsync(cancellationToken);
            var response = orders.OrderByDescending(o => o.CreatedAt).Select(o => new GetOrderResponse()
            {
                OrderId = o.Id,
                Customer = o.CustomerName,
                Address = o.Address,
                Status = o.Status,
                TimeLog = $"{o.StartTimeLog:HH:mm} : {o.EndTimeLog:HH:mm}",
                StartPoint = o.StartPoint,
                EndPoint = o.EndPoint,
                CreatedAt = o.CreatedAt.ToString("dd.MM.yyyy"),
            }).ToList();

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

public class GetOrderResponse
{
    public long OrderId { get; set; }
    public string? Customer { get; set; }
    public string? Address { get; set; }
    public OrderStatus Status { get; set; }
    public string? TimeLog { get; set; }
    public string? StartPoint { get; set; }
    public string? EndPoint { get; set; }
    public string? CreatedAt { get; set; }
    
}