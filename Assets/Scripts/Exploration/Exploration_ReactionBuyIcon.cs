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

    public void OnPointerEnter(PointerEventData eventData)
    {
        management.OnReactionIconHover("");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        management.OnReactionIconHoverEnd();
    }

    public void OnClick()
    {

    }
}
