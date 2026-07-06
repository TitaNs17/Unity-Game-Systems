using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Hotbar Slotlarý (5 adet)")]
    public InventorySlot[] slots = new InventorySlot[5];

    private void Awake()
    {
        Instance = this;
    }

    public void AddItem(ItemInfo item)
    {
        
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item)
            {
                slots[i].amount++;
                slots[i].UpdateUI();
                return;
            }
        }

     
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].item = item;
                slots[i].amount = 1;
                slots[i].UpdateUI();
                return;
            }
        }

        Debug.Log("ENVANTER DOLU! (5 Slot)");
    }
    public GameObject RemoveSelectedItem(int selectedIndex)
    {
        if (selectedIndex < 0 || selectedIndex >= slots.Length)
            return null;

        InventorySlot slot = slots[selectedIndex];

        if (slot.item == null)
            return null;

        GameObject prefab = slot.item.prefab;

        slot.amount--;

        if (slot.amount <= 0)
        {
            slot.item = null;
            slot.amount = 0;
        }

        slot.UpdateUI();
        return prefab;
    }
}
