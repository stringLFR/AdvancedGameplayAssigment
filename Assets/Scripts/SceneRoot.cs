using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SceneRoot : MonoBehaviour
{
    public static List<SceneRoot> roots = new List<SceneRoot>();

    private void Awake() => roots.Add(this);

    public static void SetRoot(int targetIndex)
    {
        int index = -1;
        foreach (SceneRoot root in roots)
        {
            index++;

            if (index != targetIndex) root.gameObject.SetActive(false);
            else root.gameObject.SetActive(true);
        }
    }
}
