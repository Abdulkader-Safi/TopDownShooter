# TopDownShooter

A top-down shooter game built with MonoGame Framework targeting .NET 8.0, featuring a modular **Core/Game architecture** where the Core framework is reusable across MonoGame projects and Game contains TopDownShooter-specific logic.

## Features

- **Modular Core/Game Architecture** - Reusable MonoGame framework (`Core/`) separated from game logic (`Game/`)
- **Custom Entity-Component-System (ECS)** - Component-based architecture with Transform, ModelRenderer, CharacterMotor, etc.
- **Dual Physics System** - Spatial hashing collision detection with BepuPhysics v2.4.0 integration foundation
- **Level Management System** - Multi-level progression with seamless transitions and cycling support
- **3D Rendering with 2D UI** - Mixed rendering pipeline with Myra UI framework and performance charts
- **Performance Monitoring** - Real-time FPS tracking, performance metrics, and interactive charts (F5)
- **Debug Visualization** - Comprehensive debug tools (F1-F4) for collision boxes, aim vectors, and performance stats
- **AI Pathfinding** - Enemy AI with navigation grid-based pathfinding system
- **Service Architecture** - Global services accessible via GameRoot static properties

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
- **F5** - Toggle FPS chart window
- **F11** - Toggle fullscreen
- **Escape** - Exit game

## Architecture

### Core/Game Split

The project is organized into two main directories for modularity and reusability:

- **Core/** - Reusable MonoGame framework components (namespaced as `Core.*`)
  - GameSystems, Framework (ECS), Services, Physics, Rendering, UI
- **Game/** - TopDownShooter-specific implementations (namespaced as `Game.*`)
  - Scenes, Gameplay (Player/Enemies/Combat), World management

### Core Framework (Reusable)

- **GameRoot** (`Core.GameSystems`) - Main game class with service initialization and static service access
- **GameManager** (`Core.GameSystems`) - Global settings, debug toggles, window management
- **Entity-Component-System** (`Core.Framework`) - Entity base class, Scene management, component system
- **Services** (`Core.Services`) - InputService, AudioService, AssetService, MyraService, PhysicsService
- **Physics** (`Core.Physics`) - Spatial hashing collision + BepuPhysics integration foundation
- **Rendering** (`Core.Rendering`) - Camera, DebugDraw, Lighting utilities

### Game Implementation (TopDownShooter)

- **Scenes** (`Game.Scenes`) - StartMenuScene, GameScene, Level2Scene
- **Player** (`Game.Gameplay.Player`) - PlayerController with movement, aiming, combat
- **Enemies** (`Game.Gameplay.Enemies`) - AI entities like DummyChaser with pathfinding
- **Combat** (`Game.Gameplay.Combat`) - Damage system with hitboxes and visual feedback
- **World** (`Game.World`) - Level, LevelManager, NavGrid, LevelTransitionTrigger

## Project Structure

```bash
TopDownShooter/
├── Program.cs                    # Application entry point
├── Core/                         # Reusable MonoGame Framework
│   ├── GameSystems/             # Core game systems
│   │   ├── GameRoot.cs          # Main game class
│   │   ├── GameManager.cs       # Global settings/debug
│   │   ├── Time.cs              # Time utilities
│   │   └── Extensions.cs        # Extension methods
│   ├── Framework/               # Entity-Component-System
│   │   ├── Entity.cs            # Base entity class
│   │   ├── Scene.cs             # Abstract scene base
│   │   ├── SceneManager.cs      # Scene transitions
│   │   ├── PerformanceTracker.cs # FPS monitoring
│   │   └── Components/          # All component types
│   ├── Services/                # Global services
│   ├── Physics/                 # Physics & collision systems
│   ├── Rendering/               # Camera & rendering utilities
│   └── UI/                      # Generic UI components
├── Game/                        # TopDownShooter Specific
│   ├── Scenes/                  # Game scene implementations
│   ├── Gameplay/                # Player, enemies, combat
│   └── World/                   # Level management
└── Content/                     # MonoGame assets
    ├── Content.mgcb             # Content pipeline config
    └── DefaultFont.spritefont   # UI font asset
```

## Technology Stack

- **Framework**: MonoGame Framework DesktopGL 3.8.\*
- **Platform**: Cross-platform desktop (Windows, macOS, Linux)
- **Language**: C# with .NET 8.0
- **UI Framework**: Myra UI for performance overlays and debug interface
- **Architecture**: Modular Core/Game split with Entity-Component-System and Service Locator patterns
- **Physics**: Dual-layer system - spatial hashing collision + BepuPhysics v2.4.0 integration foundation

## Game Loop

The game uses MonoGame's fixed timestep pattern at 60 FPS:

1. `Initialize()` - Service initialization and ECS setup
2. `LoadContent()` - Asset loading and first level initialization
3. `Update(GameTime)` - Input, game logic, physics, and AI updates
4. `Draw(GameTime)` - 3D world rendering followed by 2D UI overlay

## Performance

- **Fixed 60 FPS** with VSync support
- **Spatial Hashing** for O(1) collision detection in `Core.Physics.SpatialHash`
- **Component Caching** to minimize allocations
- **Real-time Metrics** via `PerformanceTracker` and interactive FPS charts (`MyraFpsWindow`)

## Using the Core Framework

### For New Projects

1. **Copy the Core directory** - All files in `Core/` are reusable framework components
2. **Update namespaces** - Change `Core.*` to your project's namespace (e.g., `MyGame.Core.*`)
3. **Create your Game directory** - Implement game-specific logic with your own namespace
4. **Follow the patterns** - Use Service Locator, ECS, and Scene Management patterns

### Benefits of This Architecture

- **Modularity** - Core framework can be reused across multiple MonoGame projects
- **Clear Separation** - Game logic is isolated from reusable framework systems
- **Maintainability** - Framework improvements don't break game-specific code
- **Testability** - Core components can be unit tested independently
