using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DroneUpgradePage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Exploration_Management management;
    private DroneUnit myUnit;

    [SerializeField]
    private TextMeshProUGUI droneName, updgradeValue, full, half, minus;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (management == null) return;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (management == null) return;
    }

    public void Init(Exploration_Management m, DroneUnit unit)
    {
        management = m;
        myUnit = unit;
    }
}
