using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class NPCBubbleUI : MonoBehaviour
{
    public NPCController npc;
    public TextMeshProUGUI bubbleText;
    public GameObject canvasObject;

    void Update()
    {
        if (npc.currentState == NPCController.NPCState.Eating &&
            npc.myOrder != null &&
            !npc.myOrder.isCompleted &&
            npc.myOrder.remainingTime > 0)
        {
            canvasObject.SetActive(true);

            // Ürünleri say ve grupla
            Dictionary<ProductType, int> urunSayilari = new Dictionary<ProductType, int>();

            foreach (var item in npc.myOrder.items)
            {
                if (urunSayilari.ContainsKey(item))
                    urunSayilari[item]++;
                else
                    urunSayilari[item] = 1;
            }

            // Yazýyý oluþtur
            string siparisYazisi = "";
            foreach (var urun in urunSayilari)
            {
                if (urun.Value > 1)
                {
                    siparisYazisi += urun.Value + "x " + urun.Key.ToString() + "\n";
                }
                else
                {
                    siparisYazisi += urun.Key.ToString() + "\n";
                }
            }

            siparisYazisi += "<color=red>" + (int)npc.myOrder.remainingTime + " sn</color>";
            bubbleText.text = siparisYazisi;
        }
        else
        {
            canvasObject.SetActive(false);
        }
    }
}