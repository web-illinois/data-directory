# WIGG Directory Knowledge Base

The WIGG Directory is where faculty and staff enter directory information for **units**, **offices**, and **profiles**. This information may be displayed on your unit and office directories, as well as the Tech Help Center Help Desks page (TeamDynamix).

## Getting Started

- [01 - Getting Started and Signing In](01-getting-started.md)
- [02 - Understanding Units, Offices, and Profiles](02-understanding-units-offices-profiles.md)

## Managing Your Profile

- [03 - Edit Your General Profile Information](03-profile-general-information.md)
- [04 - Edit Your Job / Position Information](04-profile-job-information.md)
- [05 - Upload or Remove a Headshot](05-profile-headshot.md)
- [06 - Upload or Remove a Resume / CV](06-profile-resume-cv.md)
- [07 - Edit Your Biography](07-profile-biography.md)
- [08 - Set Your Profile Office Hours](08-profile-office-hours.md)
- [09 - Add Activities and Courses](09-profile-activities-and-courses.md)
- [10 - Generate an Email Signature](10-email-signature.md)
- [11 - Using Illinois Experts with Your Profile](11-illinois-experts.md)

## Office Administration

- [12 - Edit Office General Information](12-office-general-information.md)
- [13 - Set Office Hours](13-office-hours.md)
- [14 - Add, Edit, and Remove People in Your Office](14-office-people.md)
- [15 - Batch Load People to Your Office](15-office-batch-load.md)
- [16 - Manage Office Administrators](16-office-administrators.md)

## Unit Administration

- [17 - Unit Administration Overview](17-unit-administration.md)
- [18 - Manage Unit Tags](18-unit-tags.md)
- [22 - Edit Unit General Information](22-unit-general-information.md)
- [23 - Edit Unit Internal Information](23-unit-internal-information.md)
- [24 - Manage Unit Administrators](24-unit-administrators.md)
- [25 - How Do I Create a Directory?](25-create-a-directory.md)

## Finding Information and Integrations

- [19 - Find an Administrator or Profile](19-search.md)
- [20 - Links to Pull Directory Information (API)](20-links-to-pull-information.md)

## Help

- [21 - Troubleshooting and FAQ](21-troubleshooting-faq.md)

---

## About this Project

### Links about the project

- The code is in the [GitHub repository](https://github.com/web-illinois/data-directory).
- The project is [tracked through a GitHub project](https://github.com/orgs/web-illinois/projects/13/views/1).
- To report an issue or request a feature, please submit a [GitHub ticket](https://go.illinois.edu/directory-toolkit-help).

### Who Supports This Application?

The application cloud support is managed by Tech Services. The directory is maintained by a combination of the WIGG Team, IT Partners @ Education, and the Web COE Team. 

If you find an issue with the directory or want to request additional functionality, please submit a [GitHub ticket](https://go.illinois.edu/directory-toolkit-help). 

We are happy to have others work on the application to meet their needs. Please contact the WIGG Team if you want to contribute.

### Can I get information from the directory?

Yes. The directory works with the [Directory API](https://directoryapi.wigg.illinois.edu/api/swagger/ui), which can get unit, office, and basic profile information. It also has a hook that triggers a faculty load.

- The faculty load integrates with Illinois Experts, EDW, and course information from various repositories.
- There are plans to let colleges load their own information about their faculty.
- Because the load is potentially destructive, a custom key is required. The key is distributed to outside applications for their custom loads.

See [Links to Pull Directory Information](20-links-to-pull-information.md) and [How Do I Create a Directory?](25-create-a-directory.md).

### History

The College of Education needed a way to organize its faculty and staff. EDW and Illinois Experts supplied most of the information, but a few things were missing.

At the same time, TDX was building the help.illinois.edu site, which needed a list of units and offices. The unit information was originally stored in a Google Sheets document and pulled with Google APIs, which made it difficult to update.

To meet both needs, a Contact Support Application was created with Tech Services to store unit and office information. Profile information was then added, and the faculty load process was changed to call APIs from this application.

In 2024, the application was rebuilt to update the code, improve the interface, and simplify the directory load. It was also moved to Tech Services so it can be used by campus.