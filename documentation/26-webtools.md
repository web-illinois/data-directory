# WebTools Integration

This page documents the Office WebTools page and the WebTools log (search).

## Office WebTools Integration

**Route:** `/office/webtools`

Links an office to WebTools so office information (and optionally employees) can be imported into the directory. If no office is selected, a picker is shown first.

> **Warning:** Saving overwrites existing information, including job profiles associated with the office.

### Fields

| Field | Description |
|-------|-------------|
| Department Code | Identifies the office within the organization. Not shown to end users. |
| Department Code Lookup | Dropdown of known department codes. Selecting one fills in the Department Code. |
| Service ID | Identifies the office's service. Not shown to end users. |
| Service Code Lookup | Dropdown of known service codes. Selecting one fills in the Service ID. |
| WebTools Automatic Import | `No automatic import`, `Import just the office`, or `Import the office and employees`. |

### Buttons

- **Save** – saves the settings. Leaving the page with unsaved changes prompts for confirmation.
- **Clear Cache** – clears the cached directory data.

## WebTools Log (Search)

**Route:** `/search/webtoolslogs`

Shows the history of WebTools imports.

- **Search Text** – filters the log entries.
- **Refresh Information** – reloads the log.
- Each entry shows the toolkit type, success status, and date/time. Expand an entry to see the full log text.

