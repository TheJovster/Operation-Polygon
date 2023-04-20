using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragCount : MonoBehaviour
{
    private int fragCount = 0;

    public int GetFragCount() 
    {
        return fragCount;
    }

    public void IncrementFragCount() 
    {
        fragCount++;
    }
}
