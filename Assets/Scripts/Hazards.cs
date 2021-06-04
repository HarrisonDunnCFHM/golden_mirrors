using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazards : MonoBehaviour
{
    [SerializeField] bool isSpikes = true;

    //ref cache
    TreasureHunter player;

    private void Start()
    {
        player = FindObjectOfType<TreasureHunter>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(isSpikes)
        {
            player.LevelFail();
        }
    }
}
