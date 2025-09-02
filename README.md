# TopDownShooter

A top-down shooter game built with MonoGame Framework targeting .NET 8.0, featuring a custom Entity-Component-System architecture, advanced collision detection, and level progression system.

## Features

- **Custom Entity-Component-System (ECS)** - Modular architecture with reusable components
- **Advanced Collision Detection** - Spatial hashing for efficient AABB and Capsule collision
- **Level Management System** - Multi-level progression with seamless transitions
- **3D Rendering with 2D UI** - Mixed rendering pipeline with Myra UI framework
- **Performance Monitoring** - Real-time FPS tracking and performance metrics
- **Debug Visualization** - Comprehensive debug tools for collision boxes, aim vectors, and stats
- **AI Pathfinding** - Enemy AI with navigation grid-based pathfinding
- **Service Architecture** - Global services for Input, Audio, Assets, and UI management

## Prerequisites

- .NET 8.0 SDK
- MonoGame Framework DesktopGL 3.8.\*

## Getting Started

### Building and Running

```bash
# Restore dependencies and MonoGame tools
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

### MonoGame Content Pipeline

```bash
# Restore MonoGame content tools
dotnet tool restore

# Open content pipeline editor
mgcb-editor

# Build content from command line
mgcb
```

### Development

```bash
# Clean build artifacts
dotnet clean
```

## Controls

### Gameplay

- **WASD** - Player movement
- **Mouse** - Aim and look direction
- **Mouse Click** - Fire weapon (when implemented)

### Debug Controls

- **F1** - Toggle debug info and aim visualization
- **F2** - Toggle collision box visualization
- **F3** - Toggle FPS display
- **F4** - Toggle performance statistics
- **F11** - Toggle fullscreen
- **Escape** - Exit game

## Architecture

### Core Game Structure

- **GameRoot** - Main game class extending MonoGame.Game with service initialization
- **GameManager** - Singleton handling global settings, debug toggles, and window management
- **SceneManager** - Manages scene lifecycle and transitions
- **LevelManager** - Handles level progression with support for next/previous/cycling

### Entity-Component-System

- **Entity** - Base entity class with component dictionary system
- **Scene** - Abstract base for game scenes managing entity collections
- **Components** - Transform, CharacterMotor, ModelRenderer, and custom components
- **Interfaces** - IComponent, IUpdateable, IDrawable for component behaviors

### Game Systems

- **Physics** - Custom collision system with spatial hashing optimization
- **Rendering** - 3D world rendering with Camera, ModelRenderer, and DebugDraw utilities
- **Services** - Global services accessible via GameRoot static properties
- **World** - Level generation, navigation grids, and transition triggers

## Project Structure

### Core Files

- `Program.cs` - Application entry point
- `Game/Core/GameRoot.cs` - Main game class replacing traditional Game1.cs
- `Game/Core/GameManager.cs` - Global game settings and window management
- `Game/Core/Time.cs` - Static time management utilities

### Framework

- `Game/Framework/Entity.cs` - Base entity with component system
- `Game/Framework/Scene.cs` - Abstract scene base class
- `Game/Framework/SceneManager.cs` - Scene transition management
- `Game/Framework/Components/` - Component implementations

### Game Systems

- `Game/Physics/` - Collision detection with spatial hashing
- `Game/Rendering/` - 3D rendering, camera, and debug visualization
- `Game/Services/` - Input, Audio, Asset, and UI services
- `Game/World/` - Level management and navigation
- `Game/Gameplay/` - Player controller, enemies, and combat

### Content

- `Content/` - Game assets managed by MonoGame Content Pipeline
- `Content.mgcb` - MonoGame Content Builder configuration

## Technology Stack

- **Framework**: MonoGame Framework DesktopGL 3.8.\*
- **Platform**: Cross-platform desktop (Windows, macOS, Linux)
- **Language**: C# with .NET 8.0
- **UI Framework**: Myra UI for performance overlays and debug interface
- **Architecture**: Entity-Component-System with Service Locator pattern
- **Physics**: Custom collision system with spatial optimization

## Game Loop

The game uses MonoGame's fixed timestep pattern at 60 FPS:

1. `Initialize()` - Service initialization and ECS setup
2. `LoadContent()` - Asset loading and first level initialization
3. `Update(GameTime)` - Input, game logic, physics, and AI updates
4. `Draw(GameTime)` - 3D world rendering followed by 2D UI overlay

## Performance

- **Fixed 60 FPS** with VSync support
- **Spatial Hashing** for O(1) collision detection
- **Component Caching** to minimize allocations
- **Real-time Metrics** via PerformanceTracker and FPS charts
