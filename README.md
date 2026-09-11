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

## Architecture notes

Not every script in this repository comes from the same game. Older project-specific components are kept alongside newer reusable modules on purpose: they show how the code evolved from direct MonoBehaviour implementations toward smaller interfaces, plain C# services and event-driven systems.

New reusable code lives under `Unity-Gameplay-Systems` and avoids scene lookups where possible. Unity-facing behaviour stays in MonoBehaviours while logic that does not need the engine is kept as regular C# classes.

## Reusable modules

`Core/GameEventBus.cs` provides typed communication without requiring gameplay systems to hold references to one another.

`Interaction/` separates an interactable object's behaviour from the player's raycast logic through `IInteractable`.

`StateMachine/` contains a small engine-independent state machine suitable for NPCs, player states or game flow.

`Stats/RuntimeStat.cs` supports flat and percentage modifiers and can remove modifiers by their source object.

`Utilities/ComponentPool.cs` is a generic pool for Unity components used for frequently spawned objects.

`Save/SaveGameService.cs` contains a versioned JSON save model and uses a temporary file before replacing the current save.

## Tech

- Unity
- C#
- NavMesh
- ScriptableObject-based data in project-specific systems
- Event-driven gameplay patterns

This is a code-focused repository rather than a complete Unity project, so scenes, art assets and project-specific packages are intentionally not included.
