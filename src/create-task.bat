@echo off
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo Administrator privileges required.
    powershell -Command "Start-Process '%~dpnx0' -Verb RunAs"
    exit /b
)

set EXE_PATH=%~dp0todoist-context-notifier.exe

if not exist "%EXE_PATH%" (
    echo Could not find executable at: %EXE_PATH%
    echo Please build the project first using: dotnet build
    pause
    exit /b 1
)

echo Found executable at: %EXE_PATH%

schtasks /Create /F /RL HIGHEST /SC ONLOGON /TN "Todoist context notifier" /TR "\"%EXE_PATH%\""
if %errorLevel% equ 0 (
    echo Task scheduler job created successfully.
    echo Task Name: Todoist context notifier

    :: Disable "Start task only if on AC power" using PowerShell
    powershell -Command ^
      "$task = Get-ScheduledTask -TaskName 'Todoist context notifier';" ^
      "$settings = $task.Settings;" ^
      "$settings.DisallowStartIfOnBatteries = $false;" ^
      "$settings.StopIfGoingOnBatteries = $false;" ^
      "Set-ScheduledTask -TaskName 'Todoist context notifier' -Settings $settings"

    if %errorLevel% equ 0 (
      echo Task power settings updated to run on battery too.
    ) else (
      echo Failed to update power settings. Run script as admin.
    )

) else (
    echo Failed to create task. Error code: %errorLevel%
)

pause