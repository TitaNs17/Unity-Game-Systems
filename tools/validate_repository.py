from pathlib import Path
import sys

ROOT = Path(__file__).resolve().parents[1]

errors = []

required_paths = [
    "Unity-Gameplay-Systems/Interaction/IInteractable.cs",
    "Unity-Gameplay-Systems/Compatibility/IInteractable.cs",
    "Unity-Gameplay-Systems/Legacy/Character/SliceableObject.cs",
    "Unity-AI-Systems/Reusable/AdvancedNPCBrain.cs",
    "Unity-Restaurant-Systems/Seating/SeatingService.cs",
    "Unity-Restaurant-Systems/Staff/StaffMember.cs",
    "Unity-Restaurant-Systems/Stock/IngredientStock.cs",
    "Unity-Restaurant-Systems/Shift/ShiftTracker.cs",
]

for relative in required_paths:
    if not (ROOT / relative).is_file():
        errors.append(f"Missing required file: {relative}")

for deprecated in [
    "Unity-Restaurant-Systems/V2",
    "Unity-Gameplay-Systems/Character",
]:
    if (ROOT / deprecated).exists():
        errors.append(f"Deprecated directory still exists: {deprecated}")

for path in ROOT.rglob("*.cs"):
    if " " in path.name:
        errors.append(f"C# filename contains spaces: {path.relative_to(ROOT)}")

new_interaction = ROOT / "Unity-Gameplay-Systems/Interaction/IInteractable.cs"
legacy_interaction = ROOT / "Unity-Gameplay-Systems/Compatibility/IInteractable.cs"

if new_interaction.is_file():
    content = new_interaction.read_text(encoding="utf-8-sig")
    if "namespace UnityGameSystems.Interaction" not in content:
        errors.append("Reusable IInteractable must remain namespaced.")

if legacy_interaction.is_file():
    content = legacy_interaction.read_text(encoding="utf-8-sig")
    if "GetInteractionText" not in content or "void Interact()" not in content:
        errors.append("Legacy IInteractable compatibility contract changed unexpectedly.")

if errors:
    print("Repository validation failed:")
    for error in errors:
        print(f"- {error}")
    sys.exit(1)

print("Repository structure validation passed.")
