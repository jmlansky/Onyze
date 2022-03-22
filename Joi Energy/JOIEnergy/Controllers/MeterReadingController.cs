using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JOIEnergy.Domain;
using JOIEnergy.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JOIEnergy.Controllers
{
    [Route("readings")]
    public class MeterReadingController : Controller
    {
        private readonly IMeterReadingService _meterReadingService;

        public MeterReadingController(IMeterReadingService meterReadingService)
        {
            _meterReadingService = meterReadingService;
        }

        [HttpGet("readings/{smartMeterId}")]
        public ObjectResult GetMeterReadingsFromLastWeek(string smartMeterId)
        {
            if (string.IsNullOrWhiteSpace(smartMeterId))
                return new BadRequestObjectResult("empty smartMeterId");

            var reading = _meterReadingService.GetReadingsFromLastWeek(smartMeterId);

            if (reading != null && reading.Any() )
                return new OkObjectResult(reading);

            return new NotFoundObjectResult(string.Format("Smart Meter ID ({0}) not found", smartMeterId));
        }



        [HttpPost("store")]
        // The MeterReadings object shouldnt be in the request, it should be a DTO, to separate concenrns. The client shouldnt know the business entitities (pattern Reqeuest/response - separation of concerns)
        public ObjectResult Post([FromBody] MeterReadings meterReadings)
        {
            //If the validation its a request validation --> its ok to be in the controller, if its a business validation, should be in the service layer
            if (!IsMeterReadingsValid(meterReadings))
            {
                // If the request is wrong, the error shouldnt be "internal server error", should be more descriptive or else "BadRequest()" because its not an internal server error
                return new BadRequestObjectResult("Internal Server Error");
            }

            _meterReadingService.StoreReadings(meterReadings.SmartMeterId, meterReadings.ElectricityReadings);

            // return empty value? why not return an IActionResult Ok()
            return new OkObjectResult("{}");
        }

        // the private method could be at the bottom to place all the private methods together. Also there are some people, who doesnt like private methods --> its ok by me.
        private bool IsMeterReadingsValid(MeterReadings meterReadings)
        {
            // All of this could be regrouped but its ok
            String smartMeterId = meterReadings.SmartMeterId;
            List<ElectricityReading> electricityReadings = meterReadings.ElectricityReadings;

            // smartMeterId is a string value, so the validation could be optimized to "isNullOrWhiteSpace"
            return smartMeterId != null && smartMeterId.Any() && electricityReadings != null && electricityReadings.Any();

            //var hasSmartMeterId = !string.IsNullOrWhiteSpace(meterReadings.SmartMeterId);
            //var hasElectricityReadings = meterReadings.ElectricityReadings != null && meterReadings.ElectricityReadings.Any();
            //return hasSmartMeterId && hasElectricityReadings;
        }

        [HttpGet("read/{smartMeterId}")]
        public ObjectResult GetReading(string smartMeterId)
        {
            return new OkObjectResult(_meterReadingService.GetReadings(smartMeterId));
        }
    }
}
