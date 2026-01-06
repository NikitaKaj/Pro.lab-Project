using Ardalis.ApiEndpoints;
using ProLab.Data;
using ProLab.Data.Enums;
using System.ComponentModel.DataAnnotations;
using ProLab.Data.Entities.Orders;

namespace ProLab.Api.Endpoints;

public class PlaceOrder : EndpointBaseAsync
    .WithRequest<PlaceOrderRequest>
    .WithActionResult<SuccessResult>
{
    private readonly ApplicationDbContext ctx;

    public PlaceOrder(ApplicationDbContext ctx)
    {
        this.ctx = ctx;
    }

    [HttpPost($"api/{Constants.ORDERS}/place")]
    [OpenApiTag(Constants.ORDERS)]
    [OpenApiOperation(Constants.ORDERS + "_place")]
    [ProducesResponseType(typeof(SuccessResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult<SuccessResult>> HandleAsync([FromBody] PlaceOrderRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
           Order order = new Order()
           {
               CustomerName = request.Customer,
               CourierId = request.CourierId,
               Address = request.Address,
               Status = request.Status,
           };

            ctx.Orders.Add(order);
            await ctx.SaveChangesAsync();

            return Ok(new { });
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

public class PlaceOrderRequest
{
    public string? Customer { get; set; }
    [Required]
    public required string Address { get; set; }
    public OrderStatus Status { get; set; }
    [Required]
    public required long CourierId { get; set; }
}