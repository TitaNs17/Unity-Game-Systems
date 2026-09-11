# Unity AI Systems

Reusable NavMesh-driven NPC and customer behaviours used by the simulation examples in this repository.

## Customer flow

`NPCController` supports a simple customer lifecycle:

`Street -> Shop -> Waiting For Order -> Eating -> Leaving`

Setup:

1. Add `NavMeshAgent` and `NPCController` to the NPC root.
2. Add an Animator only if animation parameters are needed.
3. Create street waypoint transforms under one parent and assign that parent to `NPCManager`.
4. Add `SeatingManager` to the restaurant and place `SeatPoint` components under it.
5. Bake a NavMesh covering the street, entrance and restaurant floor.
6. If the restaurant order prototype is used, keep one `OrderManager` in the scene.

Seats are reserved atomically through `SeatPoint.TryReserve` and are released when a customer leaves or is destroyed.

## NPC manager

`NPCManager` validates prefab and waypoint data, tracks live NPC instances and initializes either `NPCController`, `NPCWander`, or both when present on a prefab.

## Simple customer AI

`CustomerAI` is a smaller seat/eat/leave example. It handles missing seating, NavMesh validity and releases its seat during cleanup.

## Wander behaviour

`NPCWander` moves through waypoint targets for a random number of stops, waits between destinations and asks `NPCManager` for a replacement NPC when its route is finished.

## Vision and perception

Add `Reusable/VisionSensor` to an NPC that needs sight-based perception. Configure eye transform, range, field of view, target layers, obstruction layers and scan interval.

Add `Reusable/HearingSensor` when the NPC should react to sound. Any object can use `NoiseEmitter.Emit()` or call `NoiseSystem.Emit(...)` directly. Hearing strength falls off with distance and can optionally be blocked by geometry.

`PerceptionController` combines vision, hearing and `SuspicionMeter`. Visual contact raises suspicion continuously, audible events add suspicion instantly and the controller exposes the latest interesting world position for investigation behaviours.

## AI state machine

`Reusable/AIStateMachine` is a small engine-independent state machine for behaviours such as idle, patrol, investigate, chase, attack and flee. States implement `IAIState` and can be registered once, switched by type and ticked from a MonoBehaviour.

## Patrol

Add `Reusable/PatrolAgent` to a GameObject with `NavMeshAgent`, assign patrol point transforms and choose sequential or random movement. The component handles waiting, invalid paths and stopping cleanly when disabled.

## Crowd avoidance

Add `Reusable/CrowdAgentTuner` beside a `NavMeshAgent` when many NPCs share the same area. It assigns a randomized avoidance priority, configurable obstacle avoidance quality and minimum personal-space radius to reduce agents locking into identical paths.

## Customer personality and needs

Create personality assets from `Create > AI > Customer Personality`. A profile stores patience, generosity, cleanliness tolerance and social need as normalized values.

`Customers/CustomerNeeds` is a runtime model for hunger, comfort and patience. Simulation code can drain patience while waiting, increase hunger over time, restore comfort and feed the customer without coupling those values to a specific NPC controller.

## Recommended composition

For a general-purpose NPC prefab:

1. Add `NavMeshAgent`.
2. Add `VisionSensor` and `HearingSensor` when perception is needed.
3. Add `PerceptionController` to combine both sensors.
4. Add `PatrolAgent` for default roaming behaviour.
5. Add `CrowdAgentTuner` for busy scenes.
6. Drive high-level behaviour through `AIStateMachine` from the game-specific controller.

The restaurant-specific NPC scripts remain separate from the reusable `UnityGameSystems.AI` components. The reusable systems do not depend on restaurant order classes and can be copied into another NavMesh project independently.
