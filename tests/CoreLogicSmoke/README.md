# Core Logic Smoke Tests

This executable test harness compiles and runs engine-independent systems without requiring the Unity Editor.

Covered areas:

- AI state transitions
- countdown timer behavior
- runtime stat modifiers
- restaurant seating and reservations
- ingredient stock and reorder behavior
- waste tracking
- shift revenue/expense/profit calculations
- staff energy and training

The harness is run by GitHub Actions on pull requests and on `main`.

Unity-specific MonoBehaviours, NavMesh behavior, physics, animation, scene loading and UI still require a real Unity project and Unity Test Runner for full integration coverage.
