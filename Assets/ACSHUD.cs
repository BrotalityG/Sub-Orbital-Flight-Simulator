using System.Collections;
using System.Collections.Generic;
using Calculator;
using TMPro;
using UnityEngine;
[AddComponentMenu("ACS/HUD")]

public class ACSHUD : MonoBehaviour
{
    private int RCSRemaining = 500; //Just a placeholder for Sprint 2. Will update as implemented
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

    private void UpdateHUD() //Will need to fix. Updates HUD to accurately represent craft's behavior. Need a way to pull from AEPE
    {
        hudBasic.text = "Throttle: " + keyboard.getThrottle() + "%\n";
        hudBasic.text += "IAS: " + calculations.getIAS() + "km/h\n"; //+ (rb.velocity.magnitude * 3.6f).ToString("F0") + "km/h\n";
        hudBasic.text += "GS: " + calculations.getGS() + "km/h\n"; //+ ().ToString("F0") + "km/h\n";
        hudBasic.text += "Fuel: " + calculations.getFuel() + "%\n"; //+ ().ToString("F0") + "km/h\n";
        hudBasic.text += "Altitude: " + calculations.getAlt() + "m\n";//+ transform.position.y.ToString("F0") + "m";
        hudBasic.text += "Pitch angle: " + shutAtt.x + "deg\n"; //Need to double check
        hudBasic.text += "Roll angle: " + shutAtt.z + "deg\n"; //Need to double check axi
    }
}