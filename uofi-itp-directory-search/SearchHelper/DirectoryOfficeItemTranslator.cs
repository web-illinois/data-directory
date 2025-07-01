using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.Helpers;
using uofi_itp_directory_search.ViewModel;

namespace uofi_itp_directory_search.SearchHelper {

    public static class DirectoryOfficeItemTranslator {

        public static DirectoryOfficeItem? Translate(Office office, IEnumerable<ViewModel.EmployeeCompact> people) {
            var associatedPeople = people.Where(p => p.JobProfiles.Any(j => j.Office == office.Title)).Select(p => EmployeeCompact.TransferPrimaryOfficeAndTitle(p, office.Title)).ToList();
            return associatedPeople.Count == 0
                ? null
                : new DirectoryOfficeItem {
                    Address = office.Address,
                    Building = office.Building,
                    City = office.City,
                    Description = office.Description,
                    State = office.State,
                    Zip = office.ZipCode,
                    Email = office.Email,
                    ExternalUrl = office.ExternalUrl,
                    InternalUrl = office.InternalUrl,
                    TicketUrl = office.TicketUrl,
                    HoursText = office.OfficeHourText,
                    InternalOrder = office.InternalOrder,
                    Map = office.BuildingUrl,
                    OfficeType = office.OfficeType.ToPrettyString(),
                    Phone = office.Phone,
                    Room = office.Room,
                    Title = office.Title,
                    People = [.. associatedPeople.OrderBy(p => p.JobProfiles.FirstOrDefault()?.DisplayOrder).ThenBy(p => p.FullNameReversed)]
                };
        }
    }
}