using System;
using System.Collections.Generic;
using System.Linq;
using JOIEnergy.Domain;

namespace JOIEnergy.Services
{
    public class PricePlanService : IPricePlanService
    {
        // unused --> remove
        public interface Debug { void Log(string s); };

        private readonly List<PricePlan> _pricePlans;
        private IMeterReadingService _meterReadingService;

        // what if the constructor does not include priceplan? there is no non-parameter constructor in that case?
        public PricePlanService(List<PricePlan> pricePlan, IMeterReadingService meterReadingService)
        {
            _pricePlans = pricePlan;
            _meterReadingService = meterReadingService;
        }

        // Method names should begin with Capital letters.
        // Group public/private methods to facilitate the reading
        private decimal calculateAverageReading(List<ElectricityReading> electricityReadings)
        {           
            var newSummedReadings = electricityReadings.Select(readings => readings.Reading).Aggregate((reading, accumulator) => reading + accumulator);
            // its not the same than: var a = electricityReadings.Sum(x => x.Reading);

            return newSummedReadings / electricityReadings.Count();
        }

        // Method names should begin with Capital letters.
        private decimal calculateTimeElapsed(List<ElectricityReading> electricityReadings)
        {
            var first = electricityReadings.Min(reading => reading.Time);
            var last = electricityReadings.Max(reading => reading.Time);

            // Question: first and last are the same than?:
            /* 
                electricityReadings = electricityReadings.OrderBy(x => x.Time).ToList();
                first = Convert.ToDateTime( electricityReadings.First());
                first = Convert.ToDateTime(electricityReadings.Last());
            */

            return (decimal)(last - first).TotalHours;
        }

        // Method names should begin with Capital letters.
        private decimal calculateCost(List<ElectricityReading> electricityReadings, PricePlan pricePlan)
        {
            var average = calculateAverageReading(electricityReadings);
            var timeElapsed = calculateTimeElapsed(electricityReadings);

            // shouldn't be any problem but... what if timeElapsed = 0?
            var averagedCost = average/timeElapsed;
            return averagedCost * pricePlan.UnitRate;
        }

        public Dictionary<String, decimal> GetConsumptionCostOfElectricityReadingsForEachPricePlan(String smartMeterId)
        {
            // there is a pattern wich say "validate in every layer" --> according to this:
            // If using Test projects and injecting the service dependency, the validation made in the controller would be skipped

            //if (string.IsNullOrWhiteSpace(smartMeterId)) return new Dictionary<string, decimal>();

            List<ElectricityReading> electricityReadings = _meterReadingService.GetReadings(smartMeterId);

            if (!electricityReadings.Any())
            {
                return new Dictionary<string, decimal>();
            }

            return _pricePlans.ToDictionary(plan => plan.EnergySupplier.ToString(), plan => calculateCost(electricityReadings, plan));
        }
    }
}