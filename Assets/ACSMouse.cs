using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;
using System;
using TMPro;
using UnityEngine.UIElements;
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
    public float xAbsBound = 45;
    [SerializeField]
    public float yAbsBound = 45;
    [SerializeField] 
    private TextMeshProUGUI hudBasic;
    [SerializeField]
    private TextMeshProUGUI hudAttitude;
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
        GenerateHUD();
    }
    
    void Update() //Still need to add look limits (Assuming 90 off of each point?)
    {
        if(Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.LeftAlt))
        {
            if(!startSmoothing)
            {
            //Calculate frames needed between center and mouse pos
<<<<<<< Updated upstream
            SmoothCamReturn(mousePos, new Vector3 (0,0,0)); //Second vector is a placeholder
            if(Input.GetKeyUp(KeyCode.LeftAlt))
            {
                ReverseArray();
            }
=======
            SmoothCamReturn(mousePos, new Vector3 (0,0,0)); //Second vector is a placeholder. Something wrong here.
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
    void SmoothCamReturn(Vector3 currPos, Vector3 targetPos) //Calculates the increment for a certain number of frames between the current position of the mouse and 0,0, and populates the array with it
    { //Uses pointers to keep updating
=======
    void SmoothCamReturn(Vector3 currPos, Vector3 targetPos)
    { //Uses pointers to keep updating. Need to test, something not workong
>>>>>>> Stashed changes
        float xDelta = currPos.x-targetPos.x; 
        float yDelta = currPos.y-targetPos.y;
        
        float xDeltaInc = xDelta/smoothFrameCount; //Need to ensure division does not require a recast of smoothFrameCount
        float yDeltaInc = yDelta/smoothFrameCount;
        
        for(int i = 0; i > smoothFrameCount; i++)
        {
            smoothMouseDelta[i] = new Vector3 (xDeltaInc*(i+1), yDeltaInc*(i+1), 0); //Need to modify to sync with craft. Need to double check math
        }

    }

  private void SetCurrCraftAtt() //Sets current craft attitude to an angle to use SyncAtt
    {
        //Needs to fetch current craft attitude and position.
    }

   private Vector3 SyncAtt() //Syncs camera attitude with craft attitude
    {
        SetCurrCraftAtt();
        //Debug.Log("TEST Sync attitude");
        //Need to find a way to take the postiion/attitude of craft to refer the camera to.
        return new Vector3 (0,0,0);
    }

    private void UpdateHUD() //Will need to fix. Updates HUD to accurately represent craft's behavior
    {
        hudBasic.text = "Throttle: \n"; //+ throttle.ToString("F0") + "%\n";
        hudBasic.text += "Airspeed: \n"; //+ (rb.velocity.magnitude * 3.6f).ToString("F0") + "km/h\n";
        hudBasic.text += "Altitude: \n";//+ transform.position.y.ToString("F0") + "m";
        hudBasic.text += "Flaps: ";//+ transform.position.y.ToString("F0") + "m";
    }

    private void SwapCamera() //Toggles between cameras in the list Cameras per call
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
<<<<<<< Updated upstream

    //I'm Lazy, here's this instead of manually fixing SmoothCamReturn
    private void ReverseArray() //Reverses an array's order
    {
        Vector3[] tempArr = new Vector3[smoothMouseDelta.Length];
        for(int i = 0; i < smoothMouseDelta.Length; i++)
        {
            tempArr[Math.Abs(i-smoothMouseDelta.Length)] = smoothMouseDelta[i];
        }
        smoothMouseDelta = tempArr;
=======
    private void GenerateHUD()//TODO: Set to run on Start. Current snippet was taken from the Unity API for reference materials
    {
        float width = 1;
        float height = 1;
        
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(0, 0, 0),
            new Vector3(width, 0, 0),
            new Vector3(0, height, 0),
            new Vector3(width, height, 0)
        };
        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
        };
        mesh.triangles = tris;

        Vector3[] normals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        mesh.normals = normals;

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = uv;

        meshFilter.mesh = mesh;
>>>>>>> Stashed changes
    }
}

