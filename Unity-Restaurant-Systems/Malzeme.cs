using UnityEngine;

public enum MalzemeTuru
{
    Ekmek,
    Kokorec,
    Domates,
    Biber
}

public class Malzeme : MonoBehaviour
{
    public MalzemeTuru tur;
    
    [HideInInspector]
    public bool tezgahtaMi = false; 
}