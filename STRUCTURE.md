# TopDownShooter - Project Structure Documentation

This document explains the architecture and purpose of each file in this MonoGame project. The project is split into **Core** (reusable framework) and **Game** (TopDownShooter-specific) components for modularity.

## Root Directory

### Project Files

- **`Program.cs`** - Application entry point, creates and runs GameRoot instance
- **`TopDownShooter.csproj`** - Project file with dependencies (MonoGame, Myra UI, BepuPhysics)
- **`TopDownShooter.sln`** - Visual Studio solution file
- **`app.manifest`** - Windows application manifest for DPI awareness
- **`Icon.bmp`** / **`Icon.ico`** - Application icons
- **`README.md`** - Project overview and setup instructions
- **`CLAUDE.md`** - AI assistant instructions and project guidance
- **`STRUCTURE.md`** - This documentation file

## Core Directory - Reusable Framework

The **Core/** directory contains all reusable framework components that can be copied to other MonoGame projects.

### `/Core/GameSystems/` - Core Game Systems

- **`GameRoot.cs`** - Main game class (replaces Game1.cs), handles initialization, update/draw loops, and service management
- **`GameManager.cs`** - Singleton for global settings, debug controls (F1-F5, F11), and configuration management
- **`Time.cs`** - Static utility class for game timing and delta time calculations
- **`Extensions.cs`** - Extension methods for common operations and utilities

### `/Core/Framework/` - Entity-Component-System Framework

- **`Entity.cs`** - Base entity class with component system, Transform management, and lifecycle methods
- **`Scene.cs`** - Abstract base class for game scenes, manages entity collections and update/draw cycles
- **`SceneManager.cs`** - Handles scene transitions, loading/unloading, and scene lifecycle
- **`PerformanceTracker.cs`** - FPS tracking and performance monitoring with history buffer

#### `/Core/Framework/Components/` - Component System

- **`Transform.cs`** - Position, rotation, scale component for all entities
- **`ModelRenderer.cs`** - 3D model rendering component with color, size, and visibility control
- **`CharacterMotor.cs`** - Physics-based movement component with collision detection and response
- **`RigidBodyComponent.cs`** - BepuPhysics integration component with Static/Kinematic/Dynamic body types
- **`ColliderComponent.cs`** - Physics collider component with Box/Sphere/Capsule shape support

### `/Core/Services/` - Global Service Layer

- **`AssetService.cs`** - Content loading and management service
- **`AudioService.cs`** - Sound effects and music management service
- **`InputService.cs`** - Keyboard, mouse, and gamepad input handling service
- **`MyraService.cs`** - UI framework service managing MyraUI windows and components
- **`PhysicsService.cs`** - Physics system service managing both collision world and BepuPhysics integration

### `/Core/UI/` - Generic User Interface Components

- **`MyraFpsWindow.cs`** - Performance monitoring window with real-time FPS chart and statistics

### `/Core/Physics/` - Physics Framework

#### Legacy Collision System

- **`CollisionWorld.cs`** - Physics world management with spatial hashing for efficient collision detection
- **`ICollider.cs`** - Interface for collision detection components
- **`AabbCollider.cs`** - Axis-Aligned Bounding Box collider implementation
- **`CapsuleCollider.cs`** - Capsule collider implementation for character physics

#### BepuPhysics Integration

- **`SimpleBepuPhysicsWorld.cs`** - Foundation layer for BepuPhysics v2.4.0 integration
- **`VectorConversions.cs`** - Extension methods for converting between MonoGame and BepuPhysics vector types
- **`PhysicsBodyFactory.cs`** - Factory methods for creating common physics body configurations
- **`PhysicsCollisionBridge.cs`** - Bridge layer connecting legacy collision system with BepuPhysics
- **`PhysicsDebugRenderer.cs`** - Debug visualization for physics bodies, velocities, and contact points
- **`PhysicsTestEntity.cs`** - Test entity for verifying physics integration functionality

#### `/Core/Physics/Spatial/` - Spatial Optimization

- **`SpatialHash.cs`** - Spatial hashing grid for efficient collision queries and broad-phase detection

### `/Core/Rendering/` - Rendering Framework

- **`Camera.cs`** - 3D camera with projection matrices, view controls, and camera management
- **`DebugDraw.cs`** - Debug visualization utilities for collision boxes, wireframes, and debug info
- **`Lighting.cs`** - Lighting system management and light source handling

## Game Directory - TopDownShooter Specific

The **Game/** directory contains all game-specific logic and implementations unique to the TopDownShooter project.

### `/Game/Scenes/` - Game Scene Implementations

- **`StartMenuScene.cs`** - Main menu scene with title, start button, and game initialization
- **`GameScene.cs`** - Level 1 gameplay scene containing player, enemies, and world entities
- **`Level2Scene.cs`** - Level 2 implementation with specific entities and layout

### `/Game/Gameplay/` - Game Logic

#### `/Game/Gameplay/Player/` - Player Systems

- **`PlayerController.cs`** - Player entity with movement, aiming, dash mechanics, and input handling

#### `/Game/Gameplay/Enemies/` - Enemy Systems

- **`DummyChaser.cs`** - Basic AI enemy that chases player, with health system and damage feedback

#### `/Game/Gameplay/Combat/` - Combat System

- **`Hitboxes.cs`** - Combat system handling damage dealing, hitbox detection, and damage text display

### `/Game/World/` - Level and World Management

- **`Level.cs`** - Level data structure with navigation grid, spawn points, and level properties
- **`LevelManager.cs`** - Singleton managing level loading, transitions, and current level state
- **`LevelTransitionTrigger.cs`** - Trigger entity for level transitions and area changes
- **`NavGrid.cs`** - Navigation mesh/grid system for AI pathfinding

## Content Directory

### `/Content/` - Game Assets

- **`Content.mgcb`** - MonoGame Content Pipeline project file for asset compilation
- **`DefaultFont.spritefont`** - Font descriptor for UI text rendering (Arial 14pt with extended Latin character support)
- **`bin/`** - Compiled content assets (XNB files)
- **`obj/`** - Intermediate build files

## Architecture Patterns

### Service Locator Pattern

Global services accessible via `GameRoot.Input`, `GameRoot.Audio`, `GameRoot.Assets`, `GameRoot.Myra`, `GameRoot.Physics`

### Entity-Component-System (ECS)

- Entities use composition over inheritance
- Add components via `AddComponent<T>()`
- Retrieve components via `GetComponent<T>()`
- Physics helper methods: `entity.CreateDynamicBox(Vector3.One)`, `entity.GetRigidBody()`, `entity.HasPhysicsBody()`

### Scene Management

- Scenes inherit from `Scene` base class
- Automatic update/draw lifecycle through SceneManager
- Entity management per scene
- Game flow: StartMenuScene → GameScene/Level2Scene via LevelManager

## Namespace Organization

The project uses a clear namespace structure reflecting the Core/Game split:

### Core Namespaces (Reusable Framework)

- **`Core.GameSystems`** - Core game systems (GameRoot, GameManager, Time, Extensions)
- **`Core.Framework`** - ECS framework (Entity, Scene, SceneManager, PerformanceTracker)
- **`Core.Framework.Components`** - All component types
- **`Core.Services`** - Global services (Input, Audio, Assets, Myra, Physics)
- **`Core.Physics`** - Physics systems and collision detection
- **`Core.Physics.Spatial`** - Spatial optimization (SpatialHash)
- **`Core.Rendering`** - Rendering utilities (Camera, DebugDraw, Lighting)
- **`Core.UI`** - Generic UI components (MyraFpsWindow)

### Game Namespaces (TopDownShooter Specific)

- **`Game.Scenes`** - Game scene implementations
- **`Game.Gameplay.Player`** - Player-specific systems
- **`Game.Gameplay.Enemies`** - Enemy AI and behavior
- **`Game.Gameplay.Combat`** - Combat and damage systems
- **`Game.World`** - Level management and world systems

### Physics Integration

**Dual-Layer Physics System:**

- **Legacy System**: Spatial hashing for efficient collision detection with AABB and Capsule colliders
- **BepuPhysics Integration**: Foundation components (RigidBodyComponent, ColliderComponent) with factory methods
- **Bridge Layer**: PhysicsCollisionBridge enables gradual migration between systems
- **Debug Visualization**: PhysicsDebugRenderer for visualizing physics bodies, velocities, and contact points

## Key Design Decisions

1. **MonoGame Framework**: Uses MonoGame 3.8.\* with DesktopGL for cross-platform support
2. **BepuPhysics Integration**: BepuPhysics v2.4.0 foundation with gradual migration approach
3. **Fixed Timestep**: 60 FPS with vsync for consistent gameplay
4. **3D Rendering**: 3D world with 2D UI overlay approach
5. **MyraUI**: Modern UI framework replacing ImGui for better .NET 8 compatibility
6. **Extended SpriteFont**: MonoGame font system with extended Latin character support (includes degree symbol)
7. **Component System**: Modular, reusable components including physics integration
8. **Singleton Services**: Global access to input, audio, assets, UI, and physics systems
9. **Hybrid Physics**: Dual-layer physics system for gradual migration from spatial hashing to BepuPhysics

## Controls

### Menu Controls

- **Click START button**: Begin game from main menu
- **ENTER/SPACE**: Alternative way to start game from menu
- **Escape**: Exit game

### Debug Controls (In-Game)

- **F1**: Toggle debug info and aim visualization
- **F2**: Toggle collision box visualization
- **F3**: Toggle FPS display
- **F4**: Toggle performance statistics
- **F5**: Toggle FPS chart window
- **F11**: Toggle fullscreen
- **Escape**: Exit game

## Getting Started with This Structure

### For New Projects (Using Core Framework)

1. **Copy the Core directory** - All files in `/Core/` are reusable framework components
2. **Update namespaces** - Change `Core.*` to your project's core namespace (e.g., `YourGame.Core.*`)
3. **Implement game-specific content** - Create your own `/Game/` directory with scenes and gameplay
4. **Extend the framework** - Add new components, services, and systems as needed
5. **Follow established patterns** - Use the service locator, ECS, and scene management patterns

### For Extending This Project

1. **Game-specific content** goes in `/Game/` directory
2. **Framework improvements** go in `/Core/` directory
3. **Maintain namespace separation** between Core (reusable) and Game (specific)
4. **Use existing services** via GameRoot static properties
5. **Follow component pattern** for new entity behaviors

## Benefits of This Split

- **Modularity**: Core framework can be reused across multiple projects
- **Clear separation**: Game logic is isolated from reusable systems
- **Maintainability**: Changes to framework don't affect game-specific code
- **Testability**: Core components can be unit tested independently
- **Documentation**: Clear distinction between what's reusable vs project-specific

This structure provides a solid foundation for 3D MonoGame projects with modern architecture patterns, comprehensive debugging tools, and extensible systems that can be easily adapted for other games.
