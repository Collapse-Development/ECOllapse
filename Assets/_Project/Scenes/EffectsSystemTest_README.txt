Effects System Test Scene
=========================

This scene demonstrates the Universal Effects System implementation with real character movement.
Uses Unity's New Input System for keyboard input.

Scene Setup:
1. Add the Character prefab from Assets/_Project/Prefabs/Character.prefab to the scene
2. Add an EffectsSystem component as a child of the Character GameObject
3. Add the EffectsSystemTest script to the Character GameObject
4. Create a Canvas with UI Text element for displaying speed information
5. Assign the UI Text to the EffectsSystemTest script's "Speed Multiplier Text" field
6. Set the base speed in EffectsSystemTest (default: 5.0)

Character Prefab Includes:
- Character component (system manager)
- CharacterMovementSystem component (handles movement)
- Rigidbody (for physics-based movement)
- Model child with Cube mesh (visual representation)

Requirements:
- Unity New Input System package must be installed
- Player Settings > Active Input Handling set to "Input System Package (New)" or "Both"

Keyboard Controls:
- WASD: Move the character around
- Press 1: Add ConstantMultiplier (1.5x permanent speed boost)
- Press 2: Add MultiplyForDuration (2.0x speed for 3 seconds)
- Press 3: Add MultiplyForDuration (0.5x speed for 2 seconds - slow effect)
- Press 4: Add CurveMultiplier (ease-in-out from 1x to 2x over 4 seconds)
- Press C: Cancel the last added modifier

How to Use:
1. Open the EffectsSystemTest scene in Unity
2. Enter Play mode
3. Use WASD to move the character
4. Watch the "Speed Multiplier" and "Current Speed" text in the UI
5. Press the number keys to add different modifiers
6. Observe how the character's movement speed changes in real-time
7. Multiple modifiers stack multiplicatively

Expected Behavior:
- Base speed multiplier is 1.00x (character moves at base speed)
- Modifiers multiply together (e.g., 1.5x * 2.0x = 3.0x total multiplier)
- Character movement speed = base speed * speed multiplier
- Temporary modifiers expire after their duration
- Cancelled modifiers are removed immediately
- UI updates every frame to show current multiplier and actual speed
- Character visibly moves faster or slower based on active modifiers
