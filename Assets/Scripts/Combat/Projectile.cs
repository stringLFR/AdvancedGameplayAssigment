using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Projectile : MonoBehaviour
{

    [SerializeField]
    private float baseDamage = 1f;
    [SerializeField]
    Vector3 pa_Tangent, pb_Tangent, pa_Tangent_Rand, pb_Tangent_Rand;
    [SerializeField, Range(0,1)]
    private float manaDrainPerSec = 1f;
    [SerializeField, Range(1, 2)]
    private float speed = 1f;

    private ICombatObject controller;

    float test = 0f;

    private Vector3 p0, p1, p2, p3;
    private float startingMana;
    private float currentMana;
    public void InitProjectile(ICombatObject c)
    {
        controller = c;
    }
    public void moveProjectile() //TODO FINALIZE HOW I WANT TO SET SPEED OF PROJECTILE!!!
    {
        test += Time.deltaTime * 0.1f;

        //currentMana *= manaDrainPerSec;
        //currentMana *= speed;
        float i = Mathf.InverseLerp(0,1, test);
        transform.position = GetCubicBezierPosition(i);
        
    }
    public void Fire(float mana, Vector3 pa, Vector3 pb)
    {
        p0 = pa;
        p1 = pa + (pa_Tangent + RandomTangent(pa_Tangent_Rand));
        p2 = pb - (pb_Tangent + RandomTangent(pb_Tangent_Rand)); 
        p3 = pb;
        startingMana = mana;
        currentMana = startingMana;

        test = 0f;
    }

    private Vector3 RandomTangent(Vector3 value) => new Vector3(Random.Range(-value.x, value.x), Random.Range(-value.y, value.y), Random.Range(-value.z, value.z));

    private Vector3 GetCubicBezierPosition(float f)
    {
        float fOneMinusT = 1.0f - f;
        return p0 * fOneMinusT * fOneMinusT * fOneMinusT +
               p1 * 3 * fOneMinusT * fOneMinusT * f +
               p2 * 3 * fOneMinusT * f * f +
               p3 * f * f * f;
    }

    private void OnTriggerEnter(Collider other)
    {

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
