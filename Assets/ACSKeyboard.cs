using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("ACS Keyboard Control")]

public class ACSKeyboard : MonoBehaviour
{
    protected float rateOfPitch = 1;
    protected float rateOfRoll = 1;
    protected float rateOfYaw = 1;
    [SerializeField]
    private int RCSRemaining = 500; //Just a placeholder for Sprint 2. Will update as implemented
    [SerializeField]
    private int ThrustVal = 0;
    [SerializeField]
    public float throttleIncrement = 0.1f;

    public float maxThrottle = 100f;
    public float responsiveness = 10f;
    private float throttle;
    private float roll;
    private float pitch;
    private float yaw;
    private float responseMod {
    get 
    {
        return shut.mass / 10f * responsiveness;
    }
    }
    protected Rigidbody shut; 
    protected Vector3 shutPos = new Vector3(0,0,0);
    protected Vector3 shutAtt = new Vector3(0,0,0);
    void Awake()
    {
        shut = GetComponent<Rigidbody>();
    }

    private void HandleInputs() {
        roll = Input.GetAxis("Roll");
        pitch = Input.GetAxis("Pitch");
        yaw = Input.GetAxis("Yaw");

        if (Input.GetKey(KeyCode.LeftShift)) throttle += throttleIncrement;
        else if (Input.GetKey(KeyCode.LeftControl)) throttle -= throttleIncrement;
        throttle = Mathf.Clamp(throttle, 0f, 100f);
    }

    // Update is called once per frame
    void Update()
    {

        HandleInputs();

        /*
        if(Input.GetKey(KeyCode.W))
        {
            pitch(true);
        }
        if(Input.GetKey(KeyCode.S))
        {
            pitch(false);
        }
        if(Input.GetKey(KeyCode.A))
        {
            yaw(true);
        }
        if(Input.GetKey(KeyCode.D))
        {
            yaw(false);
        }
        if(Input.GetKey(KeyCode.Q))
        {
            roll(true);
        }
        if(Input.GetKey(KeyCode.E))
        {
            roll(false);
        }
        if(Input.GetKey(KeyCode.LeftShift))
        {
            if(ThrustVal <= 100)
            {
                incThrust(true);
            }
        }
        if(Input.GetKey(KeyCode.LeftControl))
        {
            if(ThrustVal >= 0)
            {
                incThrust(false);
            }
        }
        if(ThrustVal > 100)
        {
            ThrustVal = 100;
        }
        if(ThrustVal < 0)
        {
            ThrustVal = 0;
        }

        shut.transform.eulerAngles = shutAtt;//Why no respond?

        */
    }
    private void FixedUpdate() {
        float throttlePercent = throttle / maxThrottle;
        print(throttlePercent);

        shut.AddForce(-transform.forward * 5250000f * throttlePercent, ForceMode.Force);
        rollinput();
        pitchinput();
        yawinput();

    }

    void rollinput() {
        shut.AddRelativeTorque(Vector3.right * roll * responseMod);
        print(roll);
    }

    void pitchinput() {
        shut.AddRelativeTorque(Vector3.forward * pitch * responseMod);
        print(pitch);
    }

    void yawinput() {
        shut.AddRelativeTorque(Vector3.up * yaw * responseMod);
        print(yaw);
    }
    /*
    void pitchS(bool direction)
    {
        if(direction)
        {
            shutAtt += new Vector3 (rateOfPitch, 0, 0);
        }
        else
        {
            shutAtt += new Vector3 (rateOfPitch*-1, 0, 0);
        }
    }
    void rollS(bool direction)
    {
        if(direction)
        {
            shutAtt += new Vector3 (0,0, rateOfRoll);
        }
        else
        {
            shutAtt += new Vector3 (0,0, rateOfRoll*-1);
        }
    }
    void yawS(bool direction)
    {
        if(direction)
        {
            shutAtt += new Vector3 (0,rateOfYaw,0);
        }
        else
        {
            shutAtt += new Vector3 (0,rateOfYaw*-1,0);
        }
    }
    void incThrust(bool increase)
    {
        if(increase)
        {
            ThrustVal++;
        }
        else
        {
            ThrustVal--;
        }
    }
    */
    public int getThrustVal()
    {
        return ThrustVal;
    }

    
}