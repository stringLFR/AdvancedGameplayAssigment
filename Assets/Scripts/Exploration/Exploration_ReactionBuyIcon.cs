using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Exploration_ReactionBuyIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Exploration_Management management;

    [SerializeField]
    private ReactionSO ReactionSO;

    [SerializeField]
    private SupplyType[] costTypes;

    [SerializeField]
    private int costValue;

    const string owningText = "<color=red>You already have it!<color=white>";

    public void OnPointerEnter(PointerEventData eventData)
    {
        string info;

        if (management.Buyer.MyReactionNodesSO.Contains(ReactionSO) == true)
        {
            info = owningText;
            management.OnReactionIconHover(info);
            return;
        }

        info = "<color=yellow>Name:<color=white> " + ReactionSO.reactionStats.NodeName + "<br>" + "<br>" +
        "<color=yellow>Info:<color=white> " + ReactionSO.reactionStats.NodeInfo + "<br>" + "<br>" + "<color=yellow>Costs:<color=white> ";

        foreach (SupplyType t in costTypes)
        {
            info += $"{costValue} <color=purple>{t}<color=white>, ";
        }

        management.OnReactionIconHover(info);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        management.OnReactionIconHoverEnd();
    }

    public void OnClick()
    {
        if (management.Buyer.MyReactionNodesSO.Contains(ReactionSO) == true) return;

        for (int i = 0; i < management.ManagementAction.Expo.SupplyData.Length; i++)
        {
            if (costTypes.Contains(management.ManagementAction.Expo.SupplyData[i].Type) == false) continue;

            management.ManagementAction.Expo.SupplyData[i].currentAmount -= costValue;
            management.ManagementAction.Expo.UpdateSlider(management.ManagementAction.Expo.SupplyData[i]);
        }

        management.Buyer.MyReactionNodesSO.Add(ReactionSO);

        management.OnReactionIconHover(owningText);
    }
}
