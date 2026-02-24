using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Exploration_Node_Tower : Exploration_Node
{
    [SerializeField]
    private float fireRate = 1f, targetRadius = 50f, HitRadius = 5f, hitRate = 0.75f, slowDownEffect = 2f, fallOffModifier = 0.01f;

    [SerializeField]
    private int damage = 2, targets = 3, resoursCostPerAttack = 1;

    private List<Exploration_Hostile> targeting;
    private Vector3[] targetPoints;

    float timeUntilNextAttack = 0f;

    public void TowerTick(List<Exploration_Hostile> hostiles, float time, Exploration expo)
    {
        timeUntilNextAttack += time;

        if (timeUntilNextAttack < fireRate) return;

        timeUntilNextAttack = 0;

        foreach(SupplyData data in supplies)
        {
            if ((data.currentAmount - resoursCostPerAttack) < 0)
            {
                return;
            } 
        }

        targeting.Clear();

        foreach (Exploration_Hostile h in hostiles)
        {
            if (Vector3.Distance(h.body.transform.position,transform.position) <= targetRadius)
            {
                targeting.Add(h);
            }
        }

        for (int i = 0; i < targets; i++)
        {
            if (i > targeting.Count - 1) break; 

            targetPoints[i] = targeting[i].GetPosition();
        }

        foreach (Exploration_Hostile h in targeting)
        {
            Debug.DrawLine(transform.position, h.body.transform.position, Color.red, fireRate,false);

            for (int i = 0; i < targetPoints.Length; i++)
            {
                if (Vector3.Distance(h.body.transform.position, targetPoints[i]) <= HitRadius)
                {
                    if (UnityEngine.Random.Range(0, 1) > hitRate) continue;

                    Debug.DrawLine(targetPoints[i], h.body.transform.position, Color.red, fireRate, false);

                    float dist = Vector3.Distance(h.body.transform.position, transform.position);

                    int d = (int)Mathf.Clamp(damage - dist * fallOffModifier, 0, damage);

                    float s = Mathf.Clamp(slowDownEffect + dist * fallOffModifier, slowDownEffect, h.BaseSpeed);

                    h.ReciveTowerEffects(d, s, expo);
                }
            }
        }

        for(int i = 0; i < supplies.Length; i++)
        {
            supplies[i].currentAmount = Mathf.Clamp(supplies[i].currentAmount - resoursCostPerAttack, 0, supplies[i].MaxAmount);
            canvas.UpdateSlider(supplies[i]);
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public override void NodeInteract(Exploration_Caravan caravan, SupplyData d)
    {
        base.NodeInteract(caravan, d);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public override void NodeInteract()
    {
        base.NodeInteract();
    }

    public override void OnFeedOrEmpty(bool isTaking, Exploration_Caravan c, Exploration e)
    {
        if (isTaking == true)
        {

            return;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AreaSetUp();

        targeting = new List<Exploration_Hostile> { };
        targetPoints = new Vector3[targets];
    }
}
