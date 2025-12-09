using ProLab.Data.Entities.Routes;

namespace ProLab.Api.Infrastructure.Services
{
    public class RouteSelectionService
    {
        public AlternativeRoute SelectFastest(List<AlternativeRoute> alternatives)
        {
            if (alternatives == null || alternatives.Count == 0)
                throw new ArgumentException("Nav alternativu", nameof(alternatives));

            return alternatives
                .OrderBy(r => r.Duration)
                .First();
        }

        public AlternativeRoute SelectShortest(List<AlternativeRoute> alternatives)
        {
            if (alternatives == null || alternatives.Count == 0)
                throw new ArgumentException("Nav alternativu", nameof(alternatives));

            return alternatives
                .OrderBy(r => r.Distance)
                .First();
        }

        public AlternativeRoute SelectBalanced(
            List<AlternativeRoute> alternatives,
            double durationWeight = 0.6,
            double distanceWeight = 0.4)
        {
            if (alternatives == null || alternatives.Count == 0)
                throw new ArgumentException("Nav alternativu", nameof(alternatives));

            return alternatives
                .OrderBy(r => CalculateScore(r, durationWeight, distanceWeight))
                .First();
        }

        public AlternativeRoute SelectLightestWeight(List<AlternativeRoute> alternatives)
        {
            if (alternatives == null || alternatives.Count == 0)
                throw new ArgumentException("Nav alternativu", nameof(alternatives));

            return alternatives
                .OrderBy(r => r.Weight)
                .First();
        }

        public AlternativeRoute SelectAvoidingStreets(
            List<AlternativeRoute> alternatives,
            List<string> streetsToAvoid)
        {
            if (alternatives == null || alternatives.Count == 0)
                throw new ArgumentException("Nav alternativu", nameof(alternatives));

            var filtered = alternatives
                .Where(r => !streetsToAvoid.Any(street =>
                    r.Summary.Contains(street, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            return filtered.Count > 0
                ? SelectFastest(filtered)
                : SelectFastest(alternatives);
        }

        public AlternativeRoute SelectByTimeOfDay(List<AlternativeRoute> alternatives)
        {
            var hour = DateTime.Now.Hour;

            return hour switch
            {
                >= 6 and < 12 => SelectFastest(alternatives),      // Morning: fast
                >= 12 and < 18 => SelectShortest(alternatives),     // Day: distance
                >= 18 and < 22 => SelectLightestWeight(alternatives), // Evening: without пробки
                _ => SelectBalanced(alternatives)                   // Night: balance
            };
        }

        private double CalculateScore(AlternativeRoute route, double durationWeight, double distanceWeight)
        {
            return (route.Duration * durationWeight) +
                   ((route.Distance / 100.0) * distanceWeight);
        }
    }
}
