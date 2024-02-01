using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// g = G(m1*m2)/r^2 - This is the formula for the force of gravity between two objects, where m is kg and r is meters.

public class GeneralCalculations : MonoBehaviour
{
    // These will be visible to other classes.
    public float ASL = 0f;
    protected static int MSLTempC = 15; // This is in Degrees Celsius
    protected static float MSLTempK = MSLTempC + 273.15f; // This is in Kelvin
    protected float TempK = MSLTempK;
    protected float AirPressure = 1013.25f; // This is in hPa


    // These will be private so only this class can edit/view it.
    private float LapseRate = 0.0065f; // This is in degrees Celsius per meter
    private float LastAltitude = 0f;
    private int EarthRadius = 6371100; // This is in meters
    private float EarthMass = 5.972f * Mathf.Pow(10, 24); // This is in kg
    private float ShuttleMass = 110000; // This is in kg (This assumes loaded mass of the shuttle.)
    private float r = EarthRadius + ASL; // This is in meters
    private float G = 6.67408f * Mathf.Pow(10, -11); // This is in m^3 kg^-1 s^-2
    private float g = G * (EarthMass*ShuttleMass / Mathf.Pow(r, 2))


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the lapse rate.
        CalculateLapseRate();

        // Calculate temperature at altitude.
        CalculateTemperatureAtAltitude();

        // Calculate the force of gravity at altitude.
        CalculateGravity();
    }

    private void CalculateGravity()
    {
        // The force of gravity at a given altitude can be calculated using the following formula:
        // F = G(m1*m2)/r^2
        // Where:
        // F = force of gravity
        // G = gravitational constant
        // m1 = mass of first object
        // m2 = mass of second object
        // r = distance between the centers of the masses of the objects

        // The mass of the object is the mass of the Earth.
        r = EarthRadius + ASL;

        g = G * (EarthMass*ShuttleMass / Mathf.Pow(r, 2));
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

        AirPressure = Mathf.Pow((1-((LapseRate/MSLTempK)*ASL)), (g/(R*LapseRate)));
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
        TempK -= (LapseRate * ChangeInAltitude);
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
