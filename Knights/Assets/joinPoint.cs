/*
*	TickLuck
*	All rights reserved
*/
using UnityEngine;

public class joinPoint : MonoBehaviour
{
    [HideInInspector] public bool is_busy;

    private void Start()
    {
        is_busy = false;
    }
}
