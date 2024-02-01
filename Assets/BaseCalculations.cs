using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralCalculations : MonoBehaviour
{
    // These will be visible to other classes.
    public float ASL = 0f;
    protected static int MSLTempC = 15; // This is in Degrees Celsius
    protected static float MSLTempK = MSLTempC + 273.15f; // This is in Kelvin
    protected float TempK = MSLTempK;

    // These will be private so only this class can edit/view it.
    private float LastAltitude = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate temperature at altitude.
        CalculateTemperatureAtAltitude();
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

        // TempK is a summation of all of the temperature changes.
        TempK -= (CalculateLapseRate() * ChangeInAltitude);
    }

    private float CalculateLapseRate()
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

        float LapseRate = 0f;

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

        return LapseRate;
    }
}
