using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [System.NonSerialized] public bool touched = false;
    public bool vulnerable = false;

    // Update is called once per frame
    private void Update()
    {
        if (touched)
        {
            Destroy(this.gameObject);
        }
    }

    public void BecomeVulnerable()
    {
        vulnerable = true;
    }
}
