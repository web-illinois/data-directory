# Edit Unit Internal Information

## Summary

Internal Information controls how your unit works in the directory: notes, publishing, profile editing, profile loading, images, email signatures, and instructions shown to your users.

**Path:** Unit Administration > Internal Information

*Requires unit administrator access.*

## Steps

1. Open **Internal Information** from the left menu.
2. If you administer more than one unit, choose the unit first.
3. Update the settings below.
4. Select **Save**.

## General settings

- **Internal Notes** - notes to keep track of information about the unit. These are **not** shown to end users.
- **Allow Beta Access** - select to get access to beta features of the directory.
- **Profile Information** - controls who can edit profile information:
  - Do not allow profile information
  - Allow people to edit profile information
  - Allow office administrators to edit profile
  - Full Access (Illinois Experts / office administrators)

## Publishing Information

**Publishing Location** controls where your unit is shown:

- **Do not show**
- **Publish to site**
- **Publish to site and helpdesk**

It may take up to 24 hours to populate the directory information.

## Profile settings

The following sections appear only when **Profile Information** is set to something other than *Do not allow profile information*.

### Profile Url

The URL the system uses to view a profile. Updating this takes a while. Use these placeholders:

- `{netid}` - the user's NetID (lower-case)
- `{name}` - the user's name (lower-case, with dashes instead of spaces)

Examples: `https://education.illinois.edu/profile/{netid}` or `https://education.illinois.edu/profile/{name}`

### Profile Refresh Options

Choose how profile information is loaded automatically:

- **No Faculty Load**
- **Default Faculty Load** - make sure you add an Internal Code first. See [Links to Pull Directory Information](20-links-to-pull-information.md).
  - **Options** - add options for the load. Existing options include `use-directory-profile` and `searchstax`.
  - **Auto-load profiles to directory hook** - select for a regular refresh of your information.
- **Custom URL with API Key**
  - **Custom Profile Refresh Url** - the URL used to refresh a profile. Use `{netid}` for the user's NetID, for example `https://refresh.illinois.edu/Load?name={netid}`. Leave blank if you do not want information loaded automatically.

### API Updates (Custom URL only)

An API key allows you to add, update, and delete items in your source programmatically.

- **Create a New API key** - the key is shown once and becomes active when you select **Save**. **Record it before leaving the page; you cannot view it again.**
- **Invalidate the API key** - deactivates the current key.
- If you already have a key and create a new one, the old key still works. To deactivate a compromised key, invalidate it first and then create a new one.
- Changing **Profile Refresh Options** to anything other than *Custom URL with API Key* invalidates the API key.

### Image Information

- **Profile Image Width** and **Profile Image Height** - the dimensions used for profile headshots.

### Email Signature Instructions

- **Email Signature Additions** - add links to your employees' email signatures by sending a custom querystring. Popular additions are `facebook`, `twitter`, `instagram`, `linkedin`, `youtube`, and `disclaimer`. Use the pattern `&facebook=https://www.facebook.com/speciallink`.

### User Instructions

Optional text shown to your users when they edit the matching item:

- Office Instructions
- Employee Instructions (job profile or employee information)
- Employee Activities Instructions
- Employee Course Instructions
- Employee Signature Instructions
- Employee Headshot Instructions (does not include dimensions)
- Employee CV Instructions

If profile information is not allowed, only **Office Instructions** is shown.

## Order Information

Some administrators also see an **Order** setting. Units are listed alphabetically unless you choose **Top in List**, **Middle / Default**, or **Bottom in List**.

## Related articles

- [Edit Unit General Information](22-unit-general-information.md)
- [Links to Pull Directory Information](20-links-to-pull-information.md)
- [Generate an Email Signature](10-email-signature.md)
