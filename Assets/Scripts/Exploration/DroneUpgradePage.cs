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

        management.OnHover(myUnit);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (management == null) return;

        management.OnHoverEnd();
    }

    public void Init(Exploration_Management m, DroneUnit unit)
    {
        management = m;
        myUnit = unit;
    }

    public void SetUpgradeText(string name, string value, string Full, string Half,string Minus)
    {
        droneName.text = "Name: " + name;
        updgradeValue.text = "Value: " + value;
        full.text = Full;
        half.text = Half;
        minus.text = Minus;
    }

    public void OnUpgradeClick()
    {
        management.Upgrade(myUnit, this);
    }

    public void OnRerollClick()
    {
        management.Reroll(myUnit, this);
    }

    public void OnHealClick()
    {
        management.HealDrone(myUnit, this);
    }

    public void OnForceLevel()
    {
        management.ForceLevelUpDrone(myUnit, this);
    }
}
