# Unity Game Systems

A collection of gameplay code I have built while working on Unity projects. The repository is mainly a place for systems that are useful outside a single scene or prototype, so I can revisit and improve them as my projects grow.

## What's here

### Gameplay
- First-person character and interaction code
- Item pickup, cutting and inventory systems
- Day/night and environment behaviours
- Typed gameplay event bus
- Reusable state machine
- Runtime stats and modifiers
- Generic component pooling
- Versioned JSON save/load service

### AI
- NavMesh based NPC movement
- Customer behaviour
- Seating and wandering logic
- NPC UI feedback

### Restaurant / simulation
- Orders and customers
- Cooking and grill slots
- Economy and market logic
- Cleaning / dirt systems
- Ingredient and drink interactions
- Menu definitions and reusable order states
- Customer patience and satisfaction
- Kitchen station workflow
- Seating and reservation logic
- Staff roles, skill and energy
- Ingredient stock and waste tracking
- Shift summaries with revenue, expenses and completion metrics

## Architecture notes

Not every script in this repository comes from the same game. Older project-specific components are kept alongside newer reusable modules on purpose: they show how the code evolved from direct MonoBehaviour implementations toward smaller interfaces, plain C# services and event-driven systems.

New reusable code avoids scene lookups where possible. Unity-facing behaviour stays in MonoBehaviours while logic that does not need the engine is kept as regular C# classes.

## Reusable gameplay modules

`Unity-Gameplay-Systems/Core/GameEventBus.cs` provides typed communication without requiring gameplay systems to hold references to one another.

`Unity-Gameplay-Systems/Interaction/` separates an interactable object's behaviour from the player's raycast logic through `IInteractable`.

`Unity-Gameplay-Systems/StateMachine/` contains a small engine-independent state machine suitable for NPCs, player states or game flow.

`Unity-Gameplay-Systems/Stats/RuntimeStat.cs` supports flat and percentage modifiers and can remove modifiers by their source object.

`Unity-Gameplay-Systems/Utilities/ComponentPool.cs` is a generic pool for Unity components used for frequently spawned objects.

`Unity-Gameplay-Systems/Save/SaveGameService.cs` contains a versioned JSON save model and uses a temporary file before replacing the current save.

## Restaurant simulation v2

The `Unity-Restaurant-Systems/V2` folder is a cleaner simulation layer that can be reused independently from the original restaurant prototype.

It includes menu data, order state transitions, customer patience, kitchen preparation, economy, reputation, seating, reservations, staff, stock, waste and shift reporting. Most of these systems are plain C# so they can be unit tested without loading a Unity scene.

## Tech

- Unity
- C#
- NavMesh
- ScriptableObject-based data
- Event-driven gameplay patterns
- NUnit / Unity Test Framework

This is a code-focused repository rather than a complete Unity project, so scenes, art assets and project-specific packages are intentionally not included.
