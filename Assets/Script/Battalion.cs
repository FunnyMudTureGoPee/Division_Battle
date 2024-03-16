using System;
using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;

public class Battalion : MonoBehaviour
{
    public BattalionData BattalionData { get; set; }

    private void Awake()
    {
        if (BattalionData is null)
        {
            BattalionData =
                new BattalionData(gameObject, 111, BattalionData.BattalionTypes.Infantry, BattalionData.Dirs.Up);
        }
        
    }
}
