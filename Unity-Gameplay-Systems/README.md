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

## Weapons

1. Create a weapon asset from `Create > Game > Weapon`.
2. Configure damage, fire rate, magazine size, reload duration, range and spread.
3. Add `HitscanWeapon` to the weapon GameObject.
4. Assign the weapon asset and optional effects.
5. Call `TryFire()` and `TryReload()` from your input code.

Input is not embedded in the weapon so the same component can work with keyboard/mouse, controller, mobile UI or Unity's Input System.

## Enemy AI

1. Add a `NavMeshAgent`, `Health` and `EnemyBrain` to the enemy.
2. Assign the player Transform as `Target`.
3. Configure detection range, attack range, damage and cooldown.
4. Bake a NavMesh for the scene.

Animator support is optional. If assigned, the expected parameters are `Moving` (bool), `Attack` (trigger) and `Die` (trigger).

## Loot and progression

Create a loot asset from `Create > Game > Loot Table`, add `LootDropper` to an enemy and assign its `Health` component. `ExperienceComponent` can be added to a player and `ExperienceReward` can grant XP when an enemy dies.

## Save Profile V2

`SaveProfileService` stores a versioned JSON profile under `Application.persistentDataPath`. It writes through a temporary file and keeps a backup of the previous save before replacement.

```csharp
var service = new SaveProfileService();
var profile = new SaveProfile();
profile.playerPosition = transform.position;
profile.Set("coins", "250");
service.Save(profile);
```

Use `TryLoad(out SaveProfile profile)` to load. If the primary save is unreadable, the service tries the backup automatically.

## Settings

Add `GameSettings` once in a persistent scene. It stores settings through `PlayerPrefs` and applies master volume, fullscreen, quality level and target frame rate. UI controls can call `SetMasterVolume`, `SetQualityLevel`, `SetFullscreen`, `SetTargetFrameRate` and then `Save()`.

## Dialogue

1. Create a dialogue asset from `Create > Game > Dialogue`.
2. Add speaker/text lines and optional voice clips.
3. Add `DialogueRunner` to a scene object.
4. Connect its UnityEvents to your dialogue UI, or subscribe to its C# events.
5. Call `StartDialogue(asset)` and then `Next()` from your input/UI.

The runner does not require a specific dialogue UI implementation.

## Quests and quest UI

Create quest assets from `Create > Game > Quest`, then add `QuestTracker` to the player or game manager. Call `StartQuest(definition)` and `AddProgress(questId, objectiveId, amount)` from gameplay code.

`QuestUIBinder` works with standard Unity UI `Text` components. Assign the tracker, title text and objective text. It automatically refreshes when the selected quest changes.

## Wave spawner

1. Add `WaveSpawner` to an empty GameObject.
2. Create one or more child Transforms and assign them as spawn points.
3. Add wave entries with an enemy prefab, count and spawn interval.
4. Each enemy prefab must contain a `Health` component.
5. Enable `Play On Start` or call `StartWaves()` manually.

The spawner waits until every tracked enemy in the current wave reports death before advancing to the next wave.

## Pause

Add `PauseManager` once in the scene. Connect UI or input code to `Toggle`, `Pause` or `Resume`. It handles `Time.timeScale` and optional cursor state.

## Countdown timer

`CountdownTimer` is a plain C# class for rounds, cooldowns, objectives and timed events.

## UI

`HealthBarBinder` connects `Health` to a Unity UI Slider. `QuestUIBinder` connects the quest tracker to legacy Unity UI Text components.

## Compatibility

The gameplay components use standard Unity APIs. `EnemyBrain` requires Unity NavMesh support. UI binders use Unity uGUI. No third-party packages are required by the core systems. Input is intentionally left outside reusable systems. Tests are wrapped in `UNITY_INCLUDE_TESTS` so player builds do not require NUnit references.
