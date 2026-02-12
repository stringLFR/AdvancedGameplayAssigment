using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHudPrototype : MonoBehaviour
{
    public List<Button> buttons;
    public List<TextMeshProUGUI> texts;
    public List<UIHoverOverTrigger> hoverTriggers = new List<UIHoverOverTrigger>();
    [SerializeField]
    private CombatInfoPopUp infoPopUp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        infoPopUp.gameObject.SetActive(false);
        foreach (var button in buttons)
        {
            button.GetComponent<UIHoverOverTrigger>().Init(this);
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void OnHover(string name, string info)
    {
        infoPopUp.gameObject.SetActive(true);
        infoPopUp.SetPopUp(name, info);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void OnLeaving()
    {
        infoPopUp.ClearPopUp();
        infoPopUp.gameObject.SetActive(false);
    }
}
