# About the IT Partners Directory

## Summary

A short history of the project and how its information can be used by other systems.

## Background

The College of Education needed a way to organize its faculty and staff. EDW and Illinois Experts supplied most of the information, but a few things were missing.

At the same time, TDX was building the help.illinois.edu site, which needed a list of units and offices. The unit information was originally stored in a Google Sheets document and pulled with Google APIs, which made it difficult to update.

To meet both needs, a Contact Support Application was created with Tech Services to store unit and office information. Profile information was then added, and the faculty load process was changed to call APIs from this application.

In 2024 the application was rebuilt to update the code, improve the interface, and simplify the directory load. It was also moved to Tech Services so it can be used by campus.

## Can I get information from the directory?

Yes. The directory works with the [Directory API](https://directoryapi.wigg.illinois.edu/api/swagger/ui), which can get unit, office, and basic profile information. It also has a hook that triggers a faculty load.

- The faculty load integrates with Illinois Experts, EDW, and course information from various repositories.
- There are plans to let colleges load their own information about their faculty.
- Because the load is potentially destructive, a custom key is required. The key is distributed to outside applications for their custom loads.

See [Links to Pull Directory Information](20-links-to-pull-information.md) and [How Do I Create a Directory?](25-create-a-directory.md).

## Where is the code?

The code is in the [GitHub repository](https://github.com/web-illinois/data-directory).

## Related articles

- [Using Illinois Experts with Your Profile](11-illinois-experts.md)
