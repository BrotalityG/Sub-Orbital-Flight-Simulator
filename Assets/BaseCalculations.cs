/*
    * This is a script that contains the base calculations for the game. This includes the force of gravity, the atmosphere, and aerodynamics.
    * This script is attached to the ship object.

    Code References:
        ! Cross Sectional Area Calculations:
            * MatthewFK: https://forum.unity.com/threads/how-do-i-get-the-cross-sectional-surface-area-of-my-geometry-mesh-in-meters.461278/
            * dLopreiato: https://gist.github.com/dLopreiato/7fd142d0b9728518552188794b8a750c
            * dLopreiato references this: https://loyc-etc.blogspot.com/2014/05/2d-convex-hull-in-c-45-lines-of-code.html

    * The following is a list of the formulas used in this script:

    * g = G(m1*m2)/r^2 - This is the formula for the force of gravity between two objects, where m is kg and r is meters.
    * F = m * g - This is the formula for the force of gravity, where m is mass and g is the acceleration due to gravity.
    * Relative Density = P*(T0/T) - This is the formula for the relative density of air, where P is the relative air pressure, T0 is the temperature at sea level, and T is the temperature at altitude.
    * P = (1-((B/T0)h))^(g/RB) - This is the formula for air pressure, where B is the lapse rate, T0 is the temperature at sea level, h is the altitude, g is the acceleration due to gravity, and R is the gas constant.
    * T = T0 + (B * h) - This is the formula for the temperature at a given altitude, where T is the temperature at altitude, T0 is the temperature at sea level, B is the lapse rate, and h is the altitude.
    * Lapse Rate = 0.0065f - This is the lapse rate at altitudes less than 11,000 meters.
    * Lapse Rate = 0f - This is the lapse rate at altitudes between 11,000 and 20,000 meters.
    * Lapse Rate = -0.001f - This is the lapse rate at altitudes between 20,000 and 32,000 meters.
    * Lapse Rate = -0.0028f - This is the lapse rate at altitudes between 32,000 and 47,000 meters.
    * Lapse Rate = 0f - This is the lapse rate at altitudes between 47,000 and 51,000 meters.
    * Lapse Rate = 0.0028f - This is the lapse rate at altitudes between 51,000 and 71,000 meters.
    * Lapse Rate = 0.002f - This is the lapse rate at altitudes between 71,000 and 86,000 meters.
    * Lapse Rate = 0f - This is the lapse rate at altitudes above 86,000 meters.
    * Drag = (1/2)*(D)*(V^2)*(Cd)*(A) - This is the formula for drag, where D is the air density, V is the velocity, Cd is the drag coefficient, and A is the cross-sectional area.
    * Lift = (1/2)*(D)*(V^2)*(Cl)*(A) - This is the formula for lift, where D is the air density, V is the velocity, Cl is the lift coefficient, and A is the cross-sectional area.
    * Normal Force = L*cos(a) + D*sin(a) - This is the formula for the normal force, where L is the lift, D is the drag, and a is the angle of attack (simplified to the object's forward vector vs the worl's forward vector).

*/

using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GeneralCalculations : MonoBehaviour
{
    // Constants
    private static float G = 6.67430f * Mathf.Pow(10, -11); // This is in m^3/(kg*s^2)

    // These will be visible to other classes.
    [SerializeField]
    private float fuel = 100f;
    [SerializeField]
    private float fuelRCS = 100f;
    [SerializeField]
    protected float DragCoefficient = 2.2f;
    [SerializeField]
    protected float LiftCoefficient = 1f;
    [SerializeField]
    protected float ASL = 0f;
    private static int MSLTempC = 15; // This is in Degrees Celsius
    private static float MSLTempK = MSLTempC + 273.15f; // This is in Kelvin
    [SerializeField]
    private float TempK = MSLTempK;
    [SerializeField]
    private float AirPressure = 1013.25f; // This is in hPa
    [SerializeField]
    private float Density = 1.225f; // This is in kg/m^3
    private static float PlanetMass = 5.972f * Mathf.Pow(10, 24); // This is in kg
    private static float PlanetRadius = 6371000; // This is in meters
    [SerializeField]
    private float Gravity = G*PlanetMass/(Mathf.Pow(PlanetRadius, 2)); // This is in m/s^2
    [SerializeField]
    private float drag = 0f;
    private float lift = 0f;
    private float Responsiveness = 100000f;
    private float newtonWeight = 0f; // This is in Newtons

    // These will be private so only this class can edit/view it.
    private float LapseRate = 0.0065f; // This is in degrees Celsius per meter
    private float LastAltitude = 0f;
    [SerializeField]
    private float crossSectionalArea = 1f;
    private Rigidbody rb;
    [SerializeField]
    private float AirSpeed = 0f;
    [SerializeField]
    private float GroundSpeed = 0f;
    [SerializeField]
    private float Speed = 0f;
    [SerializeField]
    private int SpeedDirection = 0;
    [SerializeField]
    private float MachNumber = 0f;
    private float flapsEng = 0;
    private float spBreakEng = 0;
    [SerializeField]
    private float yCount = 2; // Measured in 10s of KM

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get the rigidbody so we don't need to automatically assign it.
        rb.useGravity = false;
        ASL = 100f;
    }

    public void toggleFlapPos()
    {
        if(flapsEng == 1)
        {
             flapsEng = 0;
        } else {
            flapsEng = 1;
        }
    }

     public void toggleBrakePos()
    {
        if(spBreakEng == 1)
        {
            spBreakEng = 0;
        } else {
            spBreakEng= 1;
        }
    }

    public bool getABPos()
    {
        if(spBreakEng == 1)
        {
            return true;

        } else {
            return false;
        }
    }

     public float getFlapPos()
    {
        if(flapsEng == 1)
        {
            return 22.55f;
        } else {
            return 0;
        }
    }

    public float getFuel()
    {
        return fuel;
    }

    public float getFuelRCS()
    {
        return fuelRCS;
    }

    public void updateRCSFuel()
    {
        fuelRCS -= 0.001f;
        rb.mass -= 3.2f;
    }

    public float setFuel(float fuel)
    {
        this.fuel = fuel;
        return fuel;
    }
    public float getIAS()
    {
        return AirSpeed;
    }

    public float getSpeed()
    {
        return Speed;
    }

    public float getGS()
    {
        return GroundSpeed;
    }

    public float getAlt()
    {
        return ASL;
    }

    public float getDensity()
    {
        return Density;
    }

    public float getMach()
    {
        return MachNumber;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position.y >= 10000f) {
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
            yCount++;
        } else if (transform.position.y <= -10000f) {
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
            yCount--;
        }

        ASL = 10000f*yCount + transform.position.y;


        // 100,000 meters is the maximum altitude for the atmosphere.
        if (ASL < 100000) {
            CalculateAtmosphere();
        } else {
            AirPressure = 0f;
            Density = 0f;
        }

        // Calculate the force of gravity.
        CalculateGravity();

        // Calculate aerodynamics
        CalculateAerodynamics();

        Vector3 projection = Vector3.Project(rb.velocity, transform.forward);
        float dot = Vector3.Dot(rb.velocity.normalized, transform.forward);

        switch (dot) {
            case float n when n > 0:
                SpeedDirection = 1;
                break;
            case float n when n < 0:
                SpeedDirection = -1;
                break;
            case float n when n == 0:
                SpeedDirection = 0;
                break;
        }

        Speed = rb.velocity.magnitude;
        AirSpeed = projection.magnitude * SpeedDirection;
        GroundSpeed = Speed * (ASL/304.8f) * 0.02f + Speed;
        if (float.IsNaN(GroundSpeed)) {
            GroundSpeed = 0f;
        }

        MachNumber = Speed/(331f*Mathf.Sqrt(TempK/273.15f));
    }

    private void CalculateAtmosphere()
    {
        // Calculate the lapse rate.
        CalculateLapseRate();

        // Calculate temperature at altitude.
        CalculateTemperatureAtAltitude();

        // Calculate air pressure.
        CalculateAirPressure();

        // Calculate the air density.
        CalculateDensity();
    }

    private void CalculateAerodynamics()
    {
        // Very important that this gets calculated FIRST.
        CalculateCrossSection();

        // Calculate the drag.
        CalculateDrag();

        // Calculate the lift.
        CalculateLift();

        // Apply the forces.
        rb.AddForce(rb.velocity.normalized * -drag, ForceMode.Force);
        rb.AddForce(transform.up * lift, ForceMode.Force);
        
        // Calculate and apply angular drag.
        CalculateAngularDrag();
    }

    private void CalculateGravity()
    {
        // You may be asking why this is a different formula than the one commonly associated with gravity; and to that the response is simple:
        // This is only the force of gravity exerted by the planet on 1 kg, which is the acceleration of another mass towards it due to gravity.
        // To get the gravity between the two objects, we utilize the typical F=m*a formula, and that applies the gravitational force as expected.
        // g = G(m)/r^2
        // Where:
        // g = acceleration due to gravity (m/s^2)
        // G = gravitational constant
        // m = Mass of planetary body (kg)
        // r = distance between the centers of the masses

        // The force of gravity is the force with which the Earth, Moon, or other massively large object attracts another object towards itself.
        // By definition, this is the weight of the object.

        // The force of gravity is the mass of the object multiplied by the acceleration due to gravity.
        // F = m * g
        // Where:
        // F = force of gravity
        // m = mass of the object
        // g = acceleration due to gravity

        // Firstly, lets get the gravitational acceleration at the current altitude:
        Gravity = G*PlanetMass/Mathf.Pow(PlanetRadius + ASL, 2);

        // Now, lets calculate and apply the force of gravity:
        newtonWeight = rb.mass * Gravity;
        rb.AddForce(Vector3.down * newtonWeight, ForceMode.Force);
    }

    private void CalculateCrossSection()
    {   float varUp = 426.81f + (6.91f*1.83f*Mathf.Cos(math.PI/180f*22.55f)*flapsEng);
        float varFront = 48.38f + (6.91f*1.83f*Mathf.Sin(math.PI/180f*22.55f)*flapsEng);
        float varRight = 313.83f + (9.03f*spBreakEng*2);

        float frontal = Mathf.Abs(Vector3.Dot(rb.velocity.normalized, transform.forward)) * varFront;

        float up = Mathf.Abs(Vector3.Dot(rb.velocity.normalized, transform.up)) * varUp;
       
        float right = Mathf.Abs(Vector3.Dot(rb.velocity.normalized, transform.right)) * varRight;

        float approx = frontal + up + right;

        crossSectionalArea = approx;
       
    }

    private void CalculateAngularDrag()
    {
        // idfk how this works but it does
        rb.AddTorque(-rb.angularVelocity * Responsiveness * (Density/1.225f));
    }

    private void CalculateDrag()
    {
        // Drag = (1/2)*(D)*(V^2)*(Cd)*(A)
        // Where:
        // D = air density
        // V = velocity
        // Cd = drag coefficient
        // A = cross-sectional area
        
        drag = 1f/2f * Density * Mathf.Pow(rb.velocity.magnitude, 2) * DragCoefficient * crossSectionalArea;
    }

    private void CalculateLift()
    {
        // Lift = (1/2)*(D)*(V^2)*(Cl)*(A)
        // Where:
        // D = air density
        // V = velocity
        // Cl = lift coefficient
        // A = cross-sectional area

        lift = 1f/2f * Density * Mathf.Pow(rb.velocity.magnitude, 2) * LiftCoefficient * crossSectionalArea;
    }

    private void CalculateDensity()
    {
        // Relative Density = P*(T0/T)
        // Where:
        // P =  relative air pressure
        // T0 = temperature at sea level
        // T = temperature at altitude

        Density = AirPressure * (MSLTempK/TempK) / 1013.25f * 1.225f;
    }

    private void CalculateAirPressure()
    {
        // P = (1-((B/T0)h))^(g/RB)
        // Where:
        // P = air pressure
        // B = lapse rate
        // T0 = temperature at sea level
        // h = altitude
        // g = acceleration due to gravity
        // R = gas constant

        float R = 287.057f; // This is in J/(mol*K)

        if (LapseRate != 0)
        {
            AirPressure = Mathf.Pow(1-((LapseRate/MSLTempK)*ASL), 9.81f/(R*LapseRate))*1013.25f;
        }
    }

    private void CalculateTemperatureAtAltitude()
    {
        // The temperature at a given altitude can be calculated using the following formula:
        // T = T0 + (B * h)
        // Where:
        // T = temperature at altitude
        // T0 = temperature at sea level
        // B = lapse rate
        // h = altitude

        float ChangeInAltitude = ASL - LastAltitude;
        LastAltitude = ASL;

        // TempK is a summation of all of the temperature changes.
        TempK -= LapseRate * ChangeInAltitude;
    }

    private void CalculateLapseRate()
    {
        // Depending on altitude, the lapse rate changes.
        // The lapse rate is the rate at which temperature changes with an increase in altitude.

        // If the altitude is less than 11,000 meters, the lapse rate is -0.0065 degrees Celsius per meter.
        // If the altitude is between 11,000 and 20,000 meters, the lapse rate is 0 degrees Celsius per meter.
        // If the altitude is between 20,000 and 32,000 meters, the lapse rate is 0.001 degrees Celsius per meter.
        // If the altitude is between 32,000 and 47,000 meters, the lapse rate is 0.0028 degrees Celsius per meter.
        // If the altitude is between 47,000 and 51,000 meters, the lapse rate is 0 degrees Celsius per meter.
        // If the altitude is between 51,000 and 71,000 meters, the lapse rate is -0.0028 degrees Celsius per meter.
        // If the altitude is between 71,000 and 86,000 meters, the lapse rate is -0.002 degrees Celsius per meter.

        if (ASL < 11000)
        {
            LapseRate = 0.0065f;
        }
        else if (ASL >= 11000 && ASL < 20000)
        {
            LapseRate = 0f;
        }
        else if (ASL >= 20000 && ASL < 32000)
        {
            LapseRate = -0.001f;
        }
        else if (ASL >= 32000 && ASL < 47000)
        {
            LapseRate = -0.0028f;
        }
        else if (ASL >= 47000 && ASL < 51000)
        {
            LapseRate = 0f;
        }
        else if (ASL >= 51000 && ASL < 71000)
        {
            LapseRate = 0.0028f;
        }
        else if (ASL >= 71000 && ASL < 86000)
        {
            LapseRate = 0.002f;
        }
        else // Assume constant temperature above 86000 meters
        {
            LapseRate = 0f;
        }
    }
}