using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using uofi_itp_directory_data.DataModels;

namespace uofi_itp_directory_data.Data {

    public class DirectoryContext : DbContext {
        private readonly Guid id;

        public DirectoryContext() : base() {
            id = Guid.NewGuid();
            Debug.WriteLine($"{id} context created.");
        }

        public DirectoryContext(DbContextOptions<DirectoryContext> options) : base(options) {
            id = Guid.NewGuid();
            Debug.WriteLine($"{id} context created.");
        }

        public DbSet<AreaJobType> AreaJobTypes { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<AreaSettings> AreaSettings { get; set; }

        public DbSet<AreaTag> AreaTags { get; set; }

        public DbSet<DirectoryEntry> DirectoryEntries { get; set; }

        public DbSet<EmployeeActivity> EmployeeActivities { get; set; }
        public DbSet<EmployeeCourse> EmployeeCourses { get; set; }
        public DbSet<EmployeeHour> EmployeeHours { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public DbSet<JobProfile> JobProfiles { get; set; }

        public DbSet<JobProfileTag> JobProfileTags { get; set; }

        public DbSet<Log> Logs { get; set; }

        public DbSet<OfficeHour> OfficeHours { get; set; }
        public DbSet<Office> Offices { get; set; }
        public DbSet<OfficeSettings> OfficeSettings { get; set; }

        public DbSet<SecurityEntry> SecurityEntries { get; set; }

        public override void Dispose() {
            Debug.WriteLine($"{id} context disposed.");
            base.Dispose();
        }

        public override ValueTask DisposeAsync() {
            Debug.WriteLine($"{id} context disposed async.");
            return base.DisposeAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            Debug.WriteLine($"{id} context starting initial setup.");
            modelBuilder.Entity<SecurityEntry>().HasData(new List<SecurityEntry>
            {
                new("jonker", "Bryan", "Jonker") { Id = -1 },
                new("rbwatson", "Rob", "Watson") { Id = -2 }
            });
            Debug.WriteLine($"{id} context finishing initial setup.");
        }
    }
}