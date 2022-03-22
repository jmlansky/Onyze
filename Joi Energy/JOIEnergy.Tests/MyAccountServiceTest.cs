using JOIEnergy.Enums;
using JOIEnergy.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace JOIEnergy.Tests
{
    public class MyAccountServiceTest
    {
        private readonly IAccountService accountService;
        private const string SMART_METER_ID = "smart-meter-3";

        readonly IServiceProvider _services = Program.BuildWebHost(new string[] { }).Services;


        public MyAccountServiceTest()
        {
            accountService = (IAccountService)_services.GetService(typeof(IAccountService));            
        }

        [Fact]
        public void GivenTheSmartMeterIdReturnsThePricePlanId()
        {
            var result = accountService.GetPricePlanIdForSmartMeterId(SMART_METER_ID);
            Assert.Equal(Supplier.PowerForEveryone, result);
        }
    }
}
