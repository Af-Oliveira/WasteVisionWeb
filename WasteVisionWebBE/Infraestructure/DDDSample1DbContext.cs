using Microsoft.EntityFrameworkCore;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.Roles;
using DDDSample1.Infrastructure.Users;
using DDDSample1.Infrastructure.Roles;
using DDDSample1.Infrastructure.RoboflowModels;
using DDDSample1.Domain.RoboflowModels;
using DDDSample1.Infrastructure.Predictions;
using DDDSample1.Domain.Predictions;
using DDDSample1.Domain.ObjectPredictions;
using DDDSample1.Infrastructure.ObjectPredictions;

namespace DDDSample1.Infrastructure
{
    public class DDDSample1DbContext : DbContext
    {
        public DbSet<Role> Roles { get; set; }
        public DbSet<ObjectPrediction> ObjectPredictions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RoboflowModel> RoboflowModels { get; set; }
        public DbSet<Prediction> Predictions { get; set; }
        public DDDSample1DbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ObjectPredictionEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RoleEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PredictionEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RoboflowModelEntityTypeConfiguration());
        }
    }
}
