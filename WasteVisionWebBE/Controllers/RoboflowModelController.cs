// filepath: DDDSample1/Controllers/RoboflowModelController.cs
using Microsoft.AspNetCore.Mvc;
using DDDSample1.Domain.RoboflowModels;
using DDDSample1.Domain.Shared;
using DDDSample1.Application.Shared; // Assuming ApiResponse is here
using System.Threading.Tasks;
using System.Collections.Generic;
using System; // For IEnumerable

namespace DDDSample1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoboflowModelController : ControllerBase
    {
        private readonly IRoboflowModelService _roboflowModelService;

        public RoboflowModelController(IRoboflowModelService roboflowModelService)
        {
            _roboflowModelService = roboflowModelService;
        }

        // GET: api/roboflowmodel
        [HttpGet]
        public async Task<ActionResult> GetAllModels([FromQuery] RoboflowModelSearchParamsDto searchParams)
        {
            try
            {
                var modelslist = await _roboflowModelService.GetAllWithFiltersAsync(searchParams);
                return ApiResponse.For(modelslist)
                    .WithMessage("Roboflow models retrieved successfully.")
                    .Build(StatusCodeEnum.Success);
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<IEnumerable<RoboflowModelDto>>() // Ensure correct generic type
                    .AsError()
                    .WithMessage(ex.Message)
                    .Build(StatusCodeEnum.BadRequestError);
            }
        }

        // GET: api/roboflowmodel/active
        [HttpGet("active")]
        public async Task<ActionResult> GetAllActiveModels()
        {
            try
            {
                var models = await _roboflowModelService.GetAllActiveAsync();
                return ApiResponse.For(models)
                    .WithMessage("Active Roboflow models retrieved successfully.")
                    .Build(StatusCodeEnum.Success);
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<List<RoboflowModelDto>>()
                    .AsError()
                    .WithMessage(ex.Message)
                    .Build(StatusCodeEnum.BadRequestError);
            }
            catch (System.Exception ex)
            {
                // Log the exception ex
                return ApiResponse.For<List<RoboflowModelDto>>()
                    .AsError()
                    .WithMessage("An unexpected error occurred while retrieving active models.")
                    .Build(StatusCodeEnum.ServerError);
            }
        }

        // GET api/roboflowmodel/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult> GetModel(string id)
        {
            try
            {
                var model = await _roboflowModelService.GetByIdAsync(new RoboflowModelId(id));
                if (model == null)
                {
                    return ApiResponse.For<RoboflowModelDto>()
                        .AsError()
                        .WithMessage($"Roboflow model with ID {id} not found.")
                        .Build(StatusCodeEnum.NotFoundError);
                }

                return ApiResponse.For(model)
                    .WithMessage("Roboflow model retrieved successfully.")
                    .Build(StatusCodeEnum.Success);
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<RoboflowModelDto>()
                    .AsError()
                    .WithMessage(ex.Message)
                    .Build(StatusCodeEnum.BadRequestError);
            }
            catch (System.Exception ex)
            {
                // Log the exception ex
                return ApiResponse.For<RoboflowModelDto>()
                    .AsError()
                    .WithMessage($"An unexpected error occurred while retrieving model with ID {id}.")
                    .Build(StatusCodeEnum.ServerError);
            }
        }

        // POST api/roboflowmodel
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> CreateModel([FromForm] CreatingRoboflowModelDto dto)
        {

            try
            {
                var result = await _roboflowModelService.CreateAsync(dto);
                // Assuming CreateAsync throws an exception on failure to retrieve after creation,
                // or returns null if some other pre-condition fails (though throwing is often better).
                if (result == null)
                {
                    return ApiResponse.For<RoboflowModelDto>()
                       .AsError()
                       .WithMessage("Failed to create Roboflow model. The service returned null.")
                       .Build(StatusCodeEnum.ServerError); // Or BadRequest if it's a known client error
                }

                return ApiResponse.For(result)
                    .WithMessage("Roboflow model created successfully.")
                    // Consider returning CreatedAtAction for POST requests
                    // .Build(StatusCodeEnum.Created); // Or use CreatedAtAction
                    .Build(StatusCodeEnum.Success); // Using Success as per your existing pattern
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<RoboflowModelDto>()
                    .AsError()
                    .WithMessage(ex.Message)
                    .Build(StatusCodeEnum.BadRequestError);
            }
            catch (System.Exception ex) // Catch more general exceptions from the service layer
            {
                // Log the exception (ex)
                return ApiResponse.For<RoboflowModelDto>()
                    .AsError()
                    .WithMessage($"An unexpected error occurred: {ex.Message}") // Provide more context if safe
                    .Build(StatusCodeEnum.ServerError);
            }
        }

        // PUT api/roboflowmodel/{id}
        [HttpPut("{id}")]
        // Use [FromForm] to correctly bind IFormFile and other properties
        public async Task<ActionResult> UpdateModel(string id, [FromForm] UpdatingRoboflowModelDto dto)
        {
            if (!ModelState.IsValid) // Optional: Add model state validation
            {
                 return ApiResponse.For<RoboflowModelDto>()
                    .AsError()
                    .WithMessage("Invalid model data provided for update.")
                    .Build(StatusCodeEnum.BadRequestError);
            }
            try
            {
                var result = await _roboflowModelService.UpdateAsync(id, dto);
                if (result == null)
                {
                    return ApiResponse.For<RoboflowModelDto>()
                        .AsError()
                        .WithMessage($"Roboflow model with ID {id} not found or update failed.")
                        .Build(StatusCodeEnum.NotFoundError);
                }

                return ApiResponse.For(result)
                    .WithMessage("Roboflow model updated successfully.")
                    .Build(StatusCodeEnum.Success);
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<RoboflowModelDto>()
                    .AsError()
                    .WithMessage(ex.Message)
                    .Build(StatusCodeEnum.BadRequestError);
            }
            catch (System.Exception ex) // Catch more general exceptions
            {
                // Log the exception (ex)
                return ApiResponse.For<RoboflowModelDto>()
                    .AsError()
                    .WithMessage($"An unexpected error occurred during update: {ex.Message}")
                    .Build(StatusCodeEnum.ServerError);
            }
        }

        // DELETE api/roboflowmodel/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteModel(string id)
        {
            try
            {
                var result = await _roboflowModelService.DeleteAsync(new RoboflowModelId(id));
                if (!result) // DeleteAsync returns bool
                {
                    return ApiResponse.For<bool>()
                        .AsError()
                        .WithMessage($"Roboflow model with ID {id} not found or deletion failed.")
                        .Build(StatusCodeEnum.NotFoundError);
                }

                return ApiResponse.For(result) // result will be true here
                    .WithMessage("Roboflow model deleted successfully.")
                    .Build(StatusCodeEnum.Success);
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<bool>()
                    .AsError()
                    .WithMessage(ex.Message)
                    .Build(StatusCodeEnum.BadRequestError);
            }
            catch (System.Exception ex)
            {
                // Log the exception ex
                return ApiResponse.For<bool>()
                    .AsError()
                    .WithMessage($"An unexpected error occurred during deletion: {ex.Message}")
                    .Build(StatusCodeEnum.ServerError);
            }
        }

        // PATCH api/roboflowmodel/activate/{id}
        [HttpPatch("activate/{id}")]
        public async Task<ActionResult> ActivateModel(string id)
        {
            try
            {
                var success = await _roboflowModelService.ActivateAsync(new RoboflowModelId(id));
                if (!success) // ActivateAsync returns bool
                {
                    return ApiResponse.For<bool>() // Return type is bool
                        .AsError()
                        .WithMessage($"Roboflow model with ID {id} not found or activation failed.")
                        .Build(StatusCodeEnum.NotFoundError);
                }

                return ApiResponse.For(success) // success will be true
                    .WithMessage("Roboflow model activated successfully.")
                    .Build(StatusCodeEnum.Success);
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<bool>()
                    .AsError()
                    .WithMessage(ex.Message)
                    .Build(StatusCodeEnum.BadRequestError);
            }
            catch (System.Exception ex)
            {
                // Log the exception ex
                return ApiResponse.For<bool>()
                    .AsError()
                    .WithMessage($"An unexpected error occurred during activation: {ex.Message}")
                    .Build(StatusCodeEnum.ServerError);
            }
        }

        // PATCH api/roboflowmodel/deactivate/{id}
        [HttpPatch("deactivate/{id}")]
        public async Task<ActionResult> DeactivateModel(string id)
        {
            try
            {
                var success = await _roboflowModelService.DeactivateAsync(new RoboflowModelId(id));
                if (!success) // DeactivateAsync returns bool
                {
                    return ApiResponse.For<bool>() // Return type is bool
                        .AsError()
                        .WithMessage($"Roboflow model with ID {id} not found or deactivation failed.")
                        .Build(StatusCodeEnum.NotFoundError);
                }

                return ApiResponse.For(success) // success will be true
                    .WithMessage("Roboflow model deactivated successfully.")
                    .Build(StatusCodeEnum.Success);
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<bool>()
                    .AsError()
                    .WithMessage(ex.Message)
                    .Build(StatusCodeEnum.BadRequestError);
            }
            catch (System.Exception ex)
            {
                // Log the exception ex
                return ApiResponse.For<bool>()
                    .AsError()
                    .WithMessage($"An unexpected error occurred during deactivation: {ex.Message}")
                    .Build(StatusCodeEnum.ServerError);
            }
        }
    }
}
