using System.Collections.Generic;
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

    public void OnHover(string name, string info)
    {
        infoPopUp.gameObject.SetActive(true);
        infoPopUp.SetPopUp(name, info);
    }

    public void OnLeaving()
    {
        infoPopUp.ClearPopUp();
        infoPopUp.gameObject.SetActive(false);
    }
}
