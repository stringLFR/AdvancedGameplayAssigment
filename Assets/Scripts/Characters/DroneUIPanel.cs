using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DroneUIPanel : MonoBehaviour
{
    private DroneUnitBody _unitBody;


    [SerializeField]
    private TextMeshProUGUI healthValueText, manaValueText, sanityValueText, droneName;
    [SerializeField]
    private Slider healthSlider, manaSlider, sanitySlider;

    private int maxHealth, maxMana, maxSanity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _unitBody = GetComponent<DroneUnitBody>();
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

    public void InitUIPanel(int hp, int mana, int sanity, string name)
    {
        maxHealth = hp;
        maxMana = mana;
        maxSanity = sanity;
        healthSlider.maxValue = maxHealth;
        manaSlider.maxValue = maxMana;
        sanitySlider.maxValue = maxSanity;
        droneName.text = name;
    }
}
