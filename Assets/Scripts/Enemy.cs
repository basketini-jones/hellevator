using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [System.NonSerialized] public bool touched = false;

    // Update is called once per frame
    void Update()
    {
        if (touched)
        {
            Destroy(this.gameObject);
        }
    }
}
