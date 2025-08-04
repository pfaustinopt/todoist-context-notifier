# Todoist Context Notifier

A lightweight Windows utility that checks your Todoist for tasks labeled with a computer-related context — like `@laptop`, `@desktop`, or `@pc`. If any matching tasks are found, it shows a Windows notification to remind you as soon as you log into your computer. Designed to integrate with Windows Task Scheduler for automatic execution on login.

![Notification Screenshot](docs/screenshot.png)

## ✨ Features

- 🔍 Checks your Todoist for tasks with a specific label
- 🔔 Displays a Windows notification with the task count
- 🔗 Clicking the notification opens the filtered task list in your browser
- ⚙️ Includes a script to configure Windows Task Scheduler

## 🛠 Requirements

- Windows 10/11
- [.NET 7.0 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/7.0/runtime)
- A [Todoist account](https://todoist.com)
- Your Todoist API token
- Administrator privileges (required to set up Task Scheduler)

## 🚀 Getting Started

1.  **Download** the latest version from the [Releases](https://github.com/pfaustinopt/todoist-context-notifier/releases) page
2.  **Extract** the contents to a folder of your choice
3.  **Edit the included `appsettings.json` file** and replace the following values:

    ```json
    {
      "Todoist": {
        "ApiKey": "your-api-key-here",
        "Label": "your-label-here"
      }
    }
    ```

    **`ApiKey`:**

    - Go to [Todoist Developer Settings](https://app.todoist.com/app/settings/integrations/developer)
    - Copy your personal API token and paste it here

    **`Label`:**

    - This is the name of the label you use to identify computer-related tasks
    - Example: `"Label": "laptop"` (no `@` symbol)

### 📅 Run at logon using Task Scheduler

To run the app automatically every time you log into Windows, **execute `create-task.bat`**. This script will:

- Prompts for admin permissions (required to register the task in Windows Task Scheduler)
- Register a scheduled task named "Todoist context notifier"
- Configure it to run with highest privileges
