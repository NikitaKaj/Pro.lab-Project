using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using ProLab.Data.Entities.Routes;
using ProLab.Api.Infrastructure.Configurations;
using System.Text.Json;
using System.Globalization;

namespace ProLab.Api.Infrastructure.Services
{
    public class MapboxService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly MapboxOptions _options;

        public MapboxService(
            HttpClient httpClient,
            IMemoryCache cache,
            IOptions<MapboxOptions> options)
        {
            _httpClient = httpClient;
            _cache = cache;
            _options = options.Value;
        }

        public async Task<double[,]> GetDistanceMatrixAsync(
            List<Coordinate> coordinates,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = $"matrix_{string.Join("_", coordinates.Select(FormatCoord))}";

            if (_cache.TryGetValue<double[,]>(cacheKey, out var cachedMatrix))
                return cachedMatrix!;

            var coordinatesString = string.Join(";", coordinates.Select(FormatCoord));
            var url = $"{_options.BaseUrl}/directions-matrix/v1/mapbox/driving/{coordinatesString}";

            var query = $"?access_token={_options.AccessToken}&annotations=duration,distance";
            var fullUrl = $"{url}{query}";

            var response = await _httpClient.GetAsync(fullUrl, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var jsonDoc = JsonDocument.Parse(content);

            var durationsArray = jsonDoc.RootElement.GetProperty("durations");
            var n = coordinates.Count;
            var matrix = new double[n, n];

            for (var i = 0; i < n; i++)
            {
                var row = durationsArray[i];
                for (var j = 0; j < n; j++)
                {
                    matrix[i, j] = row[j].GetDouble();
                }
            }

            _cache.Set(cacheKey, matrix, TimeSpan.FromMinutes(_options.CacheDurationMinutes));

            return matrix;
        }

        public async Task<List<AlternativeRoute>> GetAlternativeRoutesAsync(
            Coordinate from,
            Coordinate to,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = $"alternatives_{from}_{to}";

            if (_cache.TryGetValue<List<AlternativeRoute>>(cacheKey, out var cached))
                return cached!;

            var url = $"{_options.BaseUrl}/directions/v5/mapbox/driving/{FormatCoord(from)};{FormatCoord(to)}";
            var query = $"?access_token={_options.AccessToken}&alternatives=true&geometries=geojson&overview=full";
            var fullUrl = $"{url}{query}";

            var response = await _httpClient.GetAsync(fullUrl, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var jsonDoc = JsonDocument.Parse(content);

            var routes = new List<AlternativeRoute>();
            var routesArray = jsonDoc.RootElement.GetProperty("routes");

            foreach (var route in routesArray.EnumerateArray())
            {
                var altRoute = new AlternativeRoute
                {
                    Distance = route.GetProperty("distance").GetDouble(),
                    Duration = route.GetProperty("duration").GetDouble(),
                    Weight = route.GetProperty("weight").GetDouble(),
                    Summary = route.GetProperty("legs")[0].GetProperty("summary").GetString() ?? "",
                    Geometry = ParseGeometry(route.GetProperty("geometry"))
                };

                routes.Add(altRoute);
            }

            _cache.Set(cacheKey, routes, TimeSpan.FromMinutes(_options.CacheDurationMinutes));

            return routes;
        }

        public async Task<RouteSegment> GetRouteGeometryAsync(
            List<Coordinate> orderedCoordinates,
            CancellationToken cancellationToken = default)
        {
            var coordinatesString = string.Join(";", orderedCoordinates.Select(FormatCoord));
            var url = $"{_options.BaseUrl}/directions/v5/mapbox/driving/{coordinatesString}";
            var query = $"?access_token={_options.AccessToken}&geometries=geojson&overview=full&steps=true";
            var fullUrl = $"{url}{query}";

            var response = await _httpClient.GetAsync(fullUrl, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var jsonDoc = JsonDocument.Parse(content);

            var route = jsonDoc.RootElement.GetProperty("routes")[0];

            return new RouteSegment
            {
                Distance = route.GetProperty("distance").GetDouble(),
                Duration = route.GetProperty("duration").GetDouble(),
                Weight = route.GetProperty("weight").GetDouble(),
                Geometry = ParseGeometry(route.GetProperty("geometry"))
            };
        }

        private List<Coordinate> ParseGeometry(JsonElement geometryElement)
        {
            var coordinates = new List<Coordinate>();
            var coordsArray = geometryElement.GetProperty("coordinates");

            foreach (var coord in coordsArray.EnumerateArray())
            {
                var lon = coord[0].GetDouble();
                var lat = coord[1].GetDouble();
                coordinates.Add(new Coordinate(lon, lat));
            }

            return coordinates;
        }

        private static string FormatCoord(Coordinate c) =>
            $"{c.Longitude.ToString(CultureInfo.InvariantCulture)},{c.Latitude.ToString(CultureInfo.InvariantCulture)}";

        public async Task<Coordinate?> GeocodeAddressAsync(
    string address,
    CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(address))
                return null;

            var cacheKey = $"geocode_{address.ToLowerInvariant()}";

            if (_cache.TryGetValue<Coordinate>(cacheKey, out var cachedCoordinate))
                return cachedCoordinate;

            var encodedAddress = Uri.EscapeDataString(address);
            var url = $"{_options.BaseUrl}/geocoding/v5/mapbox.places/{encodedAddress}.json";
            var query = $"?access_token={_options.AccessToken}&limit=1";
            var fullUrl = $"{url}{query}";

            var response = await _httpClient.GetAsync(fullUrl, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var jsonDoc = JsonDocument.Parse(content);

            var features = jsonDoc.RootElement.GetProperty("features");

            if (features.GetArrayLength() == 0)
                return null;

            var firstFeature = features[0];
            var coordinates = firstFeature.GetProperty("geometry").GetProperty("coordinates");

            var longitude = coordinates[0].GetDouble();
            var latitude = coordinates[1].GetDouble();

            var coordinate = new Coordinate(longitude, latitude);

            _cache.Set(cacheKey, coordinate, TimeSpan.FromMinutes(_options.CacheDurationMinutes));

            return coordinate;
        }
    }
}
