using UnityEngine;

public class AlmaBirakmaSistemi : MonoBehaviour
{
    [Header("Ayarlar")]
    public float almaMenzili = 3f;
    public Transform elPozisyonu;
    public Camera oyuncuKamerasi;
    public KeyCode almaTusu = KeyCode.E;
    public KeyCode birakmaTusu = KeyCode.F;

    [HideInInspector] public GameObject tutulanObje = null;
    [HideInInspector] public Rigidbody tutulanObjeRB = null;

    void Update()
    {
        if (tutulanObje != null)
        {
            if (Input.GetKeyDown(birakmaTusu))
                Birak();
        }
    }

    public void Al(GameObject alinacakObje)
    {
        if (tutulanObje == null)
        {
            tutulanObje = alinacakObje;
            tutulanObjeRB = tutulanObje.GetComponent<Rigidbody>();

            if (tutulanObjeRB != null)
                tutulanObjeRB.isKinematic = true;

            tutulanObje.transform.SetParent(elPozisyonu);

            PickupItem pickup = tutulanObje.GetComponent<PickupItem>();

            if (pickup != null)
            {
                tutulanObje.transform.localPosition = pickup.holdPositionOffset;
                tutulanObje.transform.localRotation = Quaternion.Euler(pickup.holdRotationOffset);
            }
            else
            {
                tutulanObje.transform.localPosition = Vector3.zero;
                tutulanObje.transform.localRotation = Quaternion.identity;
            }
        }
    }

    public void Birak()
    {
        if (tutulanObje == null) return;

        tutulanObje.transform.SetParent(null);

        if (tutulanObjeRB != null)
            tutulanObjeRB.isKinematic = false;

        tutulanObje = null;
        tutulanObjeRB = null;
    }
}