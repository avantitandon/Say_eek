# Say_eek Ghost Game - Unity Setup Guide

## Overview
Say_eek is a Unity 3D ghost game where players control a floating ghost character. This guide will help you set up and understand the project.

## Requirements
- **Unity Version**: 2022.3.10f1 (LTS) or compatible
- **Unity Hub**: Latest version
- **Platform**: Windows, macOS, or Linux

## Quick Start

### 1. Clone the Repository
```bash
git clone https://github.com/avantitandon/Say_eek.git
cd Say_eek
```

### 2. Open in Unity
1. Launch Unity Hub
2. Click "Open" or "Add"
3. Navigate to the cloned repository folder
4. Select the root folder (Say_eek)
5. Unity will import the project (this may take a few minutes on first load)

### 3. Open the Main Scene
- In Unity, navigate to `Assets/Scenes/MainScene.unity`
- Double-click to open the scene

### 4. Play the Game
- Click the Play button (▶) at the top of the Unity Editor
- Use WASD or Arrow Keys to move the ghost
- Press ESC to pause

## Project Architecture

### Scripts Overview

#### GhostController.cs
**Purpose**: Controls the ghost player character
- **Movement**: WASD/Arrow key input for horizontal movement
- **Floating Animation**: Sine wave-based bobbing effect
- **Collectibles**: Detects and collects items with "Collectible" tag
- **Serialized Fields**:
  - `moveSpeed`: How fast the ghost moves (default: 5)
  - `floatSpeed`: Speed of floating animation (default: 1)
  - `floatAmount`: Height of floating bob (default: 0.5)

#### GameManager.cs
**Purpose**: Manages overall game state (Singleton pattern)
- **Score System**: Tracks and updates player score
- **Lives System**: Manages player lives
- **Pause System**: Press ESC to pause/unpause
- **Scene Management**: Restart functionality
- **Persistence**: Uses DontDestroyOnLoad for cross-scene persistence

#### CameraFollow.cs
**Purpose**: Smooth camera following system
- **Target Following**: Smoothly follows the ghost player
- **Auto-Detection**: Automatically finds player with "Player" tag if not assigned
- **Serialized Fields**:
  - `target`: Transform to follow (auto-assigned if empty)
  - `offset`: Camera position relative to target (default: 0, 5, -10)
  - `smoothSpeed`: How smoothly camera follows (default: 0.125)
  - `lookAtTarget`: Whether camera should look at target (default: true)

### Scene Structure

**MainScene.unity** includes:
- **Main Camera**: Positioned at (0, 1, -10)
- **Directional Light**: Positioned at (0, 3, 0) with 50° rotation

### Tags Used
- **MainCamera**: Applied to main camera
- **Player**: Should be applied to ghost character GameObject
- **Collectible**: Applied to collectible items

## Development Workflow

### Adding the Ghost Character
1. Create a 3D GameObject (e.g., Sphere or custom model)
2. Add the `GhostController` script component
3. Tag it as "Player"
4. Add a Rigidbody component (set to Kinematic if needed)
5. Add a Collider component for collision detection

### Adding Collectibles
1. Create a GameObject for the collectible
2. Tag it as "Collectible"
3. Add a Collider component
4. Check "Is Trigger" on the Collider

### Setting Up the Camera
1. Select the Main Camera
2. Add the `CameraFollow` script component
3. Either assign the ghost Transform manually or leave empty for auto-detection

### Adding the Game Manager
1. Create an empty GameObject named "GameManager"
2. Add the `GameManager` script component
3. It will persist across scenes automatically

## Controls
| Key | Action |
|-----|--------|
| W / ↑ | Move Forward |
| S / ↓ | Move Backward |
| A / ← | Move Left |
| D / → | Move Right |
| ESC | Pause/Unpause |

## Building the Game

### Build Settings
1. Go to File → Build Settings
2. Ensure "MainScene" is in the Scenes list
3. Select your target platform
4. Click "Build" or "Build and Run"

### Build Platforms
- **PC, Mac & Linux Standalone**: Full desktop support
- **WebGL**: Browser-based play
- **Mobile**: iOS and Android (requires additional setup)

## Extending the Game

### Adding New Features
1. **Power-ups**: Create new collectible types with different effects
2. **Enemies**: Add enemy GameObjects with AI scripts
3. **Levels**: Create new scenes and add to Build Settings
4. **UI**: Add Canvas with TextMeshPro for score/lives display
5. **Sound**: Add AudioSource components for music and SFX

### Best Practices
- Keep scripts modular and single-purpose
- Use SerializeField for Inspector-visible private fields
- Comment complex logic
- Test in Editor before building

## Troubleshooting

### Common Issues

**Issue**: Scripts show as "missing" in Inspector
- **Solution**: Reimport scripts or check for compilation errors in Console

**Issue**: Game doesn't respond to input
- **Solution**: Ensure "Player" tag is applied and GhostController is attached

**Issue**: Camera doesn't follow ghost
- **Solution**: Check "Player" tag on ghost GameObject

**Issue**: Scene won't load
- **Solution**: Add scene to Build Settings (File → Build Settings)

## Package Dependencies
The project uses these Unity packages (defined in `Packages/manifest.json`):
- Unity UI (UGUI)
- TextMeshPro
- Visual Studio Code Editor
- Test Framework
- Timeline
- Visual Scripting

## Contributing
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly in Unity
5. Submit a pull request

## License
Open source - available for educational purposes

## Support
For issues or questions, please open an issue on GitHub.
