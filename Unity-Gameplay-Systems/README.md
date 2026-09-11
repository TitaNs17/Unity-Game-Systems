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

The systems only use UnityEngine and standard Unity UI APIs. They do not require third-party packages or a specific input package. Tests are wrapped in `UNITY_INCLUDE_TESTS` so normal player builds do not require NUnit references.
