# SharpWM

A tiling window manager for Windows, written in C#. Inspired by [GlazeWM](https://github.com/glzr-io/glazewm).

![CI](https://github.com/NicholasCardona/SharpWM/actions/workflows/ci.yml/badge.svg)
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)
![Platform](https://img.shields.io/badge/platform-Windows-0078D4)
![License](https://img.shields.io/badge/license-GPL--3.0-blue)

> ⚠️ **Work in progress.** Core architecture is in place, first features are being implemented.

## Features (planned v1)

- Tiling layout (horizontal / vertical split)
- Global keybinding system
- Multi-monitor support
- Workspace management
- YAML configuration

## Architecture

```
SharpWM/
├── src/
│   ├── SharpWM.Common/     # Shared types: container tree, WmState, Rect
│   ├── SharpWM.Platform/   # Windows API interop (monitors, windows, hotkeys)
│   ├── SharpWM.Config/     # YAML config parsing
│   ├── SharpWM.Core/       # Command dispatcher, event loop
│   └── SharpWM/            # Entry point
└── tests/
    └── SharpWM.Tests/      # Unit tests (xUnit)
```

The WM state is represented as a **container tree**:

```
Root → Monitor → Workspace → SplitContainer → Window
```

All state mutations go through `WmState` in `SharpWM.Common`.

## Building

Requires [.NET 10 SDK](https://dotnet.microsoft.com/download) and Windows.

```powershell
git clone https://github.com/NicholasCardona/SharpWM.git
cd SharpWM
dotnet build
dotnet test
```

## Contributing

Contributions are welcome. See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## License

[GPL-3.0](LICENSE)