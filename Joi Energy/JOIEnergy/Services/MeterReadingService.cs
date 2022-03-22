using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JOIEnergy.Domain;

namespace JOIEnergy.Services
{
    public class MeterReadingService : IMeterReadingService
    {
        // 1- missing initialize MeterAsociatedReadings within a non-parameter constructor, otherwise would generate null reference value while try to add values (in line 28)
        // 2- If alreayd exists a "GetReadings" method, the access modifier could be "private" and add a "GetAll" method        
        public Dictionary<string, List<ElectricityReading>> MeterAssociatedReadings { get; set; }// = new Dictionary<string, List<ElectricityReading>>();

        // the "new" are only created in the test projects, no endpoint to initialize anything, its that ok? its just with test purposes?
        public MeterReadingService(Dictionary<string, List<ElectricityReading>> meterAssociatedReadings)
        {
            MeterAssociatedReadings = meterAssociatedReadings;
        }

        public List<ElectricityReading> GetReadings(string smartMeterId)
        {
            if (MeterAssociatedReadings.ContainsKey(smartMeterId))
            {
                return MeterAssociatedReadings[smartMeterId];
            }
            return new List<ElectricityReading>();
        }

        public List<ElectricityReading> GetReadingsFromLastWeek(string smartMeterId)
        {
            if (MeterAssociatedReadings.ContainsKey(smartMeterId))
                return MeterAssociatedReadings[smartMeterId].Where(x => x.Time > DateTime.Now.AddDays(-7)).ToList();

            return new List<ElectricityReading>();

        }

        public void StoreReadings(string smartMeterId, List<ElectricityReading> electricityReadings)
        {
            if (!MeterAssociatedReadings.ContainsKey(smartMeterId))
            {
                MeterAssociatedReadings.Add(smartMeterId, new List<ElectricityReading>());
            }

            // MeterAssociatedReadings[smartMeterId].AddRange(electricityReadings); --> easy way to use insted of using a foreach
            electricityReadings.ForEach(electricityReading => MeterAssociatedReadings[smartMeterId].Add(electricityReading));
        }

    }
}
