using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;
using System;
[AddComponentMenu("ACS Mouse Control")]

public class ACSMouse : MonoBehaviour
{   
    
    [SerializeField]
    public bool InvCamera = false;
    [SerializeField]
    public float pitchAng;
    [SerializeField]
    public float yawAng;
    [SerializeField]
    public float rollAng = 0;
    [SerializeField]
    public float sens = 0.25f;
    [SerializeField]
    public int smoothFrameCount = 10;
    [SerializeField]
    public float xAbsBound = 90;
    [SerializeField]
    public float yAbsBound = 90;
    protected Vector3 mousePos;
    public Vector3 currCraftAtt; //Need to find a way to have modify
    protected Vector3[] smoothMouseDelta = new Vector3[10];
    protected int smoothCount;
    private bool startSmoothing;
    private int iterateSmooth;
    
    void Update() //Still need to add look limits (Assuming 90 off of each point?)
    {
        if(Input.GetKeyDown(KeyCode.R) || Input.GetKeyUp(KeyCode.R))
        {
            if(!startSmoothing)
            {
            //Calculate frames needed between center and mouse pos
            startSmoothing = true;
            Debug.Log("TEST Starting smoothing");
            }
        }
    
        if(Input.GetKey(KeyCode.R))
        {
            //Add gantry for implementation on smoothing transitions
            mousePos = Input.mousePosition;

            yawAng = ((mousePos.y/2)*sens);
            pitchAng = ((mousePos.x/2)*sens);

            if(!InvCamera)
            {
                pitchAng = pitchAng*-1;
            }
            //Attitude limiting. Needs work
            if(yawAng > xAbsBound)
            {
                yawAng = xAbsBound;
            }
            if(yawAng < -xAbsBound)
            {
                yawAng = -xAbsBound;
            }
            if(pitchAng > yAbsBound)
            {
                pitchAng = yAbsBound;
            }
            if(pitchAng < -yAbsBound)
            {
                pitchAng = -yAbsBound;
            }
            mousePos.Set(yawAng, pitchAng ,rollAng);

            transform.eulerAngles = mousePos;
            if(sens <= 0)
            {
                sens = 0.1f;
            }

            if(sens > 1)
            {
                sens = 1f;
            }
        } else {
        //Correcting to level (Rollwise) with craft

        transform.eulerAngles = SyncAtt();
        }
         //Works, a tad bit janky though on trackpad.
        //Need to find a way to reset cursor pos to (0,0), as well as a way to make it natural
        if(startSmoothing == true)
        {
            if(iterateSmooth >= smoothCount)
            {
            startSmoothing = false;
            Debug.Log("TEST Stop Smoothing.");
            } else {
                iterateSmooth++;
            }
        }
        
    }

    void SmoothCamReturn(Vector3 currPos, Vector3 targetPos)
    { //Uses pointers to keep updating
        float xDelta = Math.Abs(currPos.x-targetPos.x); 
        float yDelta = Math.Abs(currPos.y-targetPos.y);
        float xDeltaInc = xDelta/smoothFrameCount;
        float yDeltaInc = yDelta/smoothFrameCount;
        
        for(int i = smoothFrameCount - 1; i >= 0; i--)
        {
            smoothMouseDelta[i] = new Vector3 (xDeltaInc*(i+1), yDeltaInc*(i+1), 0); //Need to modify to sync with craft
        }
    }

  public void SetCurrCraftAtt()
    {

    }

   public Vector3 SyncAtt()
    {
        Debug.Log("TEST Sync attitude");
        return new Vector3 (0,0,0);
    }
}

