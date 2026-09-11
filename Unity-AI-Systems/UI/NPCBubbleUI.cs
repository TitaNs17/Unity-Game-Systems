using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class NPCBubbleUI : MonoBehaviour
{
    [SerializeField] private NPCController npc;
    [SerializeField] private TextMeshProUGUI bubbleText;
    [SerializeField] private GameObject canvasObject;

    private readonly Dictionary<ProductType, int> productCounts = new Dictionary<ProductType, int>();
    private readonly StringBuilder builder = new StringBuilder(128);

    private void Awake()
    {
        if (npc == null)
            npc = GetComponentInParent<NPCController>();

        if (canvasObject == null)
            canvasObject = gameObject;
    }

    private void Update()
    {
        var order = npc != null ? npc.myOrder : null;
        var shouldShow = npc != null &&
                         npc.currentState == NPCController.NPCState.WaitingForOrder &&
                         order != null &&
                         !order.isCompleted &&
                         order.remainingTime > 0f;

        if (canvasObject != null && canvasObject.activeSelf != shouldShow)
            canvasObject.SetActive(shouldShow);

        if (!shouldShow || bubbleText == null)
            return;

        productCounts.Clear();
        if (order.items != null)
        {
            foreach (var item in order.items)
            {
                if (productCounts.TryGetValue(item, out var count))
                    productCounts[item] = count + 1;
                else
                    productCounts[item] = 1;
            }
        }

        builder.Clear();
        foreach (var product in productCounts)
        {
            if (product.Value > 1)
                builder.Append(product.Value).Append("x ");

            builder.Append(product.Key).AppendLine();
        }

        builder.Append("<color=red>")
               .Append(Mathf.CeilToInt(order.remainingTime))
               .Append(" sn</color>");

        bubbleText.text = builder.ToString();
    }
}
