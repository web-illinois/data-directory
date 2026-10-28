# How Do I Create a Directory?

## Summary

A unit can use the WIGG Directory to automatically load faculty and staff information, so profiles can be displayed on the unit's own website.

## Steps

1. **Claim your unit.** [Contact us to ask to be a unit administrator for your unit](https://go.illinois.edu/directory-toolkit-help).
2. Under **Unit Administration > Links to Pull Information**, add an **Internal Code** and save your changes. See [Links to Pull Directory Information](20-links-to-pull-information.md).
3. Under **Unit Administration > Internal Information**, change **Profile Information** to **Full Access**.
4. Change the **Profile Url** to the web page and path where your profiles will be displayed.
5. Change **Profile Refresh Options** to **Default Faculty Load**.
6. Make sure **Auto-load profiles to directory hook** is selected.
7. Select **Save**.
8. Build out your offices and profile information.

For details on each setting, see [Edit Unit Internal Information](23-unit-internal-information.md).

## What happens next

- Any people you add to your offices are automatically loaded into the faculty load using the unique ID (Internal Code) you specified in step 2.
- You can add JavaScript to create profiles from this information in any content management system (CMS).
- People are loaded automatically once they save information in their profile in this application. Full refreshes also run every day.
- If someone's name is removed from EDW or from the profile list in the directory, their information is removed from the directory.

To check on loading, or to add a person to the queue, see [Understanding Units, Offices, and Profiles](02-understanding-units-offices-profiles.md).

## Using a different system for your faculty load

If you have another system that loads your faculty information, set **Profile Refresh Options** (step 5) to that load instead of **Default Faculty Load**. See [Edit Unit Internal Information](23-unit-internal-information.md) for the **Custom URL with API Key** option.

## Need help building profiles?

If you do not have the IT staff to build a JavaScript profile, contact the directory team through the [help link](https://go.illinois.edu/directory-toolkit-help). They are working with WIGG and the WCOE to create standard profiles.

## Directory export

The [Directory API](https://directoryapi.wigg.illinois.edu/api/swagger/ui) can export most of the data for units, offices, and profiles as JSON. When you need raw HTML or binary files instead, the Directory application provides these links:

### Picture

`https://directory.wigg.illinois.edu/picture/(netid)`

Returns the person's image as a binary in **webp** format. If the Net ID cannot be found, a generic image of a person is returned.

### Illinois Directory snippet

`https://directory.wigg.illinois.edu/campusexport/(internal-code)/(netid)`

Returns HTML that matches the format the Illinois Directory imports. Find your internal code under **Links to Pull Information** in the Directory application.

## Related articles

- [Unit Administration Overview](17-unit-administration.md)
- [Troubleshooting and FAQ](21-troubleshooting-faq.md)
