using TMPro;
using UnityEngine;

public class CombatInfoPopUp : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameText;

    [SerializeField]
    private TextMeshProUGUI infoText;

    public void SetPopUp(string n, string d)
    {
        nameText.text = n;
        infoText.text = d;
    }

    public void ClearPopUp()
    {
        nameText.text = "";
        infoText.text = "";
    }
}
