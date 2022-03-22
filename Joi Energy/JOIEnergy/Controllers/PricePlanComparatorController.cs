using System.Collections.Generic;
using System.Linq;
using JOIEnergy.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JOIEnergy.Controllers
{
    [Route("price-plans")]
    public class PricePlanComparatorController : Controller
    {
        private readonly IPricePlanService _pricePlanService;
        private readonly IAccountService _accountService;

        public PricePlanComparatorController(IPricePlanService pricePlanService, IAccountService accountService)
        {
            // the "this" value could be removed because the variable names are different, but its ok
            this._pricePlanService = pricePlanService;
            this._accountService = accountService;
        }

        [HttpGet("compare-all/{smartMeterId}")]
        public ObjectResult CalculatedCostForEachPricePlan(string smartMeterId)
        {
            // unused variable, remove
            var pricePlanId = _accountService.GetPricePlanIdForSmartMeterId(smartMeterId);

            // should validate if the smartMeterId is not null or whitespace to avoid unnecessary processing.
            //if (string.IsNullOrWhiteSpace(smartMeterId)) return new BadRequestObjectResult("smartMeterId is null or empty");
            var costPerPricePlan = _pricePlanService.GetConsumptionCostOfElectricityReadingsForEachPricePlan(smartMeterId);

            if (!costPerPricePlan.Any())
            {
                // what if the smartMeter exists but has no consumption? its a not found error?
                return new NotFoundObjectResult(string.Format("Smart Meter ID ({0}) not found", smartMeterId));
            }

            // all below could be replaced by return new ObjectResult(JObject.FromObject(costPerPricePlan)); no need of further validations.
            dynamic response = JObject.FromObject(costPerPricePlan);

            // why validate twice the costPerPricePlan if its done in the line 32
            return
                costPerPricePlan.Any() ?
                new ObjectResult(response) :
                new NotFoundObjectResult(string.Format("Smart Meter ID ({0}) not found", smartMeterId));
        }

        [HttpGet("recommend/{smartMeterId}")]

        // dont remember if the limit should be in  the querystring as well ... assume it has to be.
        public ObjectResult RecommendCheapestPricePlans(string smartMeterId, int? limit = null)
        {
            // should validate if the smartMeterId is not null or whitespace to avoid unnecessary processing.
            //if (string.IsNullOrWhiteSpace(smartMeterId)) return new BadRequestObjectResult("smartMeterId is null or empty");

            // All of this business logic should be implemented in the Service
            // var recomendations = _pricePlanService.GetCheapestPricePlans(smartMeterId, limit);
            //------------------------
            var consumptionForPricePlans = _pricePlanService.GetConsumptionCostOfElectricityReadingsForEachPricePlan(smartMeterId);

            if (!consumptionForPricePlans.Any())
            {
                // wrong message, if there are no consumptions for this plan, doesn't mean the id is not found.
                return new NotFoundObjectResult(string.Format("Smart Meter ID ({0}) not found", smartMeterId));
            }

            var recommendations = consumptionForPricePlans.OrderBy(pricePlanComparison => pricePlanComparison.Value);

            if (limit.HasValue && limit.Value < recommendations.Count())
            {
                // The function required is Take or Where?
                return new ObjectResult(recommendations.Take(limit.Value));
            }
            //---------------------------

            return new ObjectResult(recommendations);
        }
    }
}
