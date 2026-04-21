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

    public void OnPointerEnter(PointerEventData eventData)
    {
        management.OnMainIconHover("");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        management.OnMainIconHoverEnd();
    }

    public void OnClick()
    {

    }
}
