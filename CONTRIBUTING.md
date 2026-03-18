# Contributing to SharpWM

Thanks for your interest in SharpWM!

## Getting started

1. Fork the repository and clone it locally
2. Install [.NET 10 SDK](https://dotnet.microsoft.com/download)
3. Run `dotnet build` to verify everything compiles
4. Run `dotnet test` to verify all tests pass

## Workflow

- Open an issue before starting work on a significant change
- Create a branch from `main` (e.g. `feature/workspace-switching`)
- Write or update tests for any logic you add or change
- Submit a pull request against `main`

## Code style

- C# 10, nullable enabled
- One class per file, filename matches class name
- `SharpWM.Common` for shared types — no Windows API calls there
- `SharpWM.Platform` for all Windows API interop

## Project structure

| Project | Purpose |
|---|---|
| `SharpWM.Common` | Container tree, WmState, shared value types |
| `SharpWM.Platform` | Windows API via P/Invoke and CsWin32 |
| `SharpWM.Config` | YAML config parsing (YamlDotNet) |
| `SharpWM.Core` | Command dispatcher, event loop |
| `SharpWM` | Entry point |
| `SharpWM.Tests` | xUnit unit tests |