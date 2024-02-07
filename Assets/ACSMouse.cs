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
    protected Vector3 currCraftPos;
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
            iterateSmooth = 0;
            } else {
                iterateSmooth++;
            }
        }
        
    }

    void SmoothCamReturn(Vector3 currPos, Vector3 targetPos)
    { //Uses pointers to keep updating
        float xDelta = currPos.x-targetPos.x; 
        float yDelta = currPos.y-targetPos.y;
        
        float xDeltaInc = xDelta/smoothFrameCount; //Need to ensure division does not require a recast of smoothFrameCount
        float yDeltaInc = yDelta/smoothFrameCount;
        
        for(int i = 0; i >= smoothFrameCount; i++)
        {
            smoothMouseDelta[i] = new Vector3 (xDeltaInc*(i+1), yDeltaInc*(i+1), 0); //Need to modify to sync with craft. Need to double check math
        }

    }

  public void SetCurrCraftAtt()
    {
        //Needs to fetch current craft attitude and position.
    }

   private Vector3 SyncAtt()
    {
        SetCurrCraftAtt;
        Debug.Log("TEST Sync attitude");
        //Need to find a way to take the postiion/attitude of craft to refer the camera to.
        return new Vector3 (0,0,0);
    }
}

