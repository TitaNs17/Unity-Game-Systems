# Unity Game Systems

A collection of gameplay, AI and simulation code built while working on Unity projects. The repository keeps reusable systems separate from older project-specific prototypes so the architecture is easier to evaluate and reuse.

## Main modules

### Unity Gameplay Systems

Reusable gameplay systems including combat, health, stamina, interaction, inventory, quests, dialogue, save/load, settings, progression, loot, pooling and game-flow utilities.

Older project-specific character and interaction code lives under `Unity-Gameplay-Systems/Legacy`. Compatibility contracts used by those scripts live under `Unity-Gameplay-Systems/Compatibility` and are kept separate from the newer namespaced APIs.

### Unity AI Systems

NavMesh-based AI code organized into `Core`, `Customers`, `Movement`, `Seating`, `UI`, `Reusable` and `Tests`.

Reusable AI includes vision, hearing, suspicion, perception, patrol, crowd tuning, state-machine logic and an `AdvancedNPCBrain` that composes patrol, investigate, chase, attack and search behavior.

### Unity Restaurant Systems

Restaurant simulation code organized by responsibility: menu/order flow, customers, kitchen, seating, staff, stock/waste, management, shift reporting and tests.

The original project-specific restaurant scripts are isolated under `Legacy`. The newer restaurant layer no longer uses a separate `V2` folder; current systems live directly in their responsibility folders.

## Architecture notes

Not every script in this repository comes from the same game. Older prototype components are kept to show iteration history, while newer reusable code favors smaller interfaces, plain C# services, ScriptableObject data, event-driven communication and isolated runtime components.

Unity-facing behavior remains in MonoBehaviours when engine access is required. Logic that does not need the engine is kept as regular C# classes where possible so it can be unit tested independently.

## Selected reusable modules

- `Unity-Gameplay-Systems/Core/GameEventBus.cs` — typed gameplay events
- `Unity-Gameplay-Systems/Interaction/` — namespaced reusable interaction layer
- `Unity-Gameplay-Systems/StateMachine/` — engine-independent state machine
- `Unity-Gameplay-Systems/Stats/RuntimeStat.cs` — flat and percentage stat modifiers
- `Unity-Gameplay-Systems/Utilities/ComponentPool.cs` — generic component pooling
- `Unity-Gameplay-Systems/Save/` — versioned JSON persistence
- `Unity-AI-Systems/Reusable/` — perception, patrol and high-level AI behavior
- `Unity-Restaurant-Systems/` — testable restaurant simulation services

## Tech

- Unity
- C#
- NavMesh
- ScriptableObject-based data
- Event-driven gameplay patterns
- NUnit / Unity Test Framework

This is a code-focused repository rather than a complete Unity project, so scenes, art assets and project-specific packages are intentionally not included.
