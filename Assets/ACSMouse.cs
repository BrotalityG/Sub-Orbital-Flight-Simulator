using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;
using System;
using TMPro;
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
    public float xAbsBound = 45f;
    [SerializeField]
    public float yAbsBound = 45f;
    [SerializeField] 
    private TextMeshProUGUI hud;
    [SerializeField]
    public List<Camera> Cameras;
    protected Vector3 mousePos;
    protected Vector3 currCraftPos;
    public Vector3 currCraftAtt; //Need to find a way to have modify
    protected Vector3[] smoothMouseDelta = new Vector3[10];
    private bool startSmoothing;
    private int iterateSmooth = 0;
    private bool toggleHUD = false;
    private bool toggleCam = true;

    void Start()
    {
        Cameras[1].enabled = false; //Third Person View
        Cameras[0].enabled = true; //First Person View
    }
    
    void Update() //Still need to add look limits (Assuming 90 off of each point?)
    {
        if(Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.LeftAlt))
        {
            if(!startSmoothing)
            {
            //Calculate frames needed between center and mouse pos
            SmoothCamReturn(mousePos, new Vector3 (0,0,0)); //Second vector is a placeholder
            startSmoothing = true;
            Debug.Log("TEST Starting smoothing");
            }
        }

        //Switches active Camera
        if(Input.GetKeyDown(KeyCode.I))
        {
            SwapCamera();
        }
    
        if(Input.GetKey(KeyCode.LeftAlt) && toggleCam)
        {
            //Add gantry for implementation on smoothing transitions
            mousePos = Input.mousePosition;

            yawAng = (mousePos.y/2)*sens;
            pitchAng = (mousePos.x/2)*sens;
            
            if(!InvCamera)
            {
                pitchAng *= -1;
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
            if(startSmoothing == true)
            {
                if(iterateSmooth >= smoothFrameCount)
                {
                    startSmoothing = false;
                    Debug.Log("TEST Stop Smoothing.");
                    iterateSmooth = 0;
                } else {
                    iterateSmooth++;
                    Debug.Log("Iteration step");
                    transform.eulerAngles = smoothMouseDelta[iterateSmooth];
                }
            } else {
                transform.eulerAngles = mousePos;
            }
            //Sensitivity limits
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
        UpdateHUD();
        //Works, a tad bit janky though on trackpad.
        //Need to find a way to reset cursor pos to (0,0), as well as a way to make it natural
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

  private void SetCurrCraftAtt()
    {
        //Needs to fetch current craft attitude and position.
    }

   private Vector3 SyncAtt()
    {
        //Debug.Log("TEST Sync attitude");
        //Need to find a way to take the postiion/attitude of craft to refer the camera to.
        return new Vector3 (0,0,0);
    }

    private void UpdateHUD() //Will need to fix
    {
        hud.text = "Throttle \n"; //+ throttle.ToString("F0") + "%\n";
        hud.text += "Airspeed \n"; //+ (rb.velocity.magnitude * 3.6f).ToString("F0") + "km/h\n";
        hud.text += "Altitude \n";//+ transform.position.y.ToString("F0") + "m";
        hud.text += "Flaps ";//+ transform.position.y.ToString("F0") + "m";
    }

    private void SwapCamera()
    {
        if(toggleCam)
        {
            toggleCam = false;
            Cameras[0].enabled = false;
            Cameras[1].enabled = true;
            
        } else {
            toggleCam = true;
            Cameras[1].enabled = false;
            Cameras[0].enabled = true;
        }
    }
}

