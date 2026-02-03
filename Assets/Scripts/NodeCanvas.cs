using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NodeCanvas : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nodeName, nodeType;

    [SerializeField]
    private Slider food, intel, mana, medicine, metalics;

    [SerializeField]
    private Image background;

    [SerializeField]
    private Button button;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
