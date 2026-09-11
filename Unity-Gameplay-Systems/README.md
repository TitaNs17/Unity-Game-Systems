# Unity Gameplay Systems

Reusable Unity components that can be dropped into a project without depending on a specific scene or game.

## Health and damage

1. Add `Health` to the player or NPC root object.
2. Set `Max Health` in the Inspector.
3. Add `DamageDealer` to hazards, projectiles or melee hitboxes.
4. Enable trigger or collision damage and choose the target layers.

`Health` exposes damage, heal, health-changed and death events so UI, audio and game flow can subscribe without modifying the component.

## Stamina

Add `Stamina` to the player. Call `TrySpend(amount)` from sprint, dodge, melee or ability code. Regeneration starts automatically after the configured delay.

## Checkpoints and respawn

1. Add `Health` and `RespawnController` to the player.
2. Assign a fallback spawn Transform if required.
3. Create a checkpoint GameObject with a trigger Collider and add `Checkpoint`.
4. Keep the player tag as `Player`, or change `Required Tag` in the Inspector.

When the player enters a checkpoint it becomes the active spawn location. When `Health` reaches zero, `RespawnController` moves the player to the latest checkpoint, clears Rigidbody velocity and restores health.

The respawn controller supports Rigidbody and CharacterController based players.

## Weapons

1. Create a weapon asset from `Create > Game > Weapon`.
2. Configure damage, fire rate, magazine size, reload duration, range and spread.
3. Add `HitscanWeapon` to the weapon GameObject.
4. Assign the weapon asset and optionally assign an aim camera, muzzle transform, particle effect and AudioSource.
5. Call `TryFire()` from your input code and `TryReload()` from the reload action.

`HitscanWeapon` handles fire-rate limiting, magazine state, automatic reload, raycasts, damage application, muzzle flash, audio and hit events. Input is not embedded in the weapon so the same component works with keyboard/mouse, controller, mobile UI or Unity's new Input System.

## Enemy AI

1. Add a `NavMeshAgent` to the enemy.
2. Add `Health` and `EnemyBrain` to the same root object.
3. Assign the player Transform as `Target`.
4. Configure detection range, attack range, damage and cooldown.
5. Bake a NavMesh for the scene.

The enemy stays idle outside detection range, chases the target while detected, checks line of sight before attacking and applies damage through the shared `Health` component. Animator support is optional. If an Animator is assigned, the expected parameters are `Moving` (bool), `Attack` (trigger) and `Die` (trigger).

## Loot

1. Create a loot asset from `Create > Game > Loot Table`.
2. Add prefab entries, weights and min/max quantities.
3. Add `LootDropper` to an enemy that already has `Health`.
4. Assign the loot table.

When the enemy dies, the component performs one weighted roll and instantiates the selected prefab. Rigidbody loot can receive an optional upward impulse and random scatter.

## XP and levels

Add `ExperienceComponent` to the player. Configure base required XP and the growth multiplier. Other systems can call `AddExperience(amount)` without knowing how levels are calculated.

To reward XP from an enemy, add `ExperienceReward` to the enemy, assign its `Health`, assign the player's `ExperienceComponent`, and set the XP amount. The reward is granted once when the enemy dies.

`ExperienceProgression` contains the engine-independent progression logic and supports multiple level-ups from a single XP grant, state restoration and unit testing.

## Pause

Add `PauseManager` once in the scene. Connect a UI button or your own input code to `Toggle`, `Pause` or `Resume`. It handles `Time.timeScale` and can optionally manage cursor visibility and locking.

Input is intentionally not hard-coded so the component works with both the old Unity Input Manager and the new Input System.

## Countdown timer

`CountdownTimer` is a plain C# class. It can be used for rounds, cooldowns, objectives and timed events without a MonoBehaviour dependency.

```csharp
private CountdownTimer timer;

private void Awake()
{
    timer = new CountdownTimer(30f);
    timer.Finished += OnTimerFinished;
    timer.Start();
}

private void Update()
{
    timer.Tick(Time.deltaTime);
}
```

## Health UI

Add a Unity UI `Slider`, then add `HealthBarBinder` to the same GameObject. Assign the target `Health` component. Add a `CanvasGroup` if `Hide When Full` should fade the bar out while keeping event subscriptions active.

## Compatibility

The gameplay components use standard Unity APIs. `EnemyBrain` requires Unity NavMesh support because it uses `NavMeshAgent`. No third-party packages are required. Input is intentionally left outside reusable systems. Tests are wrapped in `UNITY_INCLUDE_TESTS` so normal player builds do not require NUnit references.
