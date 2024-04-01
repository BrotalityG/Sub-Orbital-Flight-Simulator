using System.Collections;
using System.Collections.Generic;
using Calculator;
using TMPro;
using UnityEngine;
[AddComponentMenu("ACS/HUD")]

public class ACSHUD : MonoBehaviour
{

    [SerializeField]
    protected TextMeshProUGUI hudBasic;
    [SerializeField]
    protected Rigidbody shut; 
    protected Vector3 shutPos = new Vector3(0,0,0);
    protected Vector3 shutAtt = new Vector3(0,0,0);
    public Keyboard keyboard;
    public GeneralCalculations calculations;

    // Update is called once per frame
    void Update()
    {
        shutAtt = shut.rotation.eulerAngles;
        shutPos = shut.position;
         UpdateHUD();
    }

    private void UpdateHUD() //Updates HUD to accurately represent craft's behavior.
    {
        hudBasic.text = "Throttle: " + keyboard.getThrottle() + "%\n";
        hudBasic.text += "IAS: " + $"{calculations.getIAS():0.00}" + "km/h\n";
        if(calculations.getAlt() < 30000f)
        { 
            hudBasic.text += "GS: " + $"{calculations.getGS():0.00}" + "km/h\n"; 
            hudBasic.text += "Mach: " + $"{calculations.getMach():0.00}" + "\n";
        }
        if(calculations.getFuel() > 10f)
        {
            hudBasic.text += "Fuel: " + $"{calculations.getFuel():0.00}" + "%\n"; 
        } else if(calculations.getFuel() > 0f) {
            hudBasic.text += "Fuel: " + $"{calculations.getFuel():0.00}" + "% FUEL LOW\n";
        } else {
            hudBasic.text += "NO FUEL REMAINING\n";
        }
        if(keyboard.getRCS() == true)
        {
            if(calculations.getFuelRCS() > 10f)
            {
                hudBasic.text += "RCS fuel: " + $"{calculations.getFuel():0.00}" + "%\n"; 
            } else if(calculations.getFuelRCS() > 0f) {
                hudBasic.text += "RCS fuel: " + $"{calculations.getFuel():0.00}" + "% FUEL LOW\n";
            } else {
                hudBasic.text += "NO RCS FUEL REMAINING\n";
            }
        }
        if(calculations.getAlt() < 30000f){
            hudBasic.text += "Altitude: " + calculations.getAlt() + "m\n";
        } else {
            hudBasic.text += "Altitude: " + $"{(calculations.getAlt()/1000f):0.00}" + "km\n";
        }
        hudBasic.text += "Flap angle: " + calculations.getFlapPos() +  "deg\n"; //Populate when possible
        hudBasic.text += "Pitch angle: " + $"{shutAtt.x:0.0}" + "deg\n"; //Need to double check, need to add negative values for over 180deg
        hudBasic.text += "Roll angle: " + $"{shutAtt.z:0.0}" + "deg\n"; //Need to double check axi
    }
}