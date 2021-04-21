using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchTest : MonoBehaviour
{
    public void OnClick()
    {
        Debug.Log("Onlick "+ gameObject.name + " "+Time.realtimeSinceStartup);
    }
}
