using Ardalis.ApiEndpoints;
using ProLab.Data;

namespace ProLab.Api.Endpoints;

public class GetCourierData : EndpointBaseAsync
    .WithRequest<CourierDataRequest>
    .WithActionResult<CourierDataResponse>
{
    private readonly ApplicationDbContext ctx;

    public GetCourierData(ApplicationDbContext ctx)
    {
        this.ctx = ctx;
    }

    [HttpGet($"api/{Constants.COURIERS}/get")]
    [OpenApiTag(Constants.COURIERS)]
    [OpenApiOperation(Constants.COURIERS + "_get")]
    [ProducesResponseType(typeof(CourierDataResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult<CourierDataResponse>> HandleAsync([FromQuery] CourierDataRequest request, CancellationToken cancellationToken = default)
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
            var courier = await ctx.Couriers.FirstOrDefaultAsync(x => x.Id == request.Id);
            if(courier == null) 
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Nepareizs vaicajums",
                    Detail = "Nav tada kurjera",
                    Status = StatusCodes.Status400BadRequest
                });
            }
            var response = new CourierDataResponse()
            {
                CourierId = courier.Id,
                FullName = courier.FullName,
                UpdatedAt = (DateTimeOffset)courier.UpdatedAt,
                CreatedAt = courier.CreatedAt
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

public class CourierDataRequest
{
    [FromQuery]
    public long Id { get; set; }
}
public class CourierDataResponse
{
    public long CourierId { get; set; }
    public string FullName { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}