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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
