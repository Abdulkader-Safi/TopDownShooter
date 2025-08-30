# TopDownShooter

A top-down shooter game built with MonoGame Framework targeting .NET 8.0.

## Prerequisites

- .NET 8.0 SDK
- MonoGame Framework DesktopGL 3.8.\*

## Getting Started

### Building and Running

```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the game
dotnet run

# Build for release
dotnet build -c Release

# Publish the game
dotnet publish
```

### Development

```bash
# Clean build artifacts
dotnet clean
```

## Project Structure

- `Game1.cs` - Main game class containing core game loop
- `Program.cs` - Application entry point
- `Content/` - Game assets managed by MonoGame Content Pipeline
- `Content.mgcb` - MonoGame Content Builder configuration
- `Icon.ico`, `Icon.bmp` - Application icons

## Technology Stack

- **Framework**: MonoGame Framework DesktopGL
- **Platform**: Cross-platform desktop (Windows, macOS, Linux)
- **Language**: C#
- **Target**: .NET 8.0

## Game Loop

The game follows MonoGame's standard pattern:

1. `Initialize()` - One-time initialization
2. `LoadContent()` - Load game assets
3. `Update()` / `Draw()` - Main game loop
