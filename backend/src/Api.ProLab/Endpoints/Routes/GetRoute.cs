using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProLab.Api.Infrastructure.Services;
using ProLab.Api.Requests.Routes;
using ProLab.Api.Responses.Routes;
using ProLab.Data;
using ProLab.Data.Entities.Routes;
using ProLab.Data.Enums;

namespace ProLab.Api.Endpoints;

public class GetRoute : EndpointBaseAsync
    .WithRequest<GetRouteRequest>
    .WithActionResult<OptimizeRouteResponse>
{
    private readonly RouteOptimizationService _optimizationService;
    private readonly MapboxService _mapboxService;
    private readonly ApplicationDbContext _ctx;

    public GetRoute(
        RouteOptimizationService optimizationService,
        MapboxService mapboxService,
        ApplicationDbContext ctx)
    {
        _optimizationService = optimizationService;
        _mapboxService = mapboxService;
        _ctx = ctx;
    }

    [AllowAnonymous]
    [HttpPost("api/routes/generate")]
    [OpenApiTag(Constants.ROUTES)]
    [OpenApiOperation(Constants.ROUTES + "_" + nameof(GetRoute))]
    [ProducesResponseType(typeof(OptimizeRouteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult<OptimizeRouteResponse>> HandleAsync(
        [FromBody] GetRouteRequest request,
        CancellationToken cancellationToken = default)
    {
        var orders = await _ctx.Orders
            .Where(o => o.CourierId == request.CourierId)
            .ToListAsync(cancellationToken);

        if (!orders.Any())
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Nav pasūtījumu",
                Detail = "Šim kurjeram nav pasūtījumu",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var ordersWithoutAddress = orders.Where(o => string.IsNullOrWhiteSpace(o.Address)).ToList();
        if (ordersWithoutAddress.Any())
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Trūkst adreses",
                Detail = $"Pasūtījumiem bez adreses: {string.Join(", ", ordersWithoutAddress.Select(o => o.Id))}",
                Status = StatusCodes.Status400BadRequest
            });
        }

        if (orders.Count < 2)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Nepietiek pasūtījumu",
                Detail = "Nepieciešami vismaz 2 pasūtījumi maršruta izveidei",
                Status = StatusCodes.Status400BadRequest
            });
        }

        try
        {
            var coordinates = new List<Coordinate>();
            var failedAddresses = new List<(long orderId, string address)>();

            foreach (var order in orders)
            {
                var coordinate = await _mapboxService.GeocodeAddressAsync(
                    order.Address,
                    cancellationToken);

                if (coordinate == null)
                {
                    failedAddresses.Add((order.Id, order.Address));
                }
                else
                {
                    coordinates.Add(coordinate);
                }
            }

            if (failedAddresses.Any())
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Neizdevās atrast adreses",
                    Detail = $"Neizdevās atrast koordinātas šādām adresēm: {string.Join(", ", failedAddresses.Select(f => $"Pasūtījums #{f.orderId}: {f.address}"))}",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            if (coordinates.Count < 2)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Nepietiek derīgu adrešu",
                    Detail = "Nepieciešamas vismaz 2 derīgas adreses",
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

            OptimizedRoute optimizedRoute;

            if (request.Algorithm == OptimizationAlgorithm.NearestNeighbor)
            {
                optimizedRoute = await _optimizationService.OptimizeWithNearestNeighborAsync(
                    coordinates,
                    0,
                    cancellationToken);
            }
            else
            {
                optimizedRoute = await _optimizationService.OptimizeWithAlternativesAsync(
                    coordinates,
                    0,
                    request.SelectionStrategy!.Value,
                    cancellationToken);
            }

            var orderedOrders = optimizedRoute.VisitOrder
                .Select(index => orders[index])
                .ToList();

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
                }).ToList(),
                Coordinates = coordinates
            };

            return Ok(response);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new ProblemDetails
            {
                Title = "Ārējā servisa kļūda",
                Detail = $"Mapbox API kļūda: {ex.Message}",
                Status = StatusCodes.Status503ServiceUnavailable
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Servera kļūda",
                Detail = $"Maršruta optimizācijas kļūda: {ex.Message}",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }
}
public class GetRouteRequest
{
    public long CourierId { get; set; }
    public OptimizationAlgorithm Algorithm { get; set; } = OptimizationAlgorithm.NearestNeighbor;
    public SelectionStrategy? SelectionStrategy { get; set; }
}