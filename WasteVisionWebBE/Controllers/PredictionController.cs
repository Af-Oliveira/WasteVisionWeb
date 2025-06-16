using Microsoft.AspNetCore.Mvc;
using DDDSample1.Domain.Detections;
using System.Threading.Tasks;
using System;
using DDDSample1.Application.Shared;
using DDDSample1.Domain.Shared;
using DDDSample1.Application.Services;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.Predictions;
using System.Collections.Generic;
using DDDSample1.Domain.Application;
using System.Threading.Tasks.Dataflow;

namespace DDDSample1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PredictionController : ControllerBase
    {
        private readonly IDetectionService _detectService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IPredictionService _predictionService;

        public PredictionController(IDetectionService detectService, IAuthenticationService authenticationService, IPredictionService predictionService)
        {
            _detectService = detectService;
            _authenticationService = authenticationService;
            _predictionService = predictionService;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> UploadAndDetectImage([FromForm] CreatingDetectionDto dto)
        {
            
            try
            {
                
               
                var result = await _detectService.UploadAndDetectAsync(
                    dto.File,
                    dto.RoboflowModelId,
                    Request.Scheme,
                    Request.Host
                );
                Console.WriteLine($"Detection result: {result}");
                
                var user = await _authenticationService.GetProfileDtoFromLoggedInUserAsync();

                var createPredictionDto = new CreatingPredictionDto.Builder()
                    .WithUserId(user.Id)
                    .WithModelId(dto.RoboflowModelId)
                    .WithOriginalImageUrl(result.OriginalImageUrl)
                    .WithProcessedImageUrl(result.ProcessedImageUrl)
                    .WithObjectPrediction(result.Predictions)
                    .Build();

                

                var prediction = await _predictionService.CreateAsync(createPredictionDto);

                if (prediction == null)
                {
                    return ApiResponse.For<ImageAnalysisResultDto>()
                        .AsError()
                        .WithMessage("Failed to create prediction.")
                        .Build(StatusCodeEnum.ServerError);
                }

                return ApiResponse.For(prediction)
                    .WithMessage("Image analysis completed successfully.")
                    .Build(StatusCodeEnum.Success);

            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<ImageAnalysisResultDto>()
                    .AsError()
                    .WithMessage(ex.Message)
                    .Build(StatusCodeEnum.BadRequestError);
            }
        }

        // GET: api/Prediction
        [HttpGet]
        public async Task<ActionResult> GetAllPredictions([FromQuery] PredictionSearchParamsDto searchParams)
        {
            Console.WriteLine(searchParams.ModelName);
            Console.WriteLine(searchParams.DateTo);
            Console.WriteLine(searchParams.DateFrom);
            Console.WriteLine(searchParams.UserName);
            try
            {
                var PredictionList = await _predictionService.GetAllWithFiltersAsync(searchParams);
                return ApiResponse.For(PredictionList).WithMessage("Predictions retrieved successfully.").Build(StatusCodeEnum.Success);
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<IEnumerable<PredictionDto>>().AsError().WithMessage(ex.Message).Build(StatusCodeEnum.BadRequestError);
            }
        }


        // GET api/Prediction/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetPrediction(string id)
        {
            try
            {
                var Prediction = await _predictionService.GetByIdAsync(new PredictionId(id));
                if (Prediction == null)
                {

                    return ApiResponse.For<PredictionDto>().AsError().WithMessage($"Prediction with ID {id} not found.").Build(StatusCodeEnum.NotFoundError);
                }

                return ApiResponse.For(Prediction).WithMessage("Prediction retrieved successfully.").Build(StatusCodeEnum.Success);
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<PredictionDto>().AsError().WithMessage(ex.Message).Build(StatusCodeEnum.BadRequestError);
            }
        }


    }
}