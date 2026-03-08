# Assignment 1: AIM TRAINER

A fast-paced aim training application built in Unity featuring both a 3D First-Person Shooter (FPS) mode and a 2D "OSU-style" clicking mode. Players can test their reflexes, track reaction times, and improve their accuracy across dynamically scaling difficulties.

## 🚀 Key Features

* **Dual Game Modes:** Seamlessly switch between a 3D FPS environment and a 2D target-clicking mode.
* **Dynamic Difficulty Scaling:** In 2D mode, targets smoothly shrink in size and their time-to-live (TTL) decreases as the game progresses. In 3D mode, the TTL decreases progressively to continuously challenge the player.
* **Comprehensive Scoring System:** Players earn base points for hits, with bonus points awarded for high combos and lightning-fast reaction times.
* **Strict Penalty Mechanics:** Missing a target incurs a point penalty and resets the player's combo back to zero. 
* **Custom 2D Cursor:** The 2D mode utilizes a custom aiming crosshair that automatically confines itself to the game window during active gameplay.
* **Immersive Audio & Visuals:** Features centralized audio management for background music, UI clicks, shooting, and summary screens, paired with 3D weapon recoil, muzzle flashes, and fire animations.
* **Optimized Performance:** Utilizes Unity's native Object Pooling pattern for target lifecycles, preventing memory allocation spikes during rapid spawning and destruction.

## 🎮 Controls

The game relies on Unity's modern Input System. 

* **Movement (3D):** Standard mapping for navigating the 3D environment.
* **Look (3D):** Mouse control to look around, featuring customizable sensitivity and clamped vertical angles to prevent over-rotation.
* **Actions (3D):** Jump and Sprint are integrated into the 3D player controller.
* **Fire (3D):** Hold to continuously fire based on a designated fire rate, complete with accumulating recoil to simulate weapon kickback.
* **Fire (2D):** Left-click to shoot targets. Hit detection calculates the distance between the mouse's world position and the target's center against its sprite radius.

## 📐 Technical Implementation Details

### Scoring System
The game calculates the score dynamically upon each hit, rewarding accuracy and speed. When a target is successfully hit, the points added are calculated as:
$$S_{hit} = S_{base} + [S_{combo} \times \max(0, C - 1)] + S_{speed}$$
Where:
* $S_{base} = 100$ (Base score per hit).
* $S_{combo} = 30$ (Bonus multiplier for consecutive hits, applied when the current combo count $C > 1$).
* $S_{speed} = 50$ (Awarded if the player's reaction time $R < 1.0$ second).

Conversely, missing a target applies a penalty and breaks the combo:
$$S_{new} = \max(0, S_{current} - S_{miss})$$
Where $S_{miss} = 50$, ensuring the total score never drops below zero.

### Dynamic Target Spawning & Scaling
The 2D target spawner calculates a difficulty coefficient based on elapsed time over the total game duration. It then uses linear interpolation to scale down the target's size and time-to-live. Additionally, a light random jitter of between **0.90** and **1.10** is applied to the scale multiplier to keep target sizes unpredictable. 

### Hit Detection & Raycasting
In 3D mode, shooting utilizes `Physics.Raycast` from a designated gun raycast point to instantly detect targets on a specific LayerMask. In 2D mode, the screen click position is converted to a world point, and a mathematical distance check is performed against the active target's sprite bounds to register a hit.

### Game State Machine
Both 3D and 2D modes run on a state machine (`START`, `PLAYING`, `PAUSED`, `RESULTS`, `SETTINGS`) that dictates time scale, UI visibility, and cursor locking rules.

## 📂 Core Architecture (Scripts)

* `GameManager.cs` / `GameManager2D.cs`: Singletons controlling the core game loop, state machines, scoring, and performance statistics tracking.
* `Player.cs` / `Gun.cs`: Handles first-person locomotion, camera control with clamped vertical rotation, and shooting mechanics with recoil and visual effects.
* `Aim2DInput.cs` / `Cursor2DController.cs`: Manages 2D mouse input, cursor state modifications, and radial hit calculations.
* `Target.cs` / `Target2D.cs`: Manages individual target logic, tracking time-to-live and calculating player reaction times.
* `TargetBool.cs` / `TargetBool2D.cs`: Singleton object pool managers that handle the procedural generation and placement of targets.
* `AudioManager.cs`: A persistent singleton that centralizes all BGM and SFX playback, automatically transitioning audio between menus and gameplay.

## 🛠️ Setup & Installation

1. Clone the repository.
2. Open the project in Unity (Ensure you have the "Input System" and "TextMeshPro" packages installed).
3. Open the Main Menu scene and press **Play**.
4. Alternatively, you can play the finished build located in ./Aim Trainer

## 📦 Asset Sources

* **3D Gun Model:** https://skfb.ly/oypzZ
* **Sprites & Art Assets:** https://kenney.nl/assets/top-down-tanks-redux, https://kenney.nl/assets/top-down-shooter
* **Sound Effects (SFX):** https://harvey656.itch.io/8-bit-game-sound-effects-collection
* **Background Music:** https://beatscribe.itch.io/beatscribes-free-uge-music-asset-pack?download
