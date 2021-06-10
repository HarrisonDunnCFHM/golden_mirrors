using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MirrorManager : MonoBehaviour
{
    //config params
    [SerializeField] int maxMirrors = 3;
    int currentMirrors;
    [SerializeField] Mirror mirror;
    [SerializeField] Text mirrorCount;
    bool isStarted;
    [SerializeField] List<AudioClip> removeMirror;


    //cached refs
    TreasureHunter treasureHunter;
    AudioManager audioManager;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        treasureHunter = FindObjectOfType<TreasureHunter>();
        currentMirrors = maxMirrors;
        mirrorCount.text = "Mirrors: " + currentMirrors.ToString();
    }

    public void DisableMirrorAdd()
    {
        isStarted = true;
    }

    private void OnMouseDown()
    {
        if (!isStarted)
        {
            if (currentMirrors > 0)
            {
                PlaceMirror(GetSquareClicked());
                currentMirrors--;
                mirrorCount.text = "Mirrors: " + currentMirrors.ToString();
                int clipToPlay = UnityEngine.Random.Range(0, removeMirror.Count);
                if (audioManager != null)
                {
                    AudioSource.PlayClipAtPoint(removeMirror[clipToPlay], Camera.main.transform.position, audioManager.GetFXVolume());
                }
            }
        }
    }

    private void PlaceMirror(Vector2 gridPos)
    {
        SpawnMirror(gridPos);
    }

   public void IncreaseMirrorCount()
    {
        currentMirrors++;
        mirrorCount.text = "Mirrors: " + currentMirrors.ToString();
    }

    private Vector2 GetSquareClicked()
    {
        Vector2 clickPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(clickPos);
        Vector2 gridPos = SnapToGrid(worldPos);
        return gridPos;
    }

    private Vector2 SnapToGrid(Vector2 rawWorldPos)
    {
        float newX = Mathf.RoundToInt(rawWorldPos.x); 
        float newY = Mathf.RoundToInt(rawWorldPos.y);
        return new Vector2(newX, newY);
    }

    private void SpawnMirror(Vector2 roundedPos)
    {
        Instantiate(mirror, roundedPos, transform.rotation);
    }
}
