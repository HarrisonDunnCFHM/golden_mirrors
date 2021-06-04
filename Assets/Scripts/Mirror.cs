using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    //config params
    [SerializeField] bool isGold;
    [SerializeField] bool seesGold;
    [SerializeField] bool isPermanent;
    [SerializeField] Sprite normal;
    [SerializeField] Sprite showsGold;
    [SerializeField] List<AudioClip> removeMirror;

    //cached refs
    Animator myAnimator;
    SpriteRenderer spriteRenderer;
    List<Vector2> directions;

    private void Start()
    {
        myAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        directions = new List<Vector2> { new Vector2(1f, 0f), new Vector2(0f, 1f), new Vector2(-1f, 0f), new Vector2(0f, -1f) };
        

    }

    private void Update()
    {
        if (!isGold)
        {
            seesGold = LookForGold();
            if (seesGold)
            {
                spriteRenderer.sprite = showsGold;
            }
            else
            {
                spriteRenderer.sprite = normal;
            }

        }
            }

   
    private bool LookForGold()
    {
        foreach (Vector2 direction in directions)
        {
            var objectsHit = Physics2D.RaycastAll(transform.position, direction, Mathf.Infinity, LayerMask.GetMask("Mirrors"));
            foreach (RaycastHit2D objectHit in objectsHit)
            {
                if (objectHit.collider.GetComponent<Mirror>() != null)
                {
                    var directionToHit = objectHit.collider.transform.position - transform.position;
                    if (!FindWallRayCast(directionToHit))
                    {
                        var seesGold = objectHit.collider.GetComponent<Mirror>().SeesGold();
                        var isGold = objectHit.collider.GetComponent<Mirror>().CheckForGold();
                        if (seesGold || isGold)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

 
    private bool FindWallRayCast(Vector2 directiontoTarget)
    {
        float distanceToTarget = directiontoTarget.magnitude;
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, directiontoTarget, distanceToTarget, LayerMask.GetMask("Walls"));
        if (hit)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void OnMouseDown()
    {
        if (isPermanent) { return; }
        var mirrorManager = FindObjectOfType<MirrorManager>();
        mirrorManager.IncreaseMirrorCount();
        int clipToPlay = UnityEngine.Random.Range(0, removeMirror.Count);
        AudioSource.PlayClipAtPoint(removeMirror[clipToPlay], Camera.main.transform.position, 1.0f);
        Destroy(gameObject);
    }

    public bool SeesGold()
    {
        return seesGold;
    }

    public bool CheckForGold()
    {
        return isGold;
    }
}
