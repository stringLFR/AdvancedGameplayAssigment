using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverOverTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private PlayerHudPrototype hud;
    private int myIndex;
    private string myName;
    private string myDescription;

    public void OnPointerEnter(PointerEventData eventData)
    {
        hud.OnHover(myName, myDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hud.OnLeaving();
    }

    public void Init(PlayerHudPrototype h)
    {
        hud = h;
        hud.hoverTriggers.Add(this);
        myIndex = hud.hoverTriggers.Count - 1;
    }

    public void SetInfo(string name, string info)
    {
        myName = name;
        myDescription = info;
    }
}
