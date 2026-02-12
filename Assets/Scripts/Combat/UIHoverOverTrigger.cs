using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverOverTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private PlayerHudPrototype hud;
    private int myIndex;
    private string myName;
    private string myDescription;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void OnPointerEnter(PointerEventData eventData)
    {
        hud.OnHover(myName, myDescription);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void OnPointerExit(PointerEventData eventData)
    {
        hud.OnLeaving();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void Init(PlayerHudPrototype h)
    {
        hud = h;
        hud.hoverTriggers.Add(this);
        myIndex = hud.hoverTriggers.Count - 1;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void SetInfo(string name, string info)
    {
        myName = name;
        myDescription = info;
    }
}
