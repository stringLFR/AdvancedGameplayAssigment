using UniGameMaths;
using UnityEngine;

public class SummonObject : MonoBehaviour
{
    private ICombatObject controller;
    private float startingMana;
    private float progress;

    [SerializeField]
    private float baseDamage = 1f, manaDrainPerSec = 1f;

    [SerializeField, Range(0, 1)]
    private float speed = 1f;

    [SerializeField]
    private float lifetime;

    [SerializeField]
    private bool isMagical = false;
    public bool isMagic { get { return isMagical; } }

    public void InitSummonedObject(ICombatObject c)
    {
        controller = c;
    }

    public void Summon(float mana, Vector3 pos, Vector3 lookPos)
    {
        transform.position = pos;
        transform.rotation = Quaternion.LookRotation(lookPos, Vector3.up);
        startingMana = mana;
        progress = 0f;
    }

    public bool SustainSummon(Vector3 pos)
    {
        startingMana -= Time.deltaTime * manaDrainPerSec;
        progress += Time.deltaTime * speed;

        if (startingMana < 0.1f)
        {
            CombatListener.AddLineToCombatText($"Summon ran out of mana!");
            return false;
        }

        if (progress > lifetime) return false;

        return true;

    }
}
