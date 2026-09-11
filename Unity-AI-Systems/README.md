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

## Reusable vision sensor

Add `Reusable/VisionSensor` to an NPC that needs perception.

Configure:

- `Eye`: origin and forward direction of vision
- `Range`: maximum detection distance
- `Field Of View`: view cone in degrees
- `Target Mask`: layers that can be detected
- `Obstruction Mask`: layers that can block line of sight
- `Scan Interval`: how often the sensor scans

Use `CurrentTarget`, `TargetAcquired` and `TargetLost` from other AI behaviours.

## Reusable patrol agent

Add `Reusable/PatrolAgent` to a GameObject with `NavMeshAgent`, assign patrol point transforms and choose sequential or random movement. The component handles waiting, invalid paths and stopping cleanly when disabled.

## Notes

The restaurant-specific NPC scripts are intentionally kept separate from the reusable `UnityGameSystems.AI` components. The reusable components do not depend on the restaurant order classes and can be copied into another NavMesh project independently.
