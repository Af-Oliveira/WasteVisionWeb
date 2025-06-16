using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DDDSample1.Domain.ObjectPredictions;
using DDDSample1.Domain.Shared;
using DDDSample1.Application.Shared;
using DDDSample1.Domain.Predictions;


namespace DDDSample1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObjectPredictionController : ControllerBase
    {
        private readonly IObjectPredictionService _objectPredictionService;

        public ObjectPredictionController(IObjectPredictionService objectPredictionService)
        {
            _objectPredictionService = objectPredictionService;
        }

        // GET: api/objectprediction
        [HttpGet]
        public async Task<ActionResult> GetAllObjectPredictions([FromQuery] ObjectPredictionSearchParamsDto searchParams)
        {
            
            try
            {
                ;
                var objectPredictionList = await _objectPredictionService.GetAllAsync(searchParams);
                return ApiResponse.For(objectPredictionList).WithMessage("Object predictions retrieved successfully.").Build(StatusCodeEnum.Success);
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<IEnumerable<ObjectPredictionDto>>().AsError().WithMessage(ex.Message).Build(StatusCodeEnum.BadRequestError);
            }
        }


        // GET api/objectprediction/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult> GetObjectPrediction(string id)
        {
            try
            {
                var objectPrediction = await _objectPredictionService.GetByIdAsync(new ObjectPredictionId(id));
                if (objectPrediction == null)
                {
                    return ApiResponse.For<ObjectPredictionDto>().AsError().WithMessage($"Object prediction with ID {id} not found.").Build(StatusCodeEnum.NotFoundError);
                }

                return ApiResponse.For(objectPrediction).WithMessage("Object prediction retrieved successfully.").Build(StatusCodeEnum.Success);
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<ObjectPredictionDto>().AsError().WithMessage(ex.Message).Build(StatusCodeEnum.BadRequestError);
            }
        }

                // GET api/objectprediction/{id}
        [HttpGet("prediction/{id}")]
        public async Task<ActionResult> GetObjectPredictionByPredictionId(string id)
        {
            try
            {
                var objectPrediction = await _objectPredictionService.GetByPredictionIdAsync(new PredictionId(id));
                if (objectPrediction == null)
                {
                    return ApiResponse.For<ObjectPredictionDto>().AsError().WithMessage($"Object prediction with ID {id} not found.").Build(StatusCodeEnum.NotFoundError);
                }

                return ApiResponse.For(objectPrediction).WithMessage("Object prediction retrieved successfully.").Build(StatusCodeEnum.Success);
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<ObjectPredictionDto>().AsError().WithMessage(ex.Message).Build(StatusCodeEnum.BadRequestError);
            }
        }



      
    }
}