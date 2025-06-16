using System.Threading.Tasks;
using System.Collections.Generic;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Logging;
using DDDSample1.Domain.Predictions;
using System;

namespace DDDSample1.Domain.ObjectPredictions
{
    public class ObjectPredictionService : IObjectPredictionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IObjectPredictionRepository _repo;
        private readonly ILogManager _logManager;

        public ObjectPredictionService(IUnitOfWork unitOfWork, IObjectPredictionRepository repo, ILogManager logManager)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
            _logManager = logManager;
        }

        public async Task<ObjectPredictionDto> GetByIdAsync(ObjectPredictionId id)
        {
            var obj = await _repo.GetByIdAsync(id);
            if (obj == null) return null;
            return ObjectPredictionMapper.ToDto(obj);
        }

        public async Task<List<ObjectPredictionDto>> GetByPredictionIdAsync(PredictionId predictionId)
        {
            var list = await _repo.GetByPredictionIdAsync(predictionId);
            return ObjectPredictionMapper.ToDtoList(list);
        }

        public async Task<ObjectPredictionDto> CreateAsync(CreatingObjectPredictionDto dto)
        {
           

            var obj = ObjectPredictionMapper.ToDomain(dto);
            
                await _repo.AddAsync(obj);
                await _unitOfWork.CommitAsync();
                _logManager.Write(LogType.ObjectPrediction, $"Created ObjectPrediction for prediction {dto.PredictionId}");
           
            return ObjectPredictionMapper.ToDto(obj);
        }

        public async Task<List<ObjectPredictionDto>> GetByCategoryAsync(Description category)
        {
            var list = await _repo.GetByCategoryAsync(category);
            return ObjectPredictionMapper.ToDtoList(list);
        }

        public async Task<List<ObjectPredictionDto>> GetAllAsync(ObjectPredictionSearchParamsDto searchParams)
        {
            var modelList = await _repo.GetAllWithFiltersAsync(searchParams);
            return ObjectPredictionMapper.ToDtoList(modelList);
        }
    }
}