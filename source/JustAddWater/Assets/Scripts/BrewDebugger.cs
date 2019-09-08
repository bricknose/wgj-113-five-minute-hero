using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrewDebugger : MonoBehaviour
{
    public Brew cauldron;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(cauldron.ToString());
        }
    }
}
