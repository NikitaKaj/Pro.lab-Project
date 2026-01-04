using Ardalis.ApiEndpoints;
using ProLab.Data;
using ProLab.Data.Entities.Couriers;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace ProLab.Api.Endpoints;

public class DeleteCourier : EndpointBaseAsync
    .WithRequest<DeleteCourierRequest>
    .WithActionResult
{
    private readonly ApplicationDbContext ctx;

    public DeleteCourier(ApplicationDbContext ctx)
    {
        this.ctx = ctx;
    }

    [HttpPost($"api/{Constants.COURIERS}/delete")]
    [OpenApiTag(Constants.COURIERS)]
    [OpenApiOperation(Constants.COURIERS + "_delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult> HandleAsync([FromQuery] DeleteCourierRequest request, CancellationToken cancellationToken = default)
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
            var courier = await ctx.Couriers.FindAsync(request.Id , cancellationToken);
            if (courier == null)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Nepareizs vaicajums",
                    Detail = "Nav tada kurjera",
                    Status = StatusCodes.Status400BadRequest
                });
            }
            ctx.Couriers.Remove(courier);
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

public class DeleteCourierRequest
{
    [Required]
    public long Id { get; set; }
}