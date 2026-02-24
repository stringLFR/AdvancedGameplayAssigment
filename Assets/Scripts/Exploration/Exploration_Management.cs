using ActionFlowStack;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Exploration_Management : MonoBehaviour
{
    [Header("UpgradePage")]

    [SerializeField]
    private GameObject upgradePage;

    [SerializeField]
    private DroneUpgradePage[] droneUpgradePages;

    [SerializeField]
    private Slider upgradeSlider;

    [SerializeField]
    private TextMeshProUGUI droneInfoPanel, sliderText;

    FlowAction_Management managementFlowAction;

    public static bool Activated = false;

    private const float maxUpgradeValue = 10;

    private const string defaultInfoPanelText = "This page handles upgrading of our drones!" +
        "<br>Just hover over any individual drone page to see their details here!" +
        "<br>It takes intel to plan out a training!" +
        "<br>While it takes every other resource to finalize a training!" +
        "<br>(The amount being equal to value)" +
        "<br>You can take a more riskfull approach and not spend the normal amount of resources!" +
        "<br>(The slider manipulates cost by modifier between 0 and 1!)" +
        "<br>But this can lead to failure, which causes sanity damage to the drone!";

    private void Awake()
    {
        managementFlowAction = ActionFlowStackHandler.CurrentFlowAction as FlowAction_Management;

        if (managementFlowAction == null) return;

        managementFlowAction.Init(this, FindFirstObjectByType<Exploration>());

        int index = -1;

        foreach (DroneUpgradePage upgradePage in droneUpgradePages)
        {
            index++;

            if (index > ActionFlowStackController.Instance.Team.PlayerMembers.droneUnits.Length - 1)
            {
                upgradePage.gameObject.SetActive(false);
                continue;
            }

            DroneUnit unit = ActionFlowStackController.Instance.Team.PlayerMembers.droneUnits[index];

            upgradePage.Init(this, unit);

            if (Activated == false)
            {
                unit.RerollUpgradeStats(maxUpgradeValue);
            }

            upgradePage.SetUpgradeText(
                unit.DroneName,
                $"{(int)unit.myUpgradePageInfo.currentUpgradeValue}",
                $"{unit.myUpgradePageInfo.full} + {(int)unit.myUpgradePageInfo.currentUpgradeValue}",
                $"{unit.myUpgradePageInfo.half} + {(int)unit.myUpgradePageInfo.currentUpgradeValue / 2}",
                $"{unit.myUpgradePageInfo.minus} - {(int)unit.myUpgradePageInfo.currentUpgradeValue / 4}");
        }

        sliderText.text = $"{upgradeSlider.value}% Success rate!<br>{(upgradeSlider.value / upgradeSlider.maxValue)}% Cost Reduction!";
        droneInfoPanel.text = defaultInfoPanelText;

        upgradePage.SetActive(false);

        Activated = true;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void CloseManagement()
    {
        if (managementFlowAction == null) return;

        managementFlowAction.GoExploring();
    }

    public void OpenUpgradePage()
    {
        upgradePage.SetActive(true);
    }

    public void OnHover(DroneUnit unit)
    {
        droneInfoPanel.text = $"Name: {unit.DroneName}" +
            $"<br>Level: {unit.Level}" +
            $"<br>" +
            $"<br>Current HP/Sanity Damages: " +
            $"<br>- HP:{unit.afterCombatStats.HPdamageTakenPercentile}" +
            $"<br>- Sanity:{unit.afterCombatStats.SanityDamageTakenPercentile}" +
            $"<br>" +
            $"<br>CoreStats:" +
            $"<br>- Str: {(int)unit.GetSTR}" +
            $"<br>- Dex: {(int)unit.GetDEX}" +
            $"<br>- Con: {(int)unit.GetCON}" +
            $"<br>- Int: {(int)unit.GetINT}" +
            $"<br>- Wis: {(int)unit.GetWIS}" +
            $"<br>- Cha: {(int)unit.GetCHA}";
    }

    public void OnHoverEnd()
    {
        droneInfoPanel.text = defaultInfoPanelText;
    }

    public void Upgrade(DroneUnit drone,DroneUpgradePage page)
    {
        float sliderValue = (upgradeSlider.value / upgradeSlider.maxValue);
        int cost = (int)(drone.myUpgradePageInfo.currentUpgradeValue * sliderValue);

        for (int i = 0; i < managementFlowAction.Expo.SupplyData.Length; i++)
        {
            if (managementFlowAction.Expo.SupplyData[i].Type == SupplyType.INTEL) continue;

            managementFlowAction.Expo.SupplyData[i].currentAmount -= cost;
            managementFlowAction.Expo.UpdateSlider(managementFlowAction.Expo.SupplyData[i]);
        }

        if (UnityEngine.Random.Range(0f, 1f) > sliderValue)
        {
            Reroll(drone, page);
            drone.afterCombatStats.SanityDamageTakenPercentile += (int)(drone.GetWIS - drone.GetWIS * sliderValue);
            return;
        }

        drone.TrainDroneUnit(
            drone.myUpgradePageInfo.currentUpgradeValue,
            drone.myUpgradePageInfo.full,
            drone.myUpgradePageInfo.half,
            drone.myUpgradePageInfo.minus);

        Reroll(drone, page);
    }

    public void Reroll(DroneUnit drone, DroneUpgradePage page)
    {
        for (int i = 0; i < managementFlowAction.Expo.SupplyData.Length; i++)
        {
            if (managementFlowAction.Expo.SupplyData[i].Type != SupplyType.INTEL) continue;

            managementFlowAction.Expo.SupplyData[i].currentAmount -= 20;
            managementFlowAction.Expo.UpdateSlider(managementFlowAction.Expo.SupplyData[i]);
        }

        drone.RerollUpgradeStats(maxUpgradeValue);

        page.SetUpgradeText(
                drone.DroneName,
                $"{(int)drone.myUpgradePageInfo.currentUpgradeValue}",
                $"{drone.myUpgradePageInfo.full} + {(int)drone.myUpgradePageInfo.currentUpgradeValue}",
                $"{drone.myUpgradePageInfo.half} + {(int)drone.myUpgradePageInfo.currentUpgradeValue / 2}",
                $"{drone.myUpgradePageInfo.minus} - {(int)drone.myUpgradePageInfo.currentUpgradeValue / 4}");

        OnHover(drone);
    }

    public void SliderTextUpdate()
    {
        sliderText.text = $"{upgradeSlider.value}% Success rate!<br>{(upgradeSlider.value / upgradeSlider.maxValue)}% Cost Reduction!";
    }
}
