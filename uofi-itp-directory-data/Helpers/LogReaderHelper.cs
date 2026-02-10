using uofi_itp_directory_data.Data;
using uofi_itp_directory_data.DataModels;

namespace uofi_itp_directory_data.Helpers {
    public class LogReaderHelper(DirectoryRepository directoryRepository) {
        private readonly DirectoryRepository _directoryRepository = directoryRepository;

        public async Task<List<Log>> GetLogs(string filter) => string.IsNullOrWhiteSpace(filter) ?
                [.. (await _directoryRepository.ReadAsync(d => d.Logs.OrderByDescending(a => a.LastUpdated).Take(1000)))] :
                [.. (await _directoryRepository.ReadAsync(d => d.Logs.Where(l => l.SubjectText.Contains(filter) || l.ChangeType.Contains(filter) || l.ChangedByNetId.Contains(filter)).OrderByDescending(a => a.LastUpdated).Take(1000)))];
    }
}
