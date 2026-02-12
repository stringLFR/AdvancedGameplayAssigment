using System.Runtime.CompilerServices;
using UnityEngine;

public class Exploration_StartScreen : MonoBehaviour
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void CloseScreen()
    {
        Destroy(gameObject);
    }
}
