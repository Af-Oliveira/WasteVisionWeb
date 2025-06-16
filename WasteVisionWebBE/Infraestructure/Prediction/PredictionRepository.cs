using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDDSample1.Domain.Predictions;
using DDDSample1.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.RoboflowModels;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Infrastructure.Predictions
{
    public class PredictionRepository : BaseRepository<Prediction, PredictionId>, IPredictionRepository
    {
        private readonly DDDSample1DbContext _context;

        public PredictionRepository(DDDSample1DbContext context) : base(context.Predictions)
        {
            _context = context;
        }

        public new async Task<Prediction> GetByIdAsync(PredictionId id)
        {
            return await _context.Predictions
                .Include(p => p.User)
                .Include(p => p.RoboflowModel)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Prediction>> GetByUserIdAsync(UserId userId)
        {
            return await _context.Predictions
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.Date)
                .ToListAsync();
        }

        public async Task<List<Prediction>> GetByModelIdAsync(RoboflowModelId modelId)
        {
            return await _context.Predictions
                .Where(p => p.RoboflowModelId == modelId)
                .OrderByDescending(p => p.Date)
                .ToListAsync();
        }

        public async Task<List<Prediction>> GetAllWithFiltersAsync(PredictionSearchParamsDto searchParams)
        {
            // Load all predictions with related data first
            var predictionsList = await _context.Predictions
                .Include(s => s.User)
                .Include(s => s.RoboflowModel)
                .Include(s => s.ObjectPredictions)
                .ToListAsync();

            // Apply filters in memory using LINQ to Objects
            var filteredPredictions = predictionsList.AsQueryable();

            
            if (!string.IsNullOrWhiteSpace(searchParams.UserId))
            {
                filteredPredictions = filteredPredictions.Where(s =>
                    s.User.Id.AsString().Contains(searchParams.UserId));
            }

            if (!string.IsNullOrWhiteSpace(searchParams.ModelName))
            {
                filteredPredictions = filteredPredictions.Where(s =>
                    s.RoboflowModel.Description.AsString().Contains(searchParams.ModelName));
            }

            if (!string.IsNullOrWhiteSpace(searchParams.UserName))
            {
                filteredPredictions = filteredPredictions.Where(s =>
                    s.User.Username.Value.Contains(searchParams.UserName));
            }

            if (!string.IsNullOrWhiteSpace(searchParams.DateFrom))
            {
                Date startDate = new Date(searchParams.DateFrom);
                filteredPredictions = filteredPredictions.Where(s =>
                    s.Date.IsAfter(startDate) || s.Date.Value == startDate.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchParams.DateTo))
            {
                Date endDate = new Date(searchParams.DateTo);
                filteredPredictions = filteredPredictions.Where(s =>
                    s.Date.IsBefore(endDate) || s.Date.Value == endDate.Value);
            }

            return filteredPredictions
                .OrderByDescending(p => p.Date.Value)
                .ToList();
        }
    }
}