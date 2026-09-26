# Flightsim.to Publishing Information

## Basic Information

**Title**

```text
IVAO Auto Reconnect for MSFS 2024
```

**Author**

```text
nicola ananda
```

**Category**

```text
Utilities
```

If `Utilities` is unavailable, choose the closest application or tool category.

**Affiliation**

```text
No Affiliation / Independent
```

Do not select an official IVAO affiliation. This is an independent community project.

**Compatibility**

```text
Microsoft Flight Simulator 2024
```

**Initial Version**

```text
1.0.0
```

## Short Summary

```text
A lightweight system-tray utility that detects when IVAO Altitude goes offline and automatically reconnects after a configurable delay.
```

## Description

# IVAO Auto Reconnect for MSFS 2024

IVAO Auto Reconnect is a lightweight Windows system-tray utility for Microsoft Flight Simulator 2024 and the IVAO Altitude Pilot Client.

It monitors the connection status displayed by IVAO Altitude. If Altitude remains offline for the configured period, the utility automatically opens the connection dialog and presses Connect through Windows UI Automation.

## Features

- Runs quietly in the Windows system tray
- Does not add a permanent window to the taskbar
- Detects Altitude's ONLINE and OFFLINE states
- Configurable reconnect delay
- Configurable MSFS and Altitude process names
- Up to three automatic reconnect attempts
- Manual Reconnect Now command
- Monitoring On/Off control
- Local activity log
- Does not store IVAO credentials
- Does not modify simulator or Altitude files

## Requirements

- Windows 10 or Windows 11, 64-bit
- Microsoft Flight Simulator 2024
- IVAO Altitude Pilot Client
- Microsoft .NET 8 Desktop Runtime x64

Download the required runtime from:
https://dotnet.microsoft.com/en-us/download/dotnet/8.0

Select **Desktop Runtime** for **Windows x64**. The SDK and ASP.NET Runtime are not required.

## Installation

1. Install Microsoft .NET 8 Desktop Runtime x64.
2. Download and extract the package.
3. Run `IvaoAuto.exe`.
4. Find the icon in the Windows system tray. It may be hidden under the `^` icon.
5. Right-click the icon and open `Settings...`.
6. Enter the executable names shown in Windows Task Manager under the Details tab.
7. Set the reconnect delay.
8. Keep `Monitoring: On` enabled.

Example process names:

- MSFS: `FlightSimulator2024.exe`
- Altitude: `PilotUI.exe`

Process names may differ between installations. Verify them in Windows Task Manager.

## Tray Menu

- **Monitoring: On/Off** — enables or pauses automatic monitoring
- **Reconnect Now** — starts a reconnect attempt manually
- **Settings...** — changes process names and reconnect delay
- **Open Log** — opens the local activity log
- **Exit** — fully closes the utility

Double-clicking the tray icon also toggles monitoring.

## How It Works

When Altitude remains OFFLINE for the configured delay, the utility:

1. Checks that the configured MSFS process is running.
2. Opens Altitude's Connect to Network dialog.
3. Presses Connect without changing the existing connection details.
4. Waits until Altitude returns to ONLINE.
5. Retries up to three times if necessary.

The utility does not connect directly to IVAO servers. It automates the same Altitude controls that a user would normally press manually.

## Important Notes

- Run Altitude and IVAO Auto Reconnect with the same Windows permission level.
- Do not run only one application as Administrator.
- The Altitude connection form must already be complete and its Connect button enabled.
- Version 1.0.0 was tested with IVAO Altitude 1.13.0.33 using the English interface and Microsoft Flight Simulator 2024.
- Automatic reconnect is skipped when the configured MSFS process is not running.
- Future Altitude interface updates may require an update to this utility.
- The executable is currently unsigned, so Windows SmartScreen may display a warning. The complete source code is publicly available for review.

## Privacy

The utility works locally through Windows process inspection and UI Automation.

It does not:

- Send telemetry
- Make external network requests
- Store or read IVAO credentials
- Modify simulator files
- Modify Altitude files
- Change callsign, server, port, voice, MTL, observer, or follow-me settings

Settings and logs are stored locally in:

```text
%LOCALAPPDATA%\IvaoAuto\
```

## Open Source

The complete source code is available at:
https://github.com/nicolaananda/ivao-reconnect

## Disclaimer

This is an independent freeware community project. It is not affiliated with, endorsed by, or officially supported by IVAO, Microsoft, Flightsim.to, or the IVAO Altitude development team.

Use it at your own risk. Network conditions, software updates, or interface changes may prevent automatic reconnect from working.

## Suggested Tags

```text
IVAO
Altitude
Utility
Auto Reconnect
MSFS 2024
System Tray
```

## Upload Package

Use this filename:

```text
IVAO-Auto-Reconnect-v1.0.0-Windows-x64.zip
```

ZIP contents:

```text
IVAO-Auto-Reconnect-v1.0.0-Windows-x64.zip
├── IvaoAuto.exe
└── README.txt
```

All application files must be uploaded directly to Flightsim.to. Do not use an external download for the main application.

## Required Screenshots

Use original screenshots only, minimum 512 x 512 pixels. Do not use AI-generated images, real-life aviation images, or irrelevant thumbnails.

Recommended screenshots:

1. The system-tray menu showing Monitoring, Reconnect Now, Settings, Open Log, and Exit.
2. The Settings window showing the process-name and reconnect-delay fields.
3. Altitude showing ONLINE after a successful reconnect, with a separate visible log window if appropriate.

Hide callsigns, VID, names, IP addresses, and other personal information before uploading.

## Release Notes v1.0.0

```text
Initial public release.

- Automatic IVAO Altitude OFFLINE detection
- Configurable reconnect delay
- Configurable MSFS and Altitude process names
- Up to three reconnect attempts
- Manual reconnect command
- Monitoring On/Off control
- System-tray operation
- Local activity log
- Tested with IVAO Altitude 1.13.0.33 and Microsoft Flight Simulator 2024

Requires Microsoft .NET 8 Desktop Runtime x64.
```
