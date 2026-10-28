# Understanding Units, Offices, and Profiles

## Summary

The directory is organized into three levels. Knowing which level you are editing helps you find the right page.

## Definitions

- **Unit** - the top-level group (for example, a college or department). Units contain offices. Units are also referred to as *areas* in some places.
- **Office** - a group within a unit that has its own contact information, hours, and people.
- **Profile** - information about an individual person. A person can be connected to more than one office, with a different title and job type in each.

## How they connect

```text
Unit
 +-- Office
 |    +-- Person (job profile: title, job type)
 |    +-- Person
 +-- Office
	  +-- Person
```

## Removing a person from an office

Removing a person from your office only removes the connection between that employee and your office. **The employee's profile information is not deleted.**

## People in more than one office

If a person belongs to more than one office, they choose a **Primary Office** on the Job / Position page. The primary office's title is what shows when someone looks at the profile. If none is chosen, the first one created is used. See [Edit Your Job / Position Information](04-profile-job-information.md).

## Job type

Each person has a job type per office. A job type of **None** means the person will **not be listed**.

## Where each type is edited

| To edit... | Go to |
| --- | --- |
| A person | [Profile pages](03-profile-general-information.md) |
| An office | [Office general information](12-office-general-information.md) |
| A unit | [Unit administration](17-unit-administration.md) |

## Searching for units, offices, and profiles

The **Search** section lets you look up information in the directory. Use the left menu to move between the search pages.

| Search page | What it does |
| --- | --- |
| **Search for People** (`/search/people`) | Find a profile by Net ID. Select **View / Edit Profile** to open it. |
| **Search for Offices** (`/search/offices`) | Find an office and see its details, administrators, and people. |
| **Search for Units** (`/search/units`) | Find a unit and see its details, administrators, and offices. |
| **Automated Directory Logs** (`/search/logs`) | See pending and processed automated updates. See below. |
| **Change Logs** (`/search/changelogs`) | See a log of changes made to units, offices, and people. |

### Search for People

1. Type a Net ID in **Lookup / Filter Net ID**.
2. Choose a profile from the list.
3. Select **View / Edit Profile**.

This searches only people inside the WIGG Directory. To search the entire campus, use the [campus directory](https://directory.illinois.edu/search).

### Search for Offices

1. Type part of the office name in **Filter Office**.
2. Choose an office from the list.
3. Select **View / Edit Office**.

You can view the office's type, audience, contact information, building information, office hours, internal information, office administrators, and people.

### Search for Units

1. Type part of the unit name in **Filter Unit**.
2. Choose a unit from the list.
3. Select **View / Edit Unit**.

You can view the unit's type, audience, URLs, internal settings (such as beta access, profile information, and auto-load), unit administrators, and offices. Select **Back to Search** to return to the list.

### Change Logs

Enter text in **Search Text** and select **Refresh Information** to see changes. Each entry shows the date, what changed, the type of change, and who made it. Select an entry to see its details.

## Automated loading of profile information

Units can load profile information automatically instead of having each person enter it. A unit administrator turns this on in the unit's **Internal Information** page under **Profile Refresh Options** (No Faculty Load, Default Faculty Load, or Custom URL with API Key). For the default load, select **Auto-load profiles to directory hook** to get a regular refresh of your information. See [Edit Unit Internal Information](23-unit-internal-information.md).

Automated updates are placed in a queue and processed at regular intervals, so changes are not immediate. Names can take a few hours to be processed, and some directory information can take up to 24 hours to appear.

### Automated Directory Logs

**Path:** Search > Automated Directory Logs

This page shows the log of automated directory updates.

- **Pending Items** - the number of items waiting to be processed.
- **Processed names** - the names that have been processed. Select a name to see its date details and message.
- The log is deleted and refreshed 12 hours after the last item is added.

Use **Refresh Information** to see the latest counts and processed names.

### Add a person to the automated directory updates

If you need to update a person right away, for example to debug their information, you can add them to the top of the queue.

1. Open **Automated Directory Logs**.
2. Enter the person's Net ID in **Net ID to load**.
3. Select **Send Net ID to queue**.
4. A message confirms **Added to queue**. If the Net ID is not in the directory, you will see **Name not found**.
5. Select **Refresh Information** to check the latest status. Repeat until the name appears under **Processed names**.

The person must already exist in the WIGG Directory.
