// filepath: DDDSample1/Domain/RoboflowModels/RoboflowModelService.cs
using DDDSample1.Domain.Shared;
using System.Threading.Tasks;
using System.Collections.Generic;
using DDDSample1.Domain.Logging;
using System;
using DDDSample1.Domain.Roles; // Assuming this is for IExceptionHandler or other dependencies
using Microsoft.AspNetCore.Hosting; // Required for IWebHostEnvironment
using System.IO; // Required for Path operations
using Microsoft.AspNetCore.Http; // Required for IFormFile

namespace DDDSample1.Domain.RoboflowModels
{
    public class RoboflowModelService : IRoboflowModelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoboflowModelRepository _repo;
        private readonly ILogManager _logManager;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IRoboflowModelInfoService _modelInfoService;
        private readonly IWebHostEnvironment _webHostEnvironment; // For file paths

        // Relative path to the folder where models will be stored
        private const string ModelsStoragePath = "PythonBE/models"; 

        public RoboflowModelService(
            IUnitOfWork unitOfWork,
            IRoboflowModelRepository repo,
            ILogManager logManager,
            IRoboflowModelInfoService modelInfoService,
            IExceptionHandler exceptionHandler,
            IWebHostEnvironment webHostEnvironment) // Inject IWebHostEnvironment
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
            _logManager = logManager;
            _modelInfoService = modelInfoService;
            _exceptionHandler = exceptionHandler;
            _webHostEnvironment = webHostEnvironment;
        }

        // ... (GetAllAsync, GetAllActiveAsync, GetByIdAsync, CreateAsync remain the same) ...
        public async Task<List<RoboflowModelDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return RoboflowModelMapper.ToDtoList(list);
        }

        public async Task<List<RoboflowModelDto>> GetAllActiveAsync()
        {
            var list = await _repo.GetAllActiveAsync();
            return RoboflowModelMapper.ToDtoList(list);
        }

        public async Task<RoboflowModelDto> GetByIdAsync(RoboflowModelId id)
        {
            var model = await _repo.GetByIdAsync(id);
            if (model == null)
                return null;
            return RoboflowModelMapper.ToDto(model);
        }
        
        public async Task<RoboflowModelDto> CreateAsync(CreatingRoboflowModelDto dto)
        {
            _logManager.Write(LogType.RoboflowModel, $"Attempting to create model: {dto.Description}");
            try
            {
                string savedModelRelativePath = null;
                if (dto.ModelFile != null && dto.ModelFile.Length > 0)
                {
                    var targetDirectory = Path.GetFullPath(Path.Combine(_webHostEnvironment.ContentRootPath, ModelsStoragePath));
                    if (!Directory.Exists(targetDirectory))
                    {
                        Directory.CreateDirectory(targetDirectory);
                        _logManager.Write(LogType.Info, $"Created directory: {targetDirectory}");
                    }
                    _logManager.Write(LogType.Info, $"target directory: {targetDirectory}");

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ModelFile.FileName);
                    var absoluteFilePath = Path.Combine(targetDirectory, uniqueFileName);

                    _logManager.Write(LogType.Info, $"Saving model file to: {absoluteFilePath}");
                    await using (var stream = new FileStream(absoluteFilePath, FileMode.Create))
                    {
                        await dto.ModelFile.CopyToAsync(stream);
                    }
                    _logManager.Write(LogType.Info, $"Successfully saved model file: {uniqueFileName}");

                
                    dto.LocalModelPath = absoluteFilePath;
                }
                else
                {
                    _logManager.Write(LogType.Info, "No model file provided for local storage.");
                    dto.LocalModelPath = "None";
                }
 
                    _logManager.Write(LogType.Info, $"Fetching model info from Roboflow: {dto.ModelUrl}");
                    var modelInfo = await _modelInfoService.GetModelInfoAsync(dto.ModelUrl, dto.ApiKey);
                    dto.EndPoint = modelInfo.Version.Model.Endpoint;
                    dto.Map = modelInfo.Version.Model.Map.ToString(); 
                    dto.Recall = modelInfo.Version.Model.Recall.ToString();
                    dto.Precision = modelInfo.Version.Model.Precision.ToString();
                    _logManager.Write(LogType.Info, "Successfully fetched model info from Roboflow.");


                var modelEntity = RoboflowModelMapper.ToDomain(dto); 
                
                await _repo.AddAsync(modelEntity);
                await _unitOfWork.CommitAsync();
                _logManager.Write(LogType.RoboflowModel, $"Successfully added model to repository with ID: {modelEntity.Id}");

                var createdModel = await _repo.GetByIdAsync(modelEntity.Id); 
                if (createdModel == null)
                {
                    _logManager.Write(LogType.Error, $"Failed to retrieve created model with ID: {modelEntity.Id}");
                    throw new InvalidOperationException("Failed to retrieve the model after creation.");
                }
                _logManager.Write(LogType.RoboflowModel, $"Created model '{createdModel.Description}' with local path '{createdModel.LocalModelPath.AsString() ?? "N/A"}'");

                return RoboflowModelMapper.ToDto(createdModel);
            }
            catch (Exception ex)
            {
                _logManager.Write(LogType.Error, $"Failed to create model: {ex.Message}{Environment.NewLine}StackTrace: {ex.StackTrace}");
                _exceptionHandler.HandleException(ex); 
                throw; 
            }
        }

        public async Task<RoboflowModelDto> UpdateAsync(string id, UpdatingRoboflowModelDto dto)
        {
            var modelId = new RoboflowModelId(id);
            var model = await _repo.GetByIdAsync(modelId);
            if (model == null)
            {
                _logManager.Write(LogType.Error, $"Failed to update model. Model with ID {id} not found.");
                return null;
            }

            _logManager.Write(LogType.Info, $"Updating model with ID {id}. Current Description: {model.Description}, Current Local Path: {model.LocalModelPath.AsString() ?? "N/A"}");

            var isModified = false;
            // 1. Handle New File Upload if present
            if (dto.ModelFile != null && dto.ModelFile.Length > 0)
            {

                _logManager.Write(LogType.Info, $"New model file provided: {dto.ModelFile.FileName}");


                // Save the new file
                var targetDirectory = Path.GetFullPath(Path.Combine(_webHostEnvironment.ContentRootPath, ModelsStoragePath));
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                    _logManager.Write(LogType.Info, $"Created directory for new model file: {targetDirectory}");
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ModelFile.FileName);
                var newAbsoluteFilePath = Path.Combine(targetDirectory, uniqueFileName);

                _logManager.Write(LogType.Info, $"Saving new model file to: {newAbsoluteFilePath}");
                await using (var stream = new FileStream(newAbsoluteFilePath, FileMode.Create))
                {
                    await dto.ModelFile.CopyToAsync(stream);
                }
                _logManager.Write(LogType.Info, $"Successfully saved new model file: {uniqueFileName}");
                                             
                model.ChangeLocalModelUrl(new FilePath(newAbsoluteFilePath)); // Assuming your domain model has this method
                isModified = true;
            }


            // 2. Fetch new info from Roboflow if URL/API key changed
            // Check if ModelUrl or ApiKey are actually different from the current model's values
            bool shouldFetchRoboflowInfo = false;
            if (!string.IsNullOrWhiteSpace(dto.ModelUrl) && !model.ModelUrl.Value.Equals(dto.ModelUrl, StringComparison.OrdinalIgnoreCase))
            {
                shouldFetchRoboflowInfo = true;
            }
            if (!string.IsNullOrWhiteSpace(dto.ApiKey) && !model.ApiKey.Value.Equals(dto.ApiKey, StringComparison.OrdinalIgnoreCase))
            {
                shouldFetchRoboflowInfo = true;
            }

            if (shouldFetchRoboflowInfo)
            {
                _logManager.Write(LogType.Info, "ModelUrl or ApiKey changed, fetching new model info from Roboflow.");
                // Use new DTO values if provided, otherwise fall back to existing model values
                var urlToFetch = !string.IsNullOrWhiteSpace(dto.ModelUrl) ? dto.ModelUrl : model.ModelUrl.Value;
                var apiKeyToFetch = !string.IsNullOrWhiteSpace(dto.ApiKey) ? dto.ApiKey : model.ApiKey.Value;

                var modelInfo = await _modelInfoService.GetModelInfoAsync(urlToFetch, apiKeyToFetch);
                
                // Compare and update Roboflow specific fields
                if (modelInfo.Version.Model.Endpoint != model.EndPoint.Value)
                {
                    model.ChangeEndPoint(new Url(modelInfo.Version.Model.Endpoint));
                    isModified = true;
                }
                if (modelInfo.Version.Model.Map.ToString() != model.Map.Value.ToString()) 
                {
                    model.ChangeMap(new NumberDouble(modelInfo.Version.Model.Map));
                    isModified = true;
                }
                if (modelInfo.Version.Model.Recall.ToString() != model.Recall.Value.ToString())
                {
                    model.ChangeRecall(new NumberDouble(modelInfo.Version.Model.Recall));
                    isModified = true;
                }
                if (modelInfo.Version.Model.Precision.ToString() != model.Precision.Value.ToString())
                {
                    model.ChangePrecision(new NumberDouble(modelInfo.Version.Model.Precision));
                    isModified = true;
                }
            }

            // 3. Update other properties
            if (!string.IsNullOrWhiteSpace(dto.Description) && model.Description.Value != dto.Description)
            {
                model.ChangeDescription(new Description(dto.Description));
                isModified = true;
            }

            if (!string.IsNullOrWhiteSpace(dto.ApiKey) && model.ApiKey.Value != dto.ApiKey)
            {
                model.ChangeApiKey(new ApiKey(dto.ApiKey));
                isModified = true; // Already covered by shouldFetchRoboflowInfo if it leads to a fetch
            }

            if (!string.IsNullOrWhiteSpace(dto.ModelUrl) && model.ModelUrl.Value != dto.ModelUrl)
            {
                model.ChangeModelUrl(new Url(dto.ModelUrl));
                isModified = true; // Already covered by shouldFetchRoboflowInfo if it leads to a fetch
            }


            if (isModified)
            {
                _logManager.Write(LogType.Info, $"Committing changes for model ID {id}.");
                await _unitOfWork.CommitAsync();
            }
            else
            {
                _logManager.Write(LogType.Info, $"No changes detected or committed for model ID {id}.");
            }

            _logManager.Write(LogType.RoboflowModel, $"Update process completed for model with ID {model.Id.ToString()}");
            return RoboflowModelMapper.ToDto(model); 
        }


        public async Task<bool> DeleteAsync(RoboflowModelId modelId)
        {
            var model = await _repo.GetByIdAsync(modelId);
            if (model == null)
            {
                _logManager.Write(LogType.Error, $"Failed to delete model. Model with ID {modelId} not found.");
                return false;
            }

            if (!string.IsNullOrEmpty(model.LocalModelPath.AsString()))
            {
                var filePath = Path.GetFullPath(Path.Combine(_webHostEnvironment.ContentRootPath, model.LocalModelPath.AsString()));
                try
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        _logManager.Write(LogType.Info, $"Deleted local model file: {filePath}");
                    }
                    else
                    {
                        _logManager.Write(LogType.Info, $"Local model file not found for deletion: {filePath}");
                    }
                }
                catch (IOException ex)
                {
                    _logManager.Write(LogType.Error, $"Error deleting local model file {filePath}: {ex.Message}");
                }
            }
            
            _logManager.Write(LogType.RoboflowModel, $"Deleting model with name {model.Description.ToString()} and ID {model.Id}");
            _repo.Remove(model);
            await _unitOfWork.CommitAsync();
            _logManager.Write(LogType.RoboflowModel, $"Successfully deleted model from repository.");
            return true;
        }

        public async Task<List<RoboflowModelDto>> GetAllWithFiltersAsync(RoboflowModelSearchParamsDto searchParams)
        {
            var modelList = await _repo.GetAllWithFiltersAsync(searchParams);
            return RoboflowModelMapper.ToDtoList(modelList);
        }

        public async Task<bool> ActivateAsync(RoboflowModelId id)
        {
            var model = await _repo.GetByIdAsync(id);
            if (model == null)
            {
                _logManager.Write(LogType.Error, $"Failed to activate model. Model with ID {id} not found.");
                return false;
            }

            model.Activate();
            await _unitOfWork.CommitAsync();
            _logManager.Write(LogType.RoboflowModel, $"Activated model with Name {model.Description.ToString()}");
            return true;
        }

        public async Task<bool> DeactivateAsync(RoboflowModelId id)
        {
            var model = await _repo.GetByIdAsync(id);
            if (model == null)
            {
                _logManager.Write(LogType.Error, $"Failed to deactivate model. Model with ID {id} not found.");
                return false;
            }

            model.Deactivate();
            await _unitOfWork.CommitAsync();
            _logManager.Write(LogType.RoboflowModel, $"Deactivated model with name {model.Description.ToString()}");
            return true;
        }
    }
}
