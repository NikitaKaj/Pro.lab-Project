using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProLab.Api.Endpoints.Accounts.Auth;
using ProLab.Api.Infrastructure.Services;
using ProLab.Api.Requests.Routes;
using ProLab.Api.Responses.Routes;
using ProLab.Data.Entities.Routes;
using ProLab.Data.Enums;

namespace ProLab.Api.Endpoints;

public class OptimizeRouteEndpoint : EndpointBaseAsync
    .WithRequest<OptimizeRouteRequest>
    .WithActionResult<OptimizeRouteResponse>
{
    private readonly RouteOptimizationService _optimizationService;

    public OptimizeRouteEndpoint(RouteOptimizationService optimizationService)
    {
        _optimizationService = optimizationService;
    }

    [AllowAnonymous]
    [HttpPost($"api/{Constants.ROUTES}/optimize")]
    [OpenApiTag(Constants.ROUTES)]
    [OpenApiOperation(Constants.ROUTES + "_" + nameof(Login))]
    [ProducesResponseType(typeof(OptimizeRouteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult<OptimizeRouteResponse>> HandleAsync([FromBody] OptimizeRouteRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Coordinates == null || request.Coordinates.Count < 2)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Nepareizs vaicajums",
                Detail = "Jabut vismaz 2 koordinatiem",
                Status = StatusCodes.Status400BadRequest
            });
        }

        if (request.StartIndex < 0 || request.StartIndex >= request.Coordinates.Count)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Nepareizs vaicajums",
                Detail = $"StartIndex jabut no 0 lidz {request.Coordinates.Count - 1}",
                Status = StatusCodes.Status400BadRequest
            });
        }

        if (request.Algorithm == OptimizationAlgorithm.WithAlternatives && request.SelectionStrategy == null)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Nepareizs vaicajums",
                Detail = "Algoritmam WithAlternatives jaievada SelectionStrategy",
                Status = StatusCodes.Status400BadRequest
            });
        }

        try
        {
            var coordinates = request.Coordinates
                .Select(c => new Coordinate(c.Longitude, c.Latitude))
                .ToList();

            OptimizedRoute optimizedRoute;

            if (request.Algorithm == OptimizationAlgorithm.NearestNeighbor)
            {
                optimizedRoute = await _optimizationService.OptimizeWithNearestNeighborAsync(
                    coordinates,
                    request.StartIndex,
                    cancellationToken);
            }
            else
            {
                optimizedRoute = await _optimizationService.OptimizeWithAlternativesAsync(
                    coordinates,
                    request.StartIndex,
                    request.SelectionStrategy!.Value,
                    cancellationToken);
            }

            var response = new OptimizeRouteResponse
            {
                VisitOrder = optimizedRoute.VisitOrder,
                TotalDistance = optimizedRoute.TotalDistance,
                TotalDuration = optimizedRoute.TotalDuration,
                FullGeometry = optimizedRoute.FullGeometry
                    .Select(c => new CoordinateDto
                    {
                        Longitude = c.Longitude,
                        Latitude = c.Latitude
                    })
                    .ToList(),
                Segments = optimizedRoute.Segments?.Select(s => new RouteSegmentDto
                {
                    FromIndex = s.FromIndex,
                    ToIndex = s.ToIndex,
                    Distance = s.Distance,
                    Duration = s.Duration,
                    Summary = s.Summary
                }).ToList()
            };

            return Ok(response);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new ProblemDetails
            {
                Title = "External service error",
                Detail = $"Mapbox API: {ex.Message}",
                Status = StatusCodes.Status503ServiceUnavailable
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = $"Optimizacijas kluda: {ex.Message}",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }
}
