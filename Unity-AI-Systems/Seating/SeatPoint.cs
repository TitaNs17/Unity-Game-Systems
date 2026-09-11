using UnityEngine;

[DisallowMultipleComponent]
public class SeatPoint : MonoBehaviour
{
    [SerializeField] public bool isOccupied;

    public GameObject Occupant { get; private set; }
    public bool IsAvailable => !isOccupied;

    public bool TryReserve(GameObject occupant)
    {
        if (isOccupied) return false;

        isOccupied = true;
        Occupant = occupant;
        return true;
    }

    public void Release(GameObject occupant = null)
    {
        if (occupant != null && Occupant != null && Occupant != occupant)
            return;

        isOccupied = false;
        Occupant = null;
    }
}
