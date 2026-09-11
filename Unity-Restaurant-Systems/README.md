# Restaurant Systems

Restaurant and management systems used by the simulation examples in this repository.

## Layout

- `Core/` — menu definitions and order state flow
- `Customers/` — customer patience and satisfaction logic
- `Kitchen/` — preparation stations and kitchen workflow
- `Seating/` — table capacity, reservations and party placement
- `Staff/` — staff roles, skill, energy and wages
- `Stock/` — ingredient inventory, reorder thresholds and waste tracking
- `Management/` — economy and reputation
- `Shift/` — end-of-shift revenue, expense, waste and completion metrics
- `Tests/` — unit tests for reusable restaurant logic
- `Legacy/` — original project-specific restaurant scripts

The former `V2/` layer was flattened into the folders above so there is one obvious current architecture. `Legacy/` remains isolated to preserve the original prototype without mixing it into the reusable layer.

## Suggested simulation flow

A typical service cycle can be composed as:

`Customer arrives -> SeatingService selects table -> order is created -> KitchenStation prepares item -> order is served -> economy/reputation update -> ShiftTracker records result`

The reusable restaurant classes are mostly plain C# and do not require a loaded Unity scene. This keeps business rules easy to test and lets MonoBehaviours remain thin integration layers.

## Seating

Create `RestaurantTable` instances with capacities and register them with `SeatingService`. The service chooses the smallest available table that can fit a walk-in party and skips reserved tables.

## Staff

`StaffMember` supports cashier, cook, server and cleaner roles. Skill and current energy affect `WorkSpeedMultiplier`, while wages can be included in shift expenses.

## Stock and waste

`IngredientStock` tracks quantity, unit cost and a reorder threshold. `WasteTracker` records discarded units and their cost so waste can be included in shift profitability.

## Shift reporting

`ShiftTracker` records received, completed and cancelled orders, customers served, revenue, expenses and waste. `ShiftSummary` exposes net profit and completion rate.

## Tests

Restaurant tests are wrapped in `UNITY_INCLUDE_TESTS` and cover economy, seating selection, stock consumption and shift reporting.
