using uofi_itp_directory_data.Data;
using uofi_itp_directory_data.DataModels;

namespace uofi_itp_directory_data.DirectoryHook {

    public class QueueManager(DirectoryRepository directoryRepository, DirectoryHookHelper directoryHookHelper) {
        private const int _hoursToWait = 12;
        private readonly DirectoryHookHelper _directoryHookHelper = directoryHookHelper;
        private readonly DirectoryRepository _directoryRepository = directoryRepository;

        public async Task<string> Process(int count) {
            var queue = await _directoryRepository.ReadAsync(d => d.DirectoryEntries.Where(de => de.DateRun == null).OrderByDescending(de => de.IsPriority).Take(count));
            if (queue.Any()) {
                var responseQueue = new List<DirectoryEntry>();
                foreach (var directoryEntry in queue) {
                    responseQueue.Add(await _directoryHookHelper.PopDirectoryEntry(directoryEntry));
                }
                return "Processed queue: " + string.Join("; ", responseQueue.Select(q => q.Summary)) + ".";
            } else {
                var lastRun = await _directoryRepository.ReadAsync(d => d.DirectoryEntries.OrderByDescending(de => de.DateRun).FirstOrDefault(de => de.DateRun != null && !de.IsPriority));
                if (lastRun != null && lastRun.DateRun.HasValue && lastRun.DateRun.Value.AddHours(_hoursToWait) > DateTime.Now) {
                    return $"Queue is empty. Last item run at {lastRun.DateRun.Value:g}. Waiting.";
                } else {
                    var items = await _directoryHookHelper.LoadAreas();
                    return $"Queue is empty. Adding {items} names to queue.";
                }
            }
        }
    }
}