using System.Collections.Generic;
using System.Threading.Tasks;
using JOIEnergy.Domain;

namespace JOIEnergy.Services
{
    public interface IMeterReadingService
    {
        List<ElectricityReading> GetReadings(string smartMeterId);
        List<ElectricityReading> GetReadingsFromLastWeek(string smartMeterId);

        void StoreReadings(string smartMeterId, List<ElectricityReading> electricityReadings);
    }
}