using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersection : MonoBehaviour
{
    //cached refs
    TreasureHunter treasureHunter;
    
    // Start is called before the first frame update
    void Start()
    {
        treasureHunter = FindObjectOfType<TreasureHunter>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.transform.position = transform.position;
        treasureHunter.CheckForNewTargets();
    }
}
