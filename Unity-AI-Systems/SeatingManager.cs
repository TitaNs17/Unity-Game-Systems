using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Listeyi karýþtýrmak için lazým

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
        // Masalarý geçici bir listeye al ve rastgele karýþtýr
        List<Table> shuffledTables = tables.ToList();
        for (int i = 0; i < shuffledTables.Count; i++)
        {
            Table temp = shuffledTables[i];
            int randomIndex = Random.Range(i, shuffledTables.Count);
            shuffledTables[i] = shuffledTables[randomIndex];
            shuffledTables[randomIndex] = temp;
        }

        // Karýþtýrýlmýþ listede boþ yer ara
        foreach (Table table in shuffledTables)
        {
            SeatPoint freeSeat = table.OccupyFirstFreeSeat();
            if (freeSeat != null) return freeSeat;
        }
        return null;
    }
}

