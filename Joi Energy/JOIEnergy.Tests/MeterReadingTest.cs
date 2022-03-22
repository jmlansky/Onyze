using JOIEnergy.Controllers;
using JOIEnergy.Domain;
using JOIEnergy.Enums;
using JOIEnergy.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace JOIEnergy.Tests
{
    public class MeterReadingTest
    {
        private MeterReadingController controller;
        private static string NON_EXISTING_SMART_METER_ID = "smart-meter-id";
        private static string EXISTING_SMART_METER_ID = "smart-meter-0";

        public MeterReadingTest()
        {

            var readings = new Dictionary<string, List<ElectricityReading>>();
            readings.Add(EXISTING_SMART_METER_ID, new List<ElectricityReading>() { new ElectricityReading() { Reading = 25m, Time = DateTime.Now.AddDays(-1) } });
            IMeterReadingService meterReadingService = new MeterReadingService(readings);

            controller = new MeterReadingController(meterReadingService);
        }


        [Fact]
        public void MeterReadingFromLastWeekTestNonExistingMeter()
        {
            var a = controller.GetMeterReadingsFromLastWeek(NON_EXISTING_SMART_METER_ID);

            Assert.Equal(404, a.StatusCode);
        }

        [Fact]
        public void MeterReadingFromLastWeekTestExistingMeter()
        {
            var a = controller.GetMeterReadingsFromLastWeek(EXISTING_SMART_METER_ID);

            Assert.Equal(200, a.StatusCode);
        }
    }
}
