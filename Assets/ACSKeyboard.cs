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
    protected Rigidbody shut; 
    protected Vector3 shutPos = new Vector3(0,0,0);
    protected Vector3 shutAtt = new Vector3(0,0,0);

    // Start is called before the first frame update
    void Start()
    {
        //Keep just in case
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    void pitch(bool direction)
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
    void roll(bool direction)
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
    void yaw(bool direction)
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

    public int getThrustVal()
    {
        return ThrustVal;
    }
}
