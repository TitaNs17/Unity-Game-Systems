using UnityEngine;
using System.Collections.Generic;
using System.Linq; 

public class SeatingManager : MonoBehaviour
{
    public static SeatingManager Instance;
    public Table[] tables;

    private void Awake()
    {
        Instance = this;
    }

    public SeatPoint GetAndReserveFreeSeat()
    {
        
        List<Table> shuffledTables = tables.ToList();
        for (int i = 0; i < shuffledTables.Count; i++)
        {
            Table temp = shuffledTables[i];
            int randomIndex = Random.Range(i, shuffledTables.Count);
            shuffledTables[i] = shuffledTables[randomIndex];
            shuffledTables[randomIndex] = temp;
        }

        
        foreach (Table table in shuffledTables)
        {
            SeatPoint freeSeat = table.OccupyFirstFreeSeat();
            if (freeSeat != null) return freeSeat;
        }
        return null;
    }
}

