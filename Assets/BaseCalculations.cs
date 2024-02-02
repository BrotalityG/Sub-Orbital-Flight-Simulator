using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// g = G(m1*m2)/r^2 - This is the formula for the force of gravity between two objects, where m is kg and r is meters.

public class GeneralCalculations : MonoBehaviour
{
    // These will be visible to other classes.

    [SerializeField]
    public float ASL = 0f;

    [SerializeField]
    public float gravity = 0f;
    [SerializeField]
    protected static int MSLTempC = 15; // This is in Degrees Celsius
    [SerializeField]
    protected static float MSLTempK = MSLTempC + 273.15f; // This is in Kelvin
    [SerializeField]
    protected float TempK = MSLTempK;

    [SerializeField]
    protected float AirPressure = 1013.25f; // This is in hPa

    [SerializeField]
    protected float Density = 1.225f; // This is in kg/m^3\

    [SerializeField]
    Rigidbody rb;

    // These will be private so only this class can edit/view it.
    private float LapseRate = 0.0065f; // This is in degrees Celsius per meter
    [SerializeField]
    private float LastAltitude = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ASL = transform.position.y;
        Physics.gravity = new Vector3(0, -gravity, 0);

        if (ASL < 100000) // 100,000 meters is the maximum altitude for the atmosphere.
        {
            CalculateAtmosphere();
        }
        else
        {
            // If the altitude is greater than 100,000 meters, the temperature is -23.4 degrees Celsius.
            AirPressure = 0f;
            Density = 0f;
        }
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

    private void CalculateDensity()
    {
        // Relative Density = P*(T0/T)
        // Where:
        // P =  relative air pressure
        // T0 = temperature at sea level
        // T = temperature at altitude

        Density = (AirPressure * (MSLTempK/TempK) / 1013.25f) * 1.225f;
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

        AirPressure = (Mathf.Pow((1-((LapseRate/MSLTempK)*ASL)), (9.81f/(R*LapseRate))))*1013.25f;
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
            LapseRate = 0.00000000000000001f;
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
            LapseRate = 0.00000000000000001f;
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
            LapseRate = 0.00000000000000001f;
        }
    }
}
