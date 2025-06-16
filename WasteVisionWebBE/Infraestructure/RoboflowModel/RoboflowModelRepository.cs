using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using DDDSample1.Domain.RoboflowModels;
using DDDSample1.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;
using DDDSample1.Domain.Shared;
using System;

namespace DDDSample1.Infrastructure.RoboflowModels
{
    public class RoboflowModelRepository : BaseRepository<RoboflowModel, RoboflowModelId>, IRoboflowModelRepository
    {
        private readonly DDDSample1DbContext _context;
        public RoboflowModelRepository(DDDSample1DbContext context) : base(context.RoboflowModels)
        {
            _context = context;
        }

        public new async Task<RoboflowModel> GetByIdAsync(RoboflowModelId id)
        {
            return await this._context.RoboflowModels
                .Where(x => x.Id.Equals(id))
                .FirstOrDefaultAsync();
        }

        public async Task<List<RoboflowModel>> GetAllActiveAsync()
        {
            return await this._context.RoboflowModels
                .Where(x => x.Active)
                .ToListAsync();
        }

         public async Task<List<RoboflowModel>> GetAllWithFiltersAsync(RoboflowModelSearchParamsDto searchParams)
        {
            var modelsList = await _context.RoboflowModels
                .ToListAsync();

            var filteredModels = modelsList.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchParams.Active))
            {
                bool isActive;
                if (searchParams.Active == "1")
                {
                    isActive = true;
                    Console.WriteLine("Filtering for active models.");
                }
                else
                {
                    isActive = false;
                    Console.WriteLine("Filtering for inactive models.");
                }
                filteredModels = filteredModels.Where(s => s.Active == isActive);
            }
           
            if (!string.IsNullOrWhiteSpace(searchParams.Description))
                filteredModels = filteredModels.Where(s => s.Description.AsString().Contains(searchParams.Description));

             if (!string.IsNullOrWhiteSpace(searchParams.ModelUrl))
                filteredModels = filteredModels.Where(s => s.ModelUrl.AsString().Contains(searchParams.ModelUrl));

            return filteredModels.ToList();
        }
    }
}