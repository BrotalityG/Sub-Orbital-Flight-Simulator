using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;
using System;
using TMPro;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;
using Unity.VisualScripting;
using Calculator;
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
    [SerializeField]
    protected Rigidbody referenceBody;
    [SerializeField]
    protected float xOffset = -30;
    [SerializeField]
    protected float yOffset = 10;
    [SerializeField]
    protected float zOffset = 0;
    protected Vector3 mousePos;
    protected Vector3 currCraftPos;
    public Vector3 currCraftAtt; //Need to find a way to have modify
    protected Vector3[] smoothMouseDelta = new Vector3[10];
    private bool startSmoothing;
    private int iterateSmooth = 0;
    private bool toggleHUD = false;
    private bool toggleCam = true;
    private float maxAlt = 200f;
    private GeneralCalculations calc;
    private ControlSystem.Keyboard kbd;

    void Start()
    {
        calc = FindAnyObjectByType<GeneralCalculations>();
        kbd = FindAnyObjectByType<ControlSystem.Keyboard>();
        Cameras[1].enabled = false; //Third Person View
        Cameras[0].enabled = true; //First Person View
        UpdateCamera();
    }
    
    void Update() //Still need to add look limits (Assuming 90 off of each point?)
    {
        if(Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.LeftAlt))
        {
            iterateSmooth = 0;
            if(!startSmoothing)
            {
            //Calculate frames needed between center and mouse pos
            SmoothCamReturn(mousePos, new Vector3 (0,0,0)); //Second vector is a placeholder
            if(Input.GetKeyUp(KeyCode.LeftAlt))
            {
                ReverseArray();
            }
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
            //Attitude limiting. Should correct to angle of craft
            if(yawAng > xAbsBound+currCraftAtt.x)
            {
                yawAng = xAbsBound+currCraftAtt.x;
            }
            if(yawAng < -xAbsBound+currCraftAtt.x)
            {
                yawAng = -xAbsBound+currCraftAtt.x;
            }
            if(pitchAng > yAbsBound+currCraftAtt.y)
            {
                pitchAng = yAbsBound+currCraftAtt.y;
            }
            if(pitchAng < -yAbsBound+currCraftAtt.y)
            {
                pitchAng = -yAbsBound+currCraftAtt.y;
            }
            mousePos.Set(yawAng, pitchAng ,currCraftAtt.z);
            if(startSmoothing == true)
            {
                if(iterateSmooth > smoothFrameCount)
                {
                    startSmoothing = false;
                    Debug.Log("TEST Stop Smoothing.");
                } else {
                    iterateSmooth++;
                    Debug.Log("Iteration step");
                    Cameras[0].transform.eulerAngles = smoothMouseDelta[iterateSmooth];
                    Cameras[1].transform.eulerAngles = smoothMouseDelta[iterateSmooth];
                }
            } else {
                Cameras[0].transform.eulerAngles = mousePos;
                Cameras[1].transform.eulerAngles = mousePos;
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
            Cameras[0].transform.eulerAngles = currCraftAtt;
            Cameras[1].transform.eulerAngles = currCraftAtt;
        }
        UpdateHUD();
        UpdateBackground();
        UpdateCamera();
        //Works, a tad bit janky though on trackpad.
        //Need to find a way to reset cursor pos to (0,0), as well as a way to make it natural
    }

    void SmoothCamReturn(Vector3 currPos, Vector3 targetPos) //Calculates the increment for a certain number of frames between the current position of the mouse and 0,0, and populates the array with it
    { //Uses pointers to keep updating
        float xDelta = currPos.x-targetPos.x; 
        float yDelta = currPos.y-targetPos.y;
        
        float xDeltaInc = xDelta/smoothFrameCount; //Need to ensure division does not require a recast of smoothFrameCount
        float yDeltaInc = yDelta/smoothFrameCount;
        
        for(int i = 0; i > smoothFrameCount; i++)
        {
            smoothMouseDelta[i] = new Vector3 (xDeltaInc*(i+1), yDeltaInc*(i+1), 0); //Need to modify to sync with craft. Need to double check math
        }

    }

  
   private void SyncAtt() //Syncs camera attitude with craft attitude
    {
        currCraftAtt = referenceBody.transform.eulerAngles;
        Cameras[0].transform.eulerAngles = currCraftAtt;
        Cameras[1].transform.eulerAngles = Cameras[0].transform.eulerAngles;
    }

    private void UpdateCamera() //Updates camera position
    {
        currCraftPos = referenceBody.transform.position;
        Cameras[0].transform.position = new Vector3(currCraftPos.x+10, currCraftPos.y+10, currCraftPos.z);
        Cameras[1].transform.position = new Vector3 (Cameras[0].transform.position.x+xOffset, Cameras[0].transform.position.y+yOffset, Cameras[0].transform.position.z+zOffset);
    }

    private void UpdateHUD() //Will need to fix. Updates HUD to accurately represent craft's behavior. Need a way to pull from AEPE
    {
        hudBasic.text = "Throttle: " + kbd.getThrottle() + "\n";
        hudBasic.text += "IAS: " + calc.getIAS() + "\n"; //+ (rb.velocity.magnitude * 3.6f).ToString("F0") + "km/h\n";
        hudBasic.text += "GS: \n"; //+ ().ToString("F0") + "km/h\n";
        hudBasic.text += "Altitude: " + calc.getAlt() + "m\n";//+ transform.position.y.ToString("F0") + "m";
        hudBasic.text += "Flap Position: 0";//+ Need to work on flaps value. Probably within AEPE
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

    //I'm Lazy, here's this instead of manually fixing SmoothCamReturn
    private void ReverseArray() //Reverses an array's order
    {
        Vector3[] tempArr = new Vector3[smoothMouseDelta.Length];
        for(int i = 0; i < smoothMouseDelta.Length; i++)
        {
            tempArr[Math.Abs(i-smoothMouseDelta.Length)] = smoothMouseDelta[i];
        }
        smoothMouseDelta = tempArr;
    }
    /*private void GenerateHUD()//TODO: Set to run on Start. Current snippet was taken from the Unity API for reference materials. NOT NEEDED FOR THIS SPRINT
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
    }*/

    void UpdateBackground() //Borrowed from Unity API for now.
    {
        Color color1 = new Color(0.663f,0.776f,0.875f,1); //Floor
        Color color2 = new Color(0.027f,0.047f,0.151f,1); //Ceiling, not complete black to be nicer on the eyes
        float Altitude = Cameras[0].transform.position.y;
        float altP = Altitude/maxAlt;

        Cameras[0].backgroundColor = Color.Lerp(color1, color2, altP);
        Cameras[1].backgroundColor = Color.Lerp(color1, color2, altP);
    }
}

