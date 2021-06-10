using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureHunter : MonoBehaviour
{

    //config params
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float stepFrequency = 0.3f;
    float currentStepTimer;
    [SerializeField] List<AudioClip> footsteps;
    [SerializeField] List<AudioClip> winGrunts;


    //ref caches;
    LevelManager levelManager;
    MirrorManager mirrorManager;
    List<Vector2> directions;
    [SerializeField] AudioManager audioManager;


    bool levelStart;
    Mirror moveTarget;
    Mirror lastTarget;
    Mirror secondLastTarget;
    Animator myAnimator;
    


    private void Start()
    {
        levelStart = false;
        levelManager = FindObjectOfType<LevelManager>();
        mirrorManager = FindObjectOfType<MirrorManager>();
        directions = new List<Vector2> { new Vector2(1f, 0f), new Vector2(0f, 1f), new Vector2(-1f, 0f), new Vector2(0f, -1f) };
        myAnimator = GetComponent<Animator>();
        currentStepTimer = stepFrequency;
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void StartMoving()
    {
        moveTarget = FindNextTarget();
        if(moveTarget == null) { return; }
        levelStart = true;
        mirrorManager.DisableMirrorAdd();
    }

    private void Update()
    {
        if (!levelStart) { return; }
        MoveHunter();
        Footsteps();
    }

    private void Footsteps()
    {
        if (moveTarget == null) { return; }
        if (currentStepTimer > 0f)
        {
            currentStepTimer -= Time.deltaTime;
        }
        else
        {
            currentStepTimer = stepFrequency;
            var clipIndex = UnityEngine.Random.Range(0, footsteps.Count);
            if (audioManager != null)
            {
                AudioSource.PlayClipAtPoint(footsteps[clipIndex], Camera.main.transform.position, audioManager.GetFXVolume());
            }
        }
    }

    private void MoveHunter()
    {
        if (moveTarget == null) { moveTarget = FindNextTarget(); }
        //Debug.Log("Moving to " + moveTarget.name);
        //if (lastTarget != null)
        //{ Debug.Log("Last Target = " + lastTarget.name); }
        //if (secondLastTarget != null)
        //{ Debug.Log("Second Last Target = " + secondLastTarget.name); }
        var targetPos = moveTarget.transform.position;
        if (targetPos != transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * moveSpeed);
            TurnSprite(targetPos);
            if (transform.position == moveTarget.transform.position)
            {
                transform.position = targetPos;
                if (transform.position == targetPos)
                {
                    GoalCheck(moveTarget);
                    if (lastTarget != null)
                    {
                        secondLastTarget = lastTarget;
                    }
                    if (moveTarget != null)
                    {
                        lastTarget = moveTarget;
                    }
                    moveTarget = FindNextTarget();
                }
            }
        }
    }

    private void TurnSprite(Vector3 targetPos)
    {
        var direction = transform.position - targetPos;
        direction.z = 0f;
        direction = direction.normalized;
        Vector3 theScale = transform.localScale;
        if (direction.y > 0)
        {
            if (theScale.x < 0)
            {
                theScale.x *= -1;
                transform.localScale = theScale;
            }
            myAnimator.SetBool("WalkingUp", false);
            myAnimator.SetBool("WalkingRight", false);
            myAnimator.SetBool("WalkingDown", true);
        }
        else if (direction.y < 0)
        {
            if (theScale.x < 0)
            {
                theScale.x *= -1;
                transform.localScale = theScale;
            }
            myAnimator.SetBool("WalkingRight", false);
            myAnimator.SetBool("WalkingDown", false);
            myAnimator.SetBool("WalkingUp", true);
        }
        else if (direction.x < 0)
        {
            if (theScale.x < 0)
            {
                theScale.x *= -1;
                transform.localScale = theScale;
            }
            myAnimator.SetBool("WalkingDown", false);
            myAnimator.SetBool("WalkingUp", false);
            myAnimator.SetBool("WalkingRight", true);
        }
        else if (direction.x > 0)
        {
            if (!myAnimator.GetBool("WalkingRight"))
            {
                theScale.x *= -1;
                transform.localScale = theScale;
            }
            myAnimator.SetBool("WalkingDown", false);
            myAnimator.SetBool("WalkingUp", false);
            myAnimator.SetBool("WalkingRight", true);
            
        }
    }

    private void GoalCheck(Mirror currentTarget)
    {
        var isGoal = currentTarget.CheckForGold();
        if (isGoal)
        {
            levelStart = false;
            if (transform.localScale.x < 0)
            {
                var theScale = transform.localScale;
                theScale.x *= -1;
                transform.localScale = theScale;
            }
            myAnimator.SetBool("Win", true);
            levelManager.WinLevel();
            var clipIndex = UnityEngine.Random.Range(0, winGrunts.Count);
            if (audioManager != null)
            {
                AudioSource.PlayClipAtPoint(winGrunts[clipIndex], Camera.main.transform.position, audioManager.GetFXVolume());
            }
        }
    }

    private Mirror FindNextTarget()
    {
        Mirror foundTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        List<Mirror> targets = FindTargetsRayCast();
        foreach (Mirror potentialTarget in targets)
        {
            if (potentialTarget != lastTarget && potentialTarget != secondLastTarget && potentialTarget.transform.position != transform.position)
            {
                Vector3 directionToTarget = potentialTarget.transform.position - currentPos;
                directionToTarget.z = 0;
                if (Mathf.Approximately(directionToTarget.x, 0.0f) || Mathf.Approximately(directionToTarget.y, 0.0f))
                {
                    float distanceSqrToTarget = directionToTarget.sqrMagnitude;
                    if (FindWallRayCast(directionToTarget, distanceSqrToTarget))
                    {
                        if (distanceSqrToTarget < closestDistanceSqr)
                        {
                            closestDistanceSqr = distanceSqrToTarget;
                            foundTarget = potentialTarget;
                        }
                    }
                }
            }
        }
        return foundTarget;
    }

    private List<Mirror> FindTargetsRayCast()
    {
        List<Mirror> mirrorsFound = new List<Mirror>();
        foreach (Vector2 direction in directions)
        {
            var foundMirrors = Physics2D.RaycastAll(transform.position, direction, Mathf.Infinity, LayerMask.GetMask("Mirrors"));
            foreach (RaycastHit2D mirror in foundMirrors)
            {
                Mirror foundMirrorComponent = mirror.collider.GetComponent<Mirror>();
                if (mirrorsFound.Count == 0)
                {
                    mirrorsFound.Add(foundMirrorComponent);
                }
                else if (!mirrorsFound.Contains(foundMirrorComponent))
                {
                    mirrorsFound.Add(foundMirrorComponent);
                }
            }
        }
        return mirrorsFound;
    }

    private bool FindWallRayCast(Vector2 directiontoTarget, float distanceToTargetSqr)
    {
        float distanceToTarget = Mathf.Sqrt(distanceToTargetSqr);
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, directiontoTarget, distanceToTarget, LayerMask.GetMask("Walls"));
        if (hit)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void LevelFail()
    {
        levelManager.RestartLevel();
    }

    public void CheckForNewTargets()
    {
        //Debug.Log("It's an intersection");
        
        moveTarget = FindNextTarget();
    }

    public bool isGameStarted()
    {
        return levelStart;
    }    
 
}
