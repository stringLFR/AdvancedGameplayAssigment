using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class CombatInfoPopUp : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameText;

    [SerializeField]
    private TextMeshProUGUI infoText;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void SetPopUp(string n, string d)
    {
        nameText.text = n;
        infoText.text = d;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void ClearPopUp()
    {
        nameText.text = "";
        infoText.text = "";
    }
}
