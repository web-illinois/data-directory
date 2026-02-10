using Microsoft.EntityFrameworkCore;
using uofi_itp_directory_data.DataModels;

namespace uofi_itp_directory_data.Data {

    public class DirectoryRepository(IDbContextFactory<DirectoryContext> factory) {
        private readonly IDbContextFactory<DirectoryContext> _factory = factory;

        public int Create<T>(T item) where T : BaseDataItem {
            using var context = _factory.CreateDbContext();
            item.LastUpdated = DateTime.Now;
            context.Add(item);
            return context.SaveChanges();
        }


        public async Task<int> DeleteAllEntries(string netId) {
            using var context = _factory.CreateDbContext();
            _ = await context.Logs.ExecuteDeleteAsync();
            _ = await context.DirectoryEntries.ExecuteDeleteAsync();
            _ = await context.JobProfileTags.ExecuteDeleteAsync();
            _ = await context.JobProfiles.ExecuteDeleteAsync();
            _ = await context.EmployeeActivities.ExecuteDeleteAsync();
            _ = await context.EmployeeCourses.ExecuteDeleteAsync();
            _ = await context.EmployeeHours.ExecuteDeleteAsync();
            _ = await context.Employees.ExecuteDeleteAsync();
            _ = await context.OfficeHours.ExecuteDeleteAsync();
            _ = await context.OfficeSettings.ExecuteDeleteAsync();
            _ = await context.SecurityEntries.ExecuteDeleteAsync();
            _ = await context.Offices.ExecuteDeleteAsync();
            _ = await context.AreaJobTypes.ExecuteDeleteAsync();
            _ = await context.AreaSettings.ExecuteDeleteAsync();
            _ = await context.AreaTags.ExecuteDeleteAsync();
            _ = await context.Areas.ExecuteDeleteAsync();
            _ = await context.SecurityEntries.AddAsync(new SecurityEntry {
                IsActive = true,
                LastUpdated = DateTime.Now,
                Email = netId,
                IsFullAdmin = true,
                IsPublic = true,
                ListedNameFirst = "",
                ListedNameLast = "",
                CanEditAllPeopleInUnit = true
            });
            _ = await context.SaveChangesAsync();
            return 1;
        }

        public async Task<int> CreateAsync<T>(T item) where T : BaseDataItem {
            using var context = _factory.CreateDbContext();
            item.LastUpdated = DateTime.Now;
            context.Add(item);
            return await context.SaveChangesAsync();
        }

        public int Delete<T>(T item) {
            if (item == null) {
                return 0;
            }
            using var context = _factory.CreateDbContext();
            _ = context.Remove(item);
            return context.SaveChanges();
        }

        public async Task<int> DeleteAllDirectoryEntries() {
            using var context = _factory.CreateDbContext();
            return await context.DirectoryEntries.ExecuteDeleteAsync();
        }

        public async Task<int> DeleteAsync<T>(T item) {
            if (item == null) {
                return 0;
            }
            using var context = _factory.CreateDbContext();
            _ = context.Remove(item);
            return await context.SaveChangesAsync();
        }

        public int MakeActive<T>(T item, bool active) where T : BaseDataItem {
            using var context = _factory.CreateDbContext();
            item.LastUpdated = DateTime.Now;
            item.IsActive = active;
            context.Update(item);
            return context.SaveChanges();
        }

        public async Task<int> MakeActiveAsync<T>(T item, bool active) where T : BaseDataItem {
            using var context = _factory.CreateDbContext();
            item.LastUpdated = DateTime.Now;
            item.IsActive = active;
            context.Update(item);
            return await context.SaveChangesAsync();
        }

        public T Read<T>(Func<DirectoryContext, T> work) {
            var context = _factory.CreateDbContext();
            return work(context);
        }

        public async Task<T> ReadAsync<T>(Func<DirectoryContext, T> work) {
            var context = _factory.CreateDbContext();
            return await Task.Run(() => work(context));
        }

        public int Update<T>(T item) where T : BaseDataItem {
            using var context = _factory.CreateDbContext();
            item.LastUpdated = DateTime.Now;
            context.Update(item);
            return context.SaveChanges();
        }

        public async Task<int> UpdateAsync<T>(T item) where T : BaseDataItem {
            using var context = _factory.CreateDbContext();
            item.LastUpdated = DateTime.Now;
            context.Update(item);
            return await context.SaveChangesAsync();
        }
    }
}