using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Exploration_MainActionBuyIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Exploration_Management management;

    [SerializeField]
    private MainActionSO MainActionSO;

    [SerializeField]
    private SupplyType[] costTypes;

    [SerializeField]
    private int costValue;

    const string owningText = "<color=red>You already have it!<color=white>";

    public void OnPointerEnter(PointerEventData eventData)
    {
        string info;

        if (management.Buyer.MyMainActionSOs.Contains(MainActionSO) == true)
        {
            info = owningText;
            management.OnMainIconHover(info);
            return;
        }

        info = "<color=green>Name:<color=white> " + MainActionSO.mainActionStats.MainActionName + "<br>" + "<br>" +
            "<color=green>Info:<color=white> " + MainActionSO.mainActionStats.MainActionDescription + "<br>" + "<br>" + "<color=green>Costs:<color=white> ";

        foreach(SupplyType t in costTypes)
        {
            info += $"{costValue} <color=purple>{t}<color=white>, ";
        }

        management.OnMainIconHover(info);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        management.OnMainIconHoverEnd();
    }

    public void OnClick()
    {
        if (management.Buyer.MyMainActionSOs.Contains(MainActionSO) == true) return; 

        for (int i = 0; i < management.ManagementAction.Expo.SupplyData.Length; i++)
        {
            if (costTypes.Contains(management.ManagementAction.Expo.SupplyData[i].Type) == false) continue;

            management.ManagementAction.Expo.SupplyData[i].currentAmount -= costValue;
            management.ManagementAction.Expo.UpdateSlider(management.ManagementAction.Expo.SupplyData[i]);
        }

        management.Buyer.MyMainActionSOs.Add(MainActionSO);

        management.OnMainIconHover(owningText);
    }
}
