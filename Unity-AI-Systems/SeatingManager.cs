using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SeatingManager : MonoBehaviour
{
    public static SeatingManager Instance { get; private set; }

    [SerializeField] private SeatPoint[] seats = System.Array.Empty<SeatPoint>();
    [SerializeField] private bool autoFindSeats = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (autoFindSeats && (seats == null || seats.Length == 0))
            seats = GetComponentsInChildren<SeatPoint>(true);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public SeatPoint GetAndReserveFreeSeat(GameObject occupant = null)
    {
        if (seats == null || seats.Length == 0)
            return null;

        var candidates = new List<SeatPoint>(seats.Length);
        foreach (var seat in seats)
        {
            if (seat != null && seat.IsAvailable)
                candidates.Add(seat);
        }

        while (candidates.Count > 0)
        {
            var index = Random.Range(0, candidates.Count);
            var seat = candidates[index];
            candidates.RemoveAt(index);

            if (seat.TryReserve(occupant))
                return seat;
        }

        return null;
    }

    public void ReleaseSeat(SeatPoint seat, GameObject occupant = null)
    {
        if (seat != null)
            seat.Release(occupant);
    }

    public int AvailableSeatCount()
    {
        var count = 0;
        if (seats == null) return count;

        foreach (var seat in seats)
            if (seat != null && seat.IsAvailable) count++;

        return count;
    }
}
