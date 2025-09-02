# TopDownShooter - Project Structure Documentation

This document explains the architecture and purpose of each file in this MonoGame project. Use this as a reference for creating similar game projects.

## Root Directory

### Core Project Files

- **`Program.cs`** - Application entry point, creates and runs GameRoot instance
- **`TopDownShooter.csproj`** - Project file with dependencies (MonoGame, Myra UI)
- **`TopDownShooter.sln`** - Visual Studio solution file
- **`app.manifest`** - Windows application manifest for DPI awareness
- **`Icon.bmp`** / **`Icon.ico`** - Application icons
- **`README.md`** - Project overview and setup instructions
- **`CLAUDE.md`** - AI assistant instructions and project guidance
- **`STRUCTURE.md`** - This documentation file

## Game Directory Structure

### `/Game/Core/` - Core Game Systems

- **`GameRoot.cs`** - Main game class (replaces Game1.cs), handles initialization, update/draw loops, and service management
- **`GameManager.cs`** - Singleton for global settings, debug controls (F1-F5, F11), and configuration management
- **`Time.cs`** - Static utility class for game timing and delta time calculations
- **`Extensions.cs`** - Extension methods for common operations and utilities

### `/Game/Framework/` - Entity-Component-System Framework

- **`Entity.cs`** - Base entity class with component system, Transform management, and lifecycle methods
- **`Scene.cs`** - Abstract base class for game scenes, manages entity collections and update/draw cycles
- **`SceneManager.cs`** - Handles scene transitions, loading/unloading, and scene lifecycle
- **`PerformanceTracker.cs`** - FPS tracking and performance monitoring with history buffer

#### `/Game/Framework/Components/` - Component System

- **`Transform.cs`** - Position, rotation, scale component for all entities
- **`ModelRenderer.cs`** - 3D model rendering component with color, size, and visibility control
- **`CharacterMotor.cs`** - Physics-based movement component with collision detection and response

### `/Game/Services/` - Global Service Layer

- **`AssetService.cs`** - Content loading and management service
- **`AudioService.cs`** - Sound effects and music management service
- **`InputService.cs`** - Keyboard, mouse, and gamepad input handling service
- **`MyraService.cs`** - UI framework service managing MyraUI windows and components

### `/Game/UI/` - User Interface

- **`MyraFpsWindow.cs`** - Performance monitoring window with real-time FPS chart and statistics

### `/Game/Physics/` - Physics System

- **`CollisionWorld.cs`** - Physics world management with spatial hashing for efficient collision detection
- **`ICollider.cs`** - Interface for collision detection components
- **`AabbCollider.cs`** - Axis-Aligned Bounding Box collider implementation
- **`CapsuleCollider.cs`** - Capsule collider implementation for character physics

#### `/Game/Physics/Spatial/` - Spatial Optimization

- **`SpatialHash.cs`** - Spatial hashing grid for efficient collision queries and broad-phase detection

### `/Game/Rendering/` - Rendering System

- **`Camera.cs`** - 3D camera with projection matrices, view controls, and camera management
- **`DebugDraw.cs`** - Debug visualization utilities for collision boxes, wireframes, and debug info
- **`Lighting.cs`** - Lighting system management and light source handling

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

### `/Game/` - Scene Implementations

- **`GameScene.cs`** - Main gameplay scene containing player, enemies, and world entities
- **`Level2Scene.cs`** - Second level implementation with specific entities and layout

## Content Directory

### `/Content/` - Game Assets

- **`Content.mgcb`** - MonoGame Content Pipeline project file for asset compilation
- **`bin/`** - Compiled content assets (XNB files)
- **`obj/`** - Intermediate build files

## Architecture Patterns

### Service Locator Pattern

Global services accessible via `GameRoot.Input`, `GameRoot.Audio`, `GameRoot.Assets`, `GameRoot.Myra`

### Entity-Component-System (ECS)

- Entities use composition over inheritance
- Add components via `AddComponent<T>()`
- Retrieve components via `GetComponent<T>()`

### Scene Management

- Scenes inherit from `Scene` base class
- Automatic update/draw lifecycle through SceneManager
- Entity management per scene

### Physics Integration

- Spatial hashing for efficient collision detection
- Component-based colliders (AABB, Capsule)
- Collision world manages all physics interactions

## Key Design Decisions

1. **MonoGame Framework**: Uses MonoGame 3.8.\* with DesktopGL for cross-platform support
2. **Fixed Timestep**: 60 FPS with vsync for consistent gameplay
3. **3D Rendering**: 3D world with 2D UI overlay approach
4. **MyraUI**: Modern UI framework replacing ImGui for better .NET 8 compatibility
5. **Component System**: Modular, reusable components for game entities
6. **Singleton Services**: Global access to input, audio, assets, and UI systems

## Debug Controls

- **F1**: Toggle debug info and aim visualization
- **F2**: Toggle collision box visualization
- **F3**: Toggle FPS display
- **F4**: Toggle performance statistics
- **F5**: Toggle FPS chart window
- **F11**: Toggle fullscreen
- **Escape**: Exit game

## Getting Started with This Structure

1. Copy the framework files (`/Game/Framework/`, `/Game/Core/`, `/Game/Services/`)
2. Implement your game-specific scenes in `/Game/`
3. Add gameplay systems in `/Game/Gameplay/`
4. Extend the component system for your needs
5. Use the service locator pattern for global systems
6. Follow the existing patterns for new entities and components

This structure provides a solid foundation for 3D MonoGame projects with modern architecture patterns, comprehensive debugging tools, and extensible systems.
