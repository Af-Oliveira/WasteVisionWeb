using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDDSample1.Domain.ObjectPredictions;
using DDDSample1.Domain.Predictions;
using DDDSample1.Domain.Shared;
using DDDSample1.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace DDDSample1.Infrastructure.ObjectPredictions
{
    public class ObjectPredictionRepository : BaseRepository<ObjectPrediction, ObjectPredictionId>, IObjectPredictionRepository
    {
        private readonly DDDSample1DbContext _context;

        public ObjectPredictionRepository(DDDSample1DbContext context) : base(context.ObjectPredictions)
        {
            _context = context;
        }

        public new async Task<ObjectPrediction> GetByIdAsync(ObjectPredictionId id)
        {
            return await _context.ObjectPredictions
                .Include(p => p.Prediction)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<ObjectPrediction>> GetByPredictionIdAsync(PredictionId predictionId)
        {
            return await _context.ObjectPredictions
                .Where(p => p.PredictionId == predictionId)
                .OrderByDescending(p => p.Confidence)
                .ToListAsync();
        }

        public async Task<List<ObjectPrediction>> GetByCategoryAsync(Description category)
        {
            return await _context.ObjectPredictions
                .Where(p => p.Category == category)
                .OrderByDescending(p => p.Confidence)
                .ToListAsync();
        }

        public async Task<List<ObjectPrediction>> GetAllWithFiltersAsync(ObjectPredictionSearchParamsDto searchParams)
        {

             var ObjectPredictionList = await _context.ObjectPredictions
                .ToListAsync();

             // Apply filters in memory using LINQ to Objects
            var filteredObjectPredictions = ObjectPredictionList.AsQueryable();


            if (!string.IsNullOrWhiteSpace(searchParams.predictionId))
            {
                var predictionId = new PredictionId(searchParams.predictionId);
                filteredObjectPredictions = filteredObjectPredictions.Where(s => s.PredictionId == predictionId);
            }

            if (!string.IsNullOrWhiteSpace(searchParams.precisionFrom) || !string.IsNullOrWhiteSpace(searchParams.precisionTo))
            {
                double? startPrecision = double.Parse(searchParams.precisionFrom);
                double? endPrecision = double.Parse(searchParams.precisionTo);

                if (startPrecision.HasValue)
                {
                    filteredObjectPredictions = filteredObjectPredictions.Where(s => s.Confidence.Value >= startPrecision.Value);
                }

                if (endPrecision.HasValue)
                {
                    filteredObjectPredictions = filteredObjectPredictions.Where(s => s.Confidence.Value <= endPrecision.Value);
                }
            }

            return  filteredObjectPredictions.ToList();
        }
    
    }
}