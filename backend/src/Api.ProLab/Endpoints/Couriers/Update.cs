using Ardalis.ApiEndpoints;
using ProLab.Data;
using ProLab.Data.Entities.Couriers;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace ProLab.Api.Endpoints;

public class UpdateCourier : EndpointBaseAsync
    .WithRequest<UpdateCourierRequest>
    .WithActionResult
{
    private readonly ApplicationDbContext ctx;

    public UpdateCourier(ApplicationDbContext ctx)
    {
        this.ctx = ctx;
    }

    [HttpPost($"api/{Constants.COURIERS}/update")]
    [OpenApiTag(Constants.COURIERS)]
    [OpenApiOperation(Constants.COURIERS + "_update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult> HandleAsync([FromBody] UpdateCourierRequest request, CancellationToken cancellationToken = default)
    {
        if (request.FullName == null)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Nepareizs vaicajums",
                Detail = "Needs normal name or id",
                Status = StatusCodes.Status400BadRequest
            });
        }
        try
        {
            var courier = await ctx.Couriers.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (courier == null)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Nepareizs vaicajums",
                    Detail = "Nav tada kurjera",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            courier.FullName = request.FullName;
            courier.UpdatedAt = DateTime.UtcNow;
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

public class UpdateCourierRequest
{
    public long Id { get; set; }

    [Required]
    public string FullName { get; set; }


}