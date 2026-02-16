using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SceneRoot : MonoBehaviour
{
    public static List<SceneRoot> roots = new List<SceneRoot>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    private void Awake() => roots.Add(this);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public static void SetRoot(int targetIndex)
    {
        int index = -1;
        foreach (SceneRoot root in roots)
        {
            index++;

            root.gameObject.SetActive(index == targetIndex);
        }
    }

    private void OnDestroy()
    {
        roots.Remove(this);
    }
}
