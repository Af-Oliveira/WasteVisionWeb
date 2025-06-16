using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.Logging;
using DDDSample1.Domain.RoboflowModels;
using DDDSample1.Domain.ObjectPredictions;
using System.Threading.Tasks.Dataflow;
using System;
using System.ComponentModel.DataAnnotations;

namespace DDDSample1.Domain.Predictions
{
    public class PredictionService : IPredictionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPredictionRepository _repo;
        private readonly IObjectPredictionService _objectPredictionService;
        private readonly ILogManager _logManager;

        public PredictionService(IUnitOfWork unitOfWork, IPredictionRepository repo, ILogManager logManager, IObjectPredictionService objectPredictionService)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
            _objectPredictionService = objectPredictionService;
            _logManager = logManager;
        }

        public async Task<List<PredictionDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return PredictionMapper.ToDtoList(list);
        }

        public async Task<PredictionDto> GetByIdAsync(PredictionId id)
        {
            var pred = await _repo.GetByIdAsync(id);
            if (pred == null) return null;
            return PredictionMapper.ToDto(pred);
        }

        public async Task<List<PredictionDto>> GetByUserIdAsync(UserId userId)
        {
            var list = await _repo.GetByUserIdAsync(userId);
            return PredictionMapper.ToDtoList(list);
        }

        public async Task<List<PredictionDto>> GetByModelIdAsync(RoboflowModelId modelId)
        {
            var list = await _repo.GetByModelIdAsync(modelId);
            return PredictionMapper.ToDtoList(list);
        }

        public async Task<PredictionDto> CreateAsync(CreatingPredictionDto dto)
        {
            try
            {
                
                //do console wwrite to see the content of the dto
                var prediction = PredictionMapper.ToDomain(dto);

                await _repo.AddAsync(prediction);
                await _unitOfWork.CommitAsync();
                 _logManager.Write(LogType.Prediction, $"Created prediction for user {dto.UserId}");

                var predictions = dto.ObjectPrediction.Predictions;

                // Dictionary to store category counts
                Dictionary<string, int> categoryCounts = new Dictionary<string, int>();

                foreach (var pred in predictions)
                {
                    var objectPredictionDto = new CreatingObjectPredictionDto.Builder()
                        .WithPredictionId(prediction.Id.AsString())
                        .WithX(pred.X)
                        .WithY(pred.Y)
                        .WithWidth(pred.Width)
                        .WithHeight(pred.Height)
                        .WithCategory(pred.Class.ToString())
                        .WithConfidence(pred.Confidence)
                        .Build();

                   var objectPrediction =  await _objectPredictionService.CreateAsync(objectPredictionDto);
                }

               
                return PredictionMapper.ToDto(prediction);
            }
            catch (BusinessRuleValidationException ex)
            {
                _logManager.Write(LogType.Error, $"Failed to create prediction: {ex.Message}");
                throw;
            }
        }

       

        public async Task<List<PredictionApiDto>> GetAllWithFiltersAsync(PredictionSearchParamsDto searchParams)
        {
            var list = await _repo.GetAllWithFiltersAsync(searchParams);
            return PredictionMapper.ToApiDtoList(list);
        }
    }
}