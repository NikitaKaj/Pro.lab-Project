using Ardalis.ApiEndpoints;
using ProLab.Data;
using System.ComponentModel.DataAnnotations;

namespace ProLab.Api.Endpoints;

public class DeleteOrder : EndpointBaseAsync
    .WithRequest<DeleteOrderRequest>
    .WithActionResult
{
    private readonly ApplicationDbContext ctx;

    public DeleteOrder(ApplicationDbContext ctx)
    {
        this.ctx = ctx;
    }

    [HttpPost($"api/{Constants.ORDERS}/delete")]
    [OpenApiTag(Constants.ORDERS)]
    [OpenApiOperation(Constants.ORDERS + "_delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult> HandleAsync([FromQuery] DeleteOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Id <= 0)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Nepareizs vaicajums",
                Detail = "Wrong id",
                Status = StatusCodes.Status400BadRequest
            });
        }
        try
        {
            var order = await ctx.Orders.FindAsync(request.Id, cancellationToken);
            if (order == null)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Nepareizs vaicajums",
                    Detail = "Nav tada kurjera",
                    Status = StatusCodes.Status400BadRequest
                });
            }
            ctx.Orders.Remove(order);
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

public class DeleteOrderRequest
{
    [Required]
    public long Id { get; set; }
}