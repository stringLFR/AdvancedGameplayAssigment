using actions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DroneUIPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private DroneUnitBody _unitBody;


    [SerializeField]
    private TextMeshProUGUI healthValueText, manaValueText, sanityValueText, droneName, hoverText;
    [SerializeField]
    private Slider healthSlider, manaSlider, sanitySlider;

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private GameObject hoverPanel;

    private int maxHealth, maxMana, maxSanity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _unitBody = GetComponent<DroneUnitBody>();
        canvas.worldCamera = Camera.main;


    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.position - transform.position, Vector3.up);
    }

    public void SetHealthSlider(int value)
    {
        healthSlider.value = value;
        healthValueText.text = $"{healthSlider.value} / {maxHealth}";
    }
    public void SetManaSlider(int value)
    {
        manaSlider.value = value;
        manaValueText.text = $"{manaSlider.value} / {maxMana}";
    }
    public void SetSanitySlider(int value)
    {
        sanitySlider.value = value;
        sanityValueText.text = $"{sanitySlider.value} / {maxSanity}";
    }

    public void InitUIPanel(DroneUnitBody body)
    {
        maxHealth = body.MyHP;
        maxMana = body.MyMana;
        maxSanity = body.MySanity;
        healthSlider.maxValue = maxHealth;
        manaSlider.maxValue = maxMana;
        sanitySlider.maxValue = maxSanity;
        droneName.text = body.DroneUnit.DroneName;

        hoverText.text = $"Level: {body.DroneUnit.Level}";
        hoverText.text += $"<br>";
        hoverText.text += $"<br>Core stats: <br>-STR: {body.DroneUnit.GetSTR} <br>-DEX: {body.DroneUnit.GetDEX} <br>-CON: {body.DroneUnit.GetCON} <br>-INT: {body.DroneUnit.GetINT} <br>-WIS: {body.DroneUnit.GetWIS} <br>-CHA: {body.DroneUnit.GetCHA}";
        hoverText.text += $"<br>";
        hoverText.text += $"<br>MainActions:";
        foreach (MainActionStats main in body.DroneUnit.MyMainActions)
        {
            hoverText.text += $"<br>-{main.MainActionName}";
            hoverText.text += $"<br>--{main.MainActionDescription}";
        }
        hoverText.text += $"<br>";
        hoverText.text += $"<br>Reactions:";
        foreach (ActionNodeStats re in body.DroneUnit.MyReactionNodes)
        {
            hoverText.text += $"<br>-{re.NodeName}";
            hoverText.text += $"<br>--{re.NodeInfo}";
        }
        hoverPanel.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverPanel.SetActive(false);
    }
}
