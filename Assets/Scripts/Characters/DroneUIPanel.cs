using actions;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DroneUIPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
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
        canvas.worldCamera = Camera.main;


    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.position - transform.position, Vector3.up);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void SetHealthSlider(int value)
    {
        healthSlider.value = value;
        healthValueText.text = $"{healthSlider.value} / {maxHealth}";
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void SetManaSlider(int value)
    {
        manaSlider.value = value;
        manaValueText.text = $"{manaSlider.value} / {maxMana}";
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void SetSanitySlider(int value)
    {
        sanitySlider.value = value;
        sanityValueText.text = $"{sanitySlider.value} / {maxSanity}";
    }

    public void InitUIPanel(DroneUnitBody body)
    {
        maxHealth = body.MyMaxHP;
        maxMana = body.MyMana;
        maxSanity = body.MySanity;
        healthSlider.maxValue = maxHealth;
        manaSlider.maxValue = maxMana;
        sanitySlider.maxValue = maxSanity;
        droneName.text = body.DroneUnit.DroneName;

        hoverText.text = $"Level: {body.DroneUnit.Level}";
        hoverText.text += $"<br>";
        hoverText.text += $"<br>Core stats: " +
            $"<br>-<color=green>STR: <color=white>{body.DroneUnit.GetSTR} " +
            $"<br>-<color=green>DEX: <color=white>{body.DroneUnit.GetDEX} " +
            $"<br>-<color=green>CON: <color=white>{body.DroneUnit.GetCON} " +
            $"<br>-<color=green>INT: <color=white>{body.DroneUnit.GetINT} " +
            $"<br>-<color=green>WIS: <color=white>{body.DroneUnit.GetWIS} " +
            $"<br>-<color=green>CHA: <color=white>{body.DroneUnit.GetCHA}";
        hoverText.text += $"<br>";
        hoverText.text += $"<br> Sub stats: " +
            $"<br>-<color=yellow>Max HP <color=white>{body.MyHP} " +
            $"<br>-<color=yellow>Max Mana <color=white>{body.MyMana} " +
            $"<br>-<color=yellow>Max Sanity <color=white>{body.MySanity} " +
            $"<br>-<color=yellow>Toughness <color=white>{body.MyToughness} " +
            $"<br>-<color=yellow>Ranged physical hitRate <color=white>{body.MyRanged_P_HitRate} " +
            $"<br>-<color=yellow>Ranged Magical hitRate <color=white>{body.MyRanged_M_HitRate} " +
            $"<br>-<color=yellow>Melee physical hitRate <color=white>{body.MyMelee_P_HitRate} " +
            $"<br>-<color=yellow>Melee magical hitRate <color=white>{body.MyMelee_M_HitRate} " +
            $"<br>-<color=yellow>Speed <color=white>{body.MySpeed}";
        hoverText.text += $"<br>";
        hoverText.text += $"<br>MainActions:";
        foreach (MainActionStats main in body.DroneUnit.MyMainActions)
        {
            hoverText.text += $"<br>-<color=green>{main.MainActionName}<color=white>";
            hoverText.text += $"<br>--{main.MainActionDescription}";
        }
        hoverText.text += $"<br>";
        hoverText.text += $"<br>Reactions:";
        foreach (ActionNodeStats re in body.DroneUnit.MyReactionNodes)
        {
            hoverText.text += $"<br>-<color=yellow>{re.NodeName}<color=white>";
            hoverText.text += $"<br>--{re.NodeInfo}";
        }
        hoverPanel.SetActive(false);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverPanel.SetActive(true);

        if (CombatListener.Combat == null) return;

        CombatListener.Combat.ActivateSynergyPanel(_unitBody);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void OnPointerExit(PointerEventData eventData)
    {
        hoverPanel.SetActive(false);

        if (CombatListener.Combat == null) return;

        CombatListener.Combat.DeactivateSynergyPanel();
    }
}
