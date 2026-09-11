using UnityGameSystems.AI;
using UnityGameSystems.GameFlow;
using UnityGameSystems.Restaurant.Seating;
using UnityGameSystems.Restaurant.Shift;
using UnityGameSystems.Restaurant.Staff;
using UnityGameSystems.Restaurant.Stock;
using UnityGameSystems.Stats;

var tests = new (string Name, Action Run)[]
{
    ("AI state machine transitions", TestAIStateMachine),
    ("Countdown timer completion", TestCountdownTimer),
    ("Runtime stat modifiers", TestRuntimeStat),
    ("Restaurant seating selection", TestSeating),
    ("Restaurant reservation rules", TestReservation),
    ("Ingredient stock consumption", TestStock),
    ("Waste tracking", TestWaste),
    ("Shift profit calculation", TestShift),
    ("Staff energy and training", TestStaff)
};

var failures = new List<string>();
foreach (var test in tests)
{
    try
    {
        test.Run();
        Console.WriteLine($"PASS  {test.Name}");
    }
    catch (Exception ex)
    {
        failures.Add($"{test.Name}: {ex.Message}");
        Console.WriteLine($"FAIL  {test.Name}: {ex.Message}");
    }
}

Console.WriteLine();
Console.WriteLine($"{tests.Length - failures.Count}/{tests.Length} checks passed.");

if (failures.Count > 0)
{
    Console.WriteLine("Failures:");
    foreach (var failure in failures)
        Console.WriteLine($"- {failure}");
    Environment.Exit(1);
}

static void TestAIStateMachine()
{
    var machine = new AIStateMachine();
    var idle = new IdleState();
    var chase = new ChaseState();
    machine.Register(idle);
    machine.Register(chase);

    Assert(machine.Change<IdleState>(), "Idle state should activate");
    machine.Tick(0.1f);
    Assert(idle.EnterCount == 1 && idle.TickCount == 1, "Idle callbacks were not executed");

    Assert(machine.Change<ChaseState>(), "Chase state should activate");
    Assert(idle.ExitCount == 1 && chase.EnterCount == 1, "Transition callbacks were not executed");

    machine.Stop();
    Assert(chase.ExitCount == 1 && machine.Current == null, "Stop should exit current state");
}

static void TestCountdownTimer()
{
    var timer = new CountdownTimer(1f);
    var finished = 0;
    timer.Finished += () => finished++;
    timer.Start();
    timer.Tick(0.4f);
    Assert(timer.IsRunning, "Timer stopped too early");
    AssertNear(0.6f, timer.Remaining, 0.0001f, "Unexpected timer remaining value");
    timer.Tick(0.6f);
    Assert(timer.IsFinished, "Timer did not finish");
    Assert(finished == 1, "Finished event must fire exactly once");
}

static void TestRuntimeStat()
{
    var source = new object();
    var stat = new RuntimeStat(100f);
    stat.AddModifier(new StatModifier(20f, ModifierMode.Flat, source));
    stat.AddModifier(new StatModifier(0.25f, ModifierMode.Percent, source));
    AssertNear(150f, stat.Value, 0.0001f, "Stat calculation is incorrect");
    Assert(stat.RemoveModifiersFrom(source) == 2, "Expected both modifiers to be removed");
    AssertNear(100f, stat.Value, 0.0001f, "Stat should return to base value");
}

static void TestSeating()
{
    var service = new SeatingService();
    var two = new RestaurantTable(2);
    var four = new RestaurantTable(4);
    var six = new RestaurantTable(6);
    service.Register(six);
    service.Register(four);
    service.Register(two);

    var best = service.FindBestTable(3);
    Assert(ReferenceEquals(best, four), "Service should choose the smallest table that fits");
    Assert(service.TrySeatParty(3, out var seated) && ReferenceEquals(seated, four), "Party should be seated at selected table");
    Assert(four.OccupiedSeats == 3, "Occupied seat count is incorrect");
}

static void TestReservation()
{
    var table = new RestaurantTable(4);
    Assert(table.TryReserve("r-1"), "Reservation should succeed on empty table");
    Assert(!table.TrySeat(2), "Walk-in party must not use a reserved table");
    Assert(table.TrySeat(2, "r-1"), "Matching reservation should be seated");
    Assert(!table.IsReserved && table.OccupiedSeats == 2, "Reservation should clear after seating");
}

static void TestStock()
{
    var stock = new IngredientStock("tomato", 5, 2, 3m);
    Assert(!stock.TryConsume(6), "Over-consumption must fail");
    Assert(stock.Quantity == 5, "Failed consume must not change quantity");
    Assert(stock.TryConsume(3), "Valid consume should succeed");
    Assert(stock.Quantity == 2 && stock.NeedsReorder, "Reorder state is incorrect");
    stock.Add(5);
    Assert(stock.Quantity == 7 && !stock.NeedsReorder, "Restock behavior is incorrect");
}

static void TestWaste()
{
    var waste = new WasteTracker();
    waste.Record("tomato", 2, 3m);
    waste.Record("tomato", 1, 3m);
    Assert(waste.GetWastedUnits("tomato") == 3, "Waste unit count is incorrect");
    Assert(waste.WasteCost == 9m, "Waste cost is incorrect");
    waste.Reset();
    Assert(waste.GetWastedUnits("tomato") == 0 && waste.WasteCost == 0m, "Waste reset failed");
}

static void TestShift()
{
    var tracker = new ShiftTracker();
    tracker.RecordOrderReceived();
    tracker.RecordOrderReceived();
    tracker.RecordOrderCompleted(120m, 2);
    tracker.RecordOrderCancelled();
    tracker.RecordExpense(30m);
    tracker.RecordWaste(10m);

    Assert(tracker.Summary.NetProfit == 80m, "Net profit is incorrect");
    AssertNear(0.5f, tracker.Summary.CompletionRate, 0.0001f, "Completion rate is incorrect");
    Assert(tracker.Summary.CustomersServed == 2, "Customer count is incorrect");
}

static void TestStaff()
{
    var member = new StaffMember("Cook", StaffRole.Cook, 1f, 100m);
    var initialSpeed = member.WorkSpeedMultiplier;
    member.Work(50f);
    Assert(member.Energy == 50f, "Work should reduce energy");
    Assert(member.WorkSpeedMultiplier < initialSpeed, "Lower energy should reduce work speed");
    member.Rest(100f);
    Assert(member.Energy == 100f, "Energy should clamp to 100");
    member.Train(0.5f);
    AssertNear(1.5f, member.Skill, 0.0001f, "Training should increase skill");
}

static void Assert(bool condition, string message)
{
    if (!condition)
        throw new InvalidOperationException(message);
}

static void AssertNear(float expected, float actual, float tolerance, string message)
{
    if (Math.Abs(expected - actual) > tolerance)
        throw new InvalidOperationException($"{message}. Expected {expected}, got {actual}");
}

sealed class IdleState : IAIState
{
    public int EnterCount { get; private set; }
    public int TickCount { get; private set; }
    public int ExitCount { get; private set; }
    public void Enter() => EnterCount++;
    public void Tick(float deltaTime) => TickCount++;
    public void Exit() => ExitCount++;
}

sealed class ChaseState : IAIState
{
    public int EnterCount { get; private set; }
    public int ExitCount { get; private set; }
    public void Enter() => EnterCount++;
    public void Tick(float deltaTime) { }
    public void Exit() => ExitCount++;
}
