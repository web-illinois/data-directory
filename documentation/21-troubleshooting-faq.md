# Troubleshooting and FAQ

## My changes in My UI Info are not showing up

Updates made in [My UI Info](https://go.uillinois.edu/myprofile) take a few days to reach the directory. To force a refresh, open **General Information** in your profile and select **Save**.

## My Illinois Experts changes are not showing up

Illinois Experts updates are processed with the normal directory process and can take a few days. Open **Biography** or **Activities** in the directory and select **Refresh Directory Entry**. See [Using Illinois Experts with Your Profile](11-illinois-experts.md).

## Faculty are not loading from Illinois Experts automatically

A unit administrator should check the unit's **Internal Information** page (Unit Administration > Internal Information) to confirm that profiles are loaded automatically and are pulling from Illinois Experts:

1. Confirm **Profile Information** is set to **Full Access (Illinois Experts / office administrators)**.
2. Confirm **Publishing Location** is not set to **Do not show**.
3. Under **Profile Refresh Options**, confirm the unit is set to **Default Faculty Load** (or a **Custom URL with API Key** if your unit uses a custom load) and not **No Faculty Load**.
4. For **Default Faculty Load**, confirm the **Options** field is set up for your unit's load, and that **Auto-load profiles to directory hook** is selected so the information is refreshed regularly.
5. Confirm the unit has an **Internal Code**. The default faculty load requires one. See [Links to Pull Directory Information](20-links-to-pull-information.md).
6. Select **Save** if you made any changes.

If the settings are correct:

- Check that the person has an Illinois Experts account and has entered the information. See [Using Illinois Experts with Your Profile](11-illinois-experts.md).
- Check the **Automated Directory Logs** in the Search section to see if the person has been processed. You can also add their Net ID to the top of the queue. See [Understanding Units, Offices, and Profiles](02-understanding-units-offices-profiles.md).
- Allow time for processing. Updates are queued and can take a few hours, and some information can take up to 24 hours to appear.

See [Edit Unit Internal Information](23-unit-internal-information.md) for details on each setting.

## I cannot edit some fields

Some fields are controlled by an office administrator or are loaded from another system. Contact your office administrator. To find them, see [Find an Administrator or Profile](19-search.md).

## I am not listed in the directory

Check your **Job Type** on the Job / Position page. A job type of **None** means you will not be listed. See [Edit Your Job / Position Information](04-profile-job-information.md).

## I cannot upload a new headshot or CV

Delete the existing file first, upload the new one, then select **Save**. See [headshot](05-profile-headshot.md) and [CV](06-profile-resume-cv.md).

## A day is missing from my office hours

If either the opening or closing time is blank, that day is not displayed.

## I removed a person from my office. Was their profile deleted?

No. Only the connection to your office is removed.

## I removed a tag, but it still appears

It can take a few hours for removal to process. Check the logs in the Search section.

## I changed something, but it does not appear on the website

Use the **Clear Cache** button on the page where you made the change, then check again.

## I see "unsaved changes" warnings

Select **Save** before leaving the page, or confirm you want to leave and discard your changes.

## Where do I report incorrect information about an office?

Use **Find an Administrator** on the home page to contact the office or unit administrators. See [Find an Administrator or Profile](19-search.md).
