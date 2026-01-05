using Ardalis.ApiEndpoints;
using ProLab.Data;
using ProLab.Data.Entities.Couriers;
using System.ComponentModel.DataAnnotations;

namespace ProLab.Api.Endpoints;

public class CreateCourier : EndpointBaseAsync
    .WithRequest<CreateCourierRequest>
    .WithActionResult<CreateCourierResponse>
{
    private readonly ApplicationDbContext ctx;

    public CreateCourier(ApplicationDbContext ctx)
    {
        this.ctx = ctx;
    }

    [HttpPost($"api/{Constants.COURIERS}/create")]
    [OpenApiTag(Constants.COURIERS)]
    [OpenApiOperation(Constants.COURIERS + "_create")]
    [ProducesResponseType(typeof(CreateCourierResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult<CreateCourierResponse>> HandleAsync([FromBody] CreateCourierRequest request, CancellationToken cancellationToken = default)
    {   
        if(request.FullName == null)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Nepareizs vaicajums",
                Detail = "Needs normal name",
                Status = StatusCodes.Status400BadRequest
            });
        }
        try
        {
            var courier = new Courier
            {
                FullName = request.FullName,
                UpdatedAt = null,
                CreatedAt = DateTime.UtcNow
            };
            ctx.Couriers.Add(courier);
            await ctx.SaveChangesAsync(cancellationToken);
            var response = new CreateCourierResponse()
            {
                Id = courier.Id
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

public class CreateCourierRequest
{
    //[Required]
   // public long Id { get; set; }

    [Required]
    public string FullName { get; set; }


}
public class CreateCourierResponse
{
    public long Id { get; set; }
}