using uofi_itp_directory_external.DataWarehouse;
using uofi_itp_directory_external.Experts;
using uofi_itp_directory_search.ViewModel;
using dM = uofi_itp_directory_data.DataModels;
using pC = uofi_itp_directory_external.ProgramCourse;

namespace uofi_itp_directory_search.LoadHelper {

    public static class EmployeeTranslator {

        public static Employee Translate(DataWarehouseItem dataWarehouseItem, dM.Employee directoryEmployee, string imageUrl, IEnumerable<pC.Course> dataModelCourses, ExpertsProfile expertsProfile, string source) => new() {
            AlternateContact = directoryEmployee.AlternateContact,
            AlternateEmail = directoryEmployee.AlternateContactEmail,
            AlternatePhone = directoryEmployee.AlternateContactPhone,
            AddressLine1 = directoryEmployee.IsAddressHidden ? "" : ChooseFirstNonBlank(directoryEmployee.AddressLine1, dataWarehouseItem.AddressLine1),
            AddressLine2 = directoryEmployee.IsAddressHidden ? "" : ChooseFirstNonBlank(directoryEmployee.AddressLine2, dataWarehouseItem.AddressLine2),
            Biography = ChooseFirstNonBlank(expertsProfile.Biography, directoryEmployee.Biography),
            Building = ChooseFirstNonBlank(directoryEmployee.Building, dataWarehouseItem.Building),
            City = directoryEmployee.IsAddressHidden ? "" : ChooseFirstNonBlank(directoryEmployee.City, dataWarehouseItem.City),
            CvUrl = directoryEmployee.CVUrl,
            Email = directoryEmployee.UseAlternateContactAsPrimary && !string.IsNullOrWhiteSpace(directoryEmployee.AlternateContactEmail) ? directoryEmployee.AlternateContactEmail : directoryEmployee.NetIdTruncated + "@illinois.edu",
            ExpertsUrl = expertsProfile.ExpertsUrl,
            FirstName = ChooseFirstNonBlank(directoryEmployee.PreferredNameFirst, dataWarehouseItem.FirstName),
            Hours = directoryEmployee.EmployeeHourText,
            ImageAltText = directoryEmployee.PhotoAltText,
            ImageUrl = imageUrl,
            LastName = ChooseFirstNonBlank(directoryEmployee.PreferredNameLast, dataWarehouseItem.LastName),
            LinkName = directoryEmployee.NameLinked,
            LinkedInUrl = ChooseFirstNonBlank(expertsProfile.LinkedIn, directoryEmployee.EmployeeActivities.FirstOrDefault(a => a.Type == dM.ActivityTypeEnum.Link && a.Url.ToLower().Contains("linkedin.com"))?.Url),
            LastUpdated = DateTime.Now,
            NetId = directoryEmployee.NetIdTruncated,
            Phone = directoryEmployee.UseAlternateContactAsPrimary ? directoryEmployee.AlternateContactPhone : directoryEmployee.IsPhoneHidden ? "" : ChooseFirstNonBlank(directoryEmployee.Phone, dataWarehouseItem.PhoneFull),
            PreferredPronouns = directoryEmployee.PreferredPronouns,
            PrimaryOffice = directoryEmployee.PrimaryJobProfile.Office.Title,
            PrimaryTitle = directoryEmployee.PrimaryJobProfile.Title,
            ProfileUrl = directoryEmployee.ProfileUrl,
            RoomNumber = directoryEmployee.Room,
            Source = source,
            State = directoryEmployee.IsAddressHidden ? "" : ChooseFirstNonBlank(directoryEmployee.State, dataWarehouseItem.State),
            Quote = expertsProfile.Quote,
            ResearchStatement = expertsProfile.ResearchStatement,
            Tags = directoryEmployee.PrimaryJobProfile.Tags.Select(t => t.Title).ToList() ?? [],
            TeachingStatement = expertsProfile.TeachingStatement,
            TwitterName = expertsProfile.Twitter,
            Uin = dataWarehouseItem.Uin,
            Zip = directoryEmployee.IsAddressHidden ? "" : ChooseFirstNonBlank(directoryEmployee.ZipCode, dataWarehouseItem.ZipCode),
            Awards = TranslateAwards(directoryEmployee.EmployeeActivities, expertsProfile.Awards),
            Organizations = TranslateOrganizations(directoryEmployee.EmployeeActivities, expertsProfile.Organizations),
            Courses = TranslateCourses(directoryEmployee.EmployeeCourses, dataModelCourses),
            EducationHistory = TranslateEducationHistory(directoryEmployee.EmployeeActivities, expertsProfile.EducationHistory),
            Grants = TranslateGrants(expertsProfile.Grants),
            JobProfiles = TranslateJobProfiles(directoryEmployee.JobProfiles),
            Keywords = expertsProfile.Keywords,
            Links = TranslateLinks(directoryEmployee.EmployeeActivities, expertsProfile.Links),
            News = TranslateNews(expertsProfile.Clippings),
            Presentations = TranslatePresentations(directoryEmployee.EmployeeActivities, expertsProfile.Presentations),
            Publications = TranslatePublications(directoryEmployee.EmployeeActivities, expertsProfile.Publications),
            Services = TranslateServices(expertsProfile.Services),
        };

        private static string ChooseFirstNonBlank(params string?[] args) => args.FirstOrDefault(a => !string.IsNullOrWhiteSpace(a)) ?? "";

        private static List<DatedItem> TranslateAwards(IEnumerable<dM.EmployeeActivity> directoryActivites, IEnumerable<ExpertsItem> experts) => experts != null && experts.Any()
                ? [.. experts.Select(a => new DatedItem { DisplayOrder = a.SortOrder, IsHighlighted = a.IsHighlighted, Title = a.TitleFull, Url = a.Url, Year = a.Year })]
                : [.. directoryActivites.Where(da => da.Type == dM.ActivityTypeEnum.Award).Select(da => new DatedItem { DisplayOrder = da.InternalOrder, IsHighlighted = false, Title = da.Title, Url = da.Url, Year = da.Year })];

        private static List<Course> TranslateCourses(IEnumerable<dM.EmployeeCourse> directoryCourses, IEnumerable<pC.Course> courses) => directoryCourses != null && directoryCourses.Any() ?
            [.. directoryCourses.Select(c => new Course { CourseNumber = c.CourseNumber, Description = c.Description, Rubric = c.Rubric, Title = c.Title, Url = c.Url })] :
            [.. courses.Select(c => new Course { CourseNumber = c.CourseNumber, Description = c.Description, Rubric = c.Rubric, Title = c.Name, Url = c.Url })];

        private static List<InstitutionalRangedItem> TranslateEducationHistory(IEnumerable<dM.EmployeeActivity> directoryActivites, IEnumerable<ExpertsItem> experts) => experts != null && experts.Any()
                ? [.. experts.Select(a => new InstitutionalRangedItem { DisplayOrder = a.SortOrder, IsHighlighted = a.IsHighlighted, Title = a.TitleFull, Url = a.Url, YearStarted = a.Year, YearEnded = a.YearEnded, Institution = a.Institution })]
                : [.. directoryActivites.Where(da => da.Type == dM.ActivityTypeEnum.Education).Select(da => new InstitutionalRangedItem { DisplayOrder = da.InternalOrder, IsHighlighted = false, Title = da.Title, Url = da.Url, YearStarted = da.YearStarted, YearEnded = da.YearEnded })];

        private static List<BaseItem> TranslateGrants(IEnumerable<ExpertsItem> experts) => experts != null && experts.Any()
                ? [.. experts.Select(a => new BaseItem { DisplayOrder = a.SortOrder, IsHighlighted = a.IsHighlighted, Title = a.TitleFull, Url = a.Url })]
                : [];

        private static List<JobProfile> TranslateJobProfiles(IEnumerable<dM.JobProfile> profiles) => [.. profiles.Where(j => j.Category != dM.ProfileCategoryTypeEnum.None || j.ProfileDisplay == dM.ProfileDisplayEnum.Not_Displayed).Select(j => new JobProfile { DisplayOrder = j.InternalOrder, JobType = j.Category.ToString(), Office = j.Office.Title, Title = j.Title, Description = j.Description, Tags = [.. j.Tags.Select(jt => jt.Title)] })];

        private static List<BaseItem> TranslateLinks(IEnumerable<dM.EmployeeActivity> directoryActivites, IEnumerable<ExpertsItem> experts) => experts != null && experts.Any()
                ? [.. experts.Select(a => new BaseItem { DisplayOrder = a.SortOrder, IsHighlighted = a.IsHighlighted, Title = a.TitleFull, Url = a.Url })]
                : [.. directoryActivites.Where(da => da.Type == dM.ActivityTypeEnum.Link && !da.Url.ToLower().Contains("linkedin.com")).Select(da => new BaseItem { DisplayOrder = da.InternalOrder, IsHighlighted = false, Title = da.Title, Url = da.Url })];

        private static List<NewsArticle> TranslateNews(IEnumerable<ExpertsItem> experts) => experts != null && experts.Any()
        ? [.. experts.Select(a => new NewsArticle { DisplayOrder = a.SortOrder, IsHighlighted = a.IsHighlighted, Title = a.TitleFull, Url = a.Url, Date = ConvertDate(a.Year), Source = a.Institution })]
        : [];

        private static string ConvertDate(string s) => DateTime.TryParse(s, out var date) ? date.ToString("MMMM dd, yyyy") : "";

        private static List<InstitutionalRangedItem> TranslateOrganizations(IEnumerable<dM.EmployeeActivity> directoryActivites, IEnumerable<ExpertsItem> experts) => experts != null && experts.Any()
                ? [.. experts.Select(a => new InstitutionalRangedItem { DisplayOrder = a.SortOrder, IsHighlighted = a.IsHighlighted, Title = a.TitleFull, Url = a.Url, Institution = a.Institution, YearEnded = a.YearEnded, YearStarted = a.Year })]
                : [.. directoryActivites.Where(da => da.Type == dM.ActivityTypeEnum.Committee).Select(da => new InstitutionalRangedItem { DisplayOrder = da.InternalOrder, IsHighlighted = false, Title = da.Title, Url = da.Url })];

        private static List<DatedItem> TranslatePresentations(IEnumerable<dM.EmployeeActivity> directoryActivites, IEnumerable<ExpertsItem> experts) => experts != null && experts.Any()
                ? [.. experts.Select(a => new DatedItem { DisplayOrder = a.SortOrder, IsHighlighted = a.IsHighlighted, Title = a.TitleFull, Url = a.Url })]
                : [.. directoryActivites.Where(da => da.Type == dM.ActivityTypeEnum.Presentation).Select(da => new DatedItem { DisplayOrder = da.InternalOrder, IsHighlighted = false, Title = da.Title, Url = da.Url })];

        private static List<DatedItem> TranslatePublications(IEnumerable<dM.EmployeeActivity> directoryActivites, IEnumerable<ExpertsItem> experts) => experts != null && experts.Any()
                ? [.. experts.Select(a => new DatedItem { DisplayOrder = a.SortOrder, IsHighlighted = a.IsHighlighted, Title = a.TitleFull, Url = a.Url, Year = a.Year })]
                : [.. directoryActivites.Where(da => da.Type == dM.ActivityTypeEnum.Publication).Select(da => new DatedItem { DisplayOrder = da.InternalOrder, IsHighlighted = false, Title = da.Title, Url = da.Url, Year = da.Year })];

        private static List<BaseItem> TranslateServices(IEnumerable<ExpertsItem> experts) => experts != null && experts.Any()
                ? [.. experts.Select(a => new BaseItem { DisplayOrder = a.SortOrder, IsHighlighted = a.IsHighlighted, Title = a.TitleFull, Url = a.Url })]
                : [];
    }
}