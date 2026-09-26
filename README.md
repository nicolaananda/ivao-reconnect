# IVAO Auto Reconnect

A lightweight Windows system-tray watchdog for **IVAO Altitude** and **Microsoft Flight Simulator 2024**. It detects when Altitude becomes offline and attempts to reconnect after a configurable delay.

> Community project. Not affiliated with or endorsed by IVAO, Microsoft, or the Altitude development team.

## Features

- Runs only in the Windows system tray; no taskbar window.
- Detects MSFS and Altitude using configurable process names.
- Detects Altitude's `ONLINE` and `OFFLINE` UI states through Windows UI Automation.
- Configurable reconnect delay from 0.1 to 60 minutes.
- Opens Altitude's connection dialog and presses `CONNECT` without changing flight details.
- Up to three reconnect attempts with increasing delays.
- Manual **Reconnect Now** command.
- Monitoring **On/Off** control.
- Local activity log; no VID or password storage.
- Small single-file Windows x64 executable.

## Download

Download `IvaoAuto.exe` from the repository's **Releases** page.

> **Required:** Install the [Microsoft .NET 8 Desktop Runtime (Windows x64)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0). Choose **Desktop Runtime**, not SDK or ASP.NET Runtime.

Windows may show a SmartScreen warning because the executable is not code-signed. Review the source or build it yourself if preferred.

## Usage

1. Start Microsoft Flight Simulator 2024 and IVAO Altitude.
2. Start `IvaoAuto.exe`.
3. Find the app icon in the Windows system tray. It may be under the hidden-icons arrow (`^`).
4. Right-click the icon and select **Settings...**.
5. Enter the process names shown in **Task Manager → Details**:

   ```text
   MSFS process (.exe):       FlightSimulator2024.exe
   Altitude process (.exe):   PilotUI.exe
   Reconnect delay (minutes): 0.3
   ```

6. Keep **Monitoring: On** enabled.

When Altitude remains `OFFLINE` for the configured delay, the app opens **CONNECT TO NETWORK**, presses `CONNECT`, and waits for `ONLINE`.

## Tray menu

| Menu | Action |
|---|---|
| `Monitoring: On/Off` | Enables or pauses automatic monitoring |
| `Reconnect Now` | Starts one reconnect cycle manually, even without MSFS detection |
| `Settings...` | Changes process names and reconnect delay |
| `Open Log` | Opens the local watchdog log |
| `Exit` | Fully closes the app |

Double-clicking the tray icon also toggles monitoring.

## Important notes

- Run Altitude and IVAO Auto Reconnect with the same permission level. Do not run only one of them as Administrator.
- Altitude's connection form must already be complete and its `CONNECT` button enabled.
- The initial release targets the English Altitude UI and layout observed in version `1.13.0.33`.
- The app does not edit callsign, server, port, voice, MTL selection, or observer/follow-me options.
- Automatic reconnect is skipped when the configured MSFS process is not running.

Settings and logs are stored locally:

```text
%LOCALAPPDATA%\IvaoAuto\settings.json
%LOCALAPPDATA%\IvaoAuto\watchdog.log
```

## Troubleshooting

### Altitude is open but not detected

Open **Task Manager → Details**, copy the actual Altitude executable name, and enter it under **Settings...**.

### MSFS is running but reconnect is skipped

Copy the actual simulator executable name from **Task Manager → Details** into the MSFS process setting.

### OFFLINE is detected but CONNECT is not pressed

Check that:

- the connection dialog is complete;
- `CONNECT` is enabled;
- Altitude uses the English interface;
- both applications use the same permission level.

Then use **Open Log** and include the relevant lines when reporting an issue.

## Build from source

Requirements: Windows and [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) for building. End users need the .NET 8 Desktop Runtime.

```powershell
./build.ps1
```

Or:

```powershell
dotnet restore
dotnet publish -c Release -r win-x64 --self-contained false -o publish
```

Output:

```text
publish\IvaoAuto.exe
```

## Privacy and security

The app works locally through Windows process inspection and UI Automation. It does not send telemetry, access IVAO credentials, or make network requests.

## License

No license has been granted yet. Source code is visible for review, but redistribution and modification rights remain reserved until a license is added.

