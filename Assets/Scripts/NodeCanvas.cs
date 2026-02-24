using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NodeCanvas : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private TextMeshProUGUI nodeName, nodeType;

    [SerializeField]
    private Slider food, intel, mana, medicine, metalics;

    [SerializeField]
    private Image background;

    [SerializeField]
    private Button button;

    [SerializeField]
    private GameObject hoverPanel;

    public Button Button => button;

    public void UpdateSlider(SupplyData data)
    {
        switch (data.Type)
        {
            case SupplyType.FOOD:
                food.value = data.currentAmount;
                break;
            case SupplyType.MANA_STORAGE:
                mana.value = data.currentAmount;
                break;
            case SupplyType.INTEL:
                intel.value = data.currentAmount;
                break;
            case SupplyType.MEDICINE:
                medicine.value = data.currentAmount;
                break;
            case SupplyType.METALLICS:
                metalics.value = data.currentAmount;
                break;
        }
    }

    public void SetUpSliders(SupplyData data)
    {
        switch (data.Type)
        {
            case SupplyType.FOOD:
                food.maxValue = data.MaxAmount;
                break;
            case SupplyType.MANA_STORAGE:
                mana.maxValue = data.MaxAmount;
                break;
            case SupplyType.INTEL:
                intel.maxValue = data.MaxAmount;
                break;
            case SupplyType.MEDICINE:
                medicine.maxValue = data.MaxAmount;
                break;
            case SupplyType.METALLICS:
                metalics.maxValue = data.MaxAmount;
                break;
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
