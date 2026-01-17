# Say_eek
Ghost game

## About
Say_eek is a Unity-based ghost game where you control a floating ghost character. The game features a spooky ghost protagonist that can move around and collect items.

## Unity Version
This project is built with Unity 2022.3.10f1 (LTS)

## Getting Started

### Prerequisites
- Unity Hub
- Unity 2022.3.10f1 or compatible version

### Opening the Project
1. Clone this repository
2. Open Unity Hub
3. Click "Add" and navigate to the cloned repository folder
4. Open the project in Unity
5. The main scene is located at `Assets/Scenes/MainScene.unity`

### Project Structure
```
Say_eek/
├── Assets/
│   ├── Scenes/          # Game scenes
│   │   └── MainScene.unity
│   └── Scripts/         # C# scripts
│       ├── GhostController.cs   # Ghost player movement and behavior
│       ├── GameManager.cs       # Game state management
│       └── CameraFollow.cs      # Camera control script
├── Packages/            # Unity package dependencies
├── ProjectSettings/     # Unity project settings
└── README.md
```

## Gameplay
- Use **WASD** or **Arrow Keys** to move the ghost
- The ghost floats naturally with a bobbing animation
- Press **ESC** to pause the game
- Collect items to increase your score

## Scripts

### GhostController.cs
Controls the ghost character movement and floating animation.

### GameManager.cs
Manages game state including score, lives, pause functionality, and game over conditions.

### CameraFollow.cs
Handles smooth camera following of the ghost player.

## Contributing
Feel free to fork this project and submit pull requests with improvements!

## License
This project is open source and available for educational purposes.
