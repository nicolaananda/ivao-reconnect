IVAO AUTO RECONNECT v1.0.0
==========================

A lightweight Windows system-tray utility for Microsoft Flight Simulator 2024 and the IVAO Altitude Pilot Client.

IVAO Auto Reconnect monitors the connection status displayed by Altitude. If Altitude remains offline for the configured period, the utility automatically opens the connection dialog and presses Connect through Windows UI Automation.

REQUIREMENTS
------------

- Windows 10 or Windows 11, 64-bit
- Microsoft Flight Simulator 2024
- IVAO Altitude Pilot Client
- Microsoft .NET 8 Desktop Runtime x64

Download the required runtime from:
https://dotnet.microsoft.com/en-us/download/dotnet/8.0

Select the Windows x64 Desktop Runtime. The SDK and ASP.NET Runtime are not required.

INSTALLATION
------------

1. Install Microsoft .NET 8 Desktop Runtime x64.
2. Extract the downloaded ZIP file to a folder of your choice.
3. Run IvaoAuto.exe.
4. Find the IVAO Auto Reconnect icon in the Windows system tray. It may be hidden under the ^ icon.
5. Right-click the icon and select Settings.
6. Enter the executable names shown in Windows Task Manager under the Details tab.
7. Choose the reconnect delay in minutes.
8. Keep Monitoring: On enabled.

Example process names:

MSFS:     FlightSimulator2024.exe
Altitude: PilotUI.exe

Process names may differ between installations. Check Task Manager if either application is not detected.

TRAY MENU
---------

Monitoring: On/Off
Enables or pauses automatic monitoring. Double-clicking the tray icon also toggles this setting.

Reconnect Now
Starts a reconnect attempt manually.

Settings
Changes the MSFS process name, Altitude process name, and reconnect delay.

Open Log
Opens the local activity log.

Exit
Fully closes the utility.

HOW IT WORKS
------------

When Altitude remains OFFLINE for the configured delay, the utility:

1. Checks that the configured MSFS process is running.
2. Opens Altitude's Connect to Network dialog.
3. Presses Connect without changing the existing connection details.
4. Waits until Altitude returns to ONLINE.
5. Retries up to three times if necessary.

The utility does not connect directly to IVAO servers. It automates the same Altitude controls that a user would normally press manually.

IMPORTANT NOTES
---------------

- Run Altitude and IVAO Auto Reconnect with the same Windows permission level.
- Do not run only one of the applications as Administrator.
- The Altitude connection form must already be complete and its Connect button enabled.
- Version 1.0.0 was tested with IVAO Altitude 1.13.0.33 using the English interface and Microsoft Flight Simulator 2024.
- A future Altitude interface update may require an update to this utility.
- Automatic reconnect is skipped when the configured MSFS process is not running.
- The executable is currently unsigned. Windows SmartScreen may display a warning.

PRIVACY
-------

IVAO Auto Reconnect works locally through Windows process inspection and UI Automation.

It does not:

- Store or read your IVAO VID or password
- Send telemetry
- Make external network requests
- Modify Microsoft Flight Simulator files
- Modify IVAO Altitude files
- Change callsign, server, port, voice, MTL, observer, or follow-me settings

Local files:

Settings: %LOCALAPPDATA%\IvaoAuto\settings.json
Log:      %LOCALAPPDATA%\IvaoAuto\watchdog.log

TROUBLESHOOTING
---------------

Altitude is open but not detected:
Open Windows Task Manager, select the Details tab, copy the actual Altitude executable name, and enter it in Settings.

MSFS is running but reconnect is skipped:
Copy the actual simulator executable name from Task Manager and enter it in Settings.

OFFLINE is detected but Connect is not pressed:
Confirm that the Altitude connection form is complete, Connect is enabled, Altitude uses the English interface, and both applications use the same permission level. Open the log for additional details.

PROJECT
-------

Source code:
https://github.com/nicolaananda/ivao-reconnect

DISCLAIMER
----------

This is an independent freeware community project. It is not affiliated with, endorsed by, or officially supported by IVAO, Microsoft, Flightsim.to, or the IVAO Altitude development team.

Use it at your own risk. Network conditions, software updates, or interface changes may prevent automatic reconnect from working.
