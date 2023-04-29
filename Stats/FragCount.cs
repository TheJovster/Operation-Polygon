using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragCount : MonoBehaviour
{
    public static FragCount Instance { get; private set; }
    public int FragCountNumber { get; private set;}

    public int GetFragCount() 
    {
        return FragCountNumber;
    }

    public void IncrementFragCount() 
    {
        FragCountNumber++;
    }
}
