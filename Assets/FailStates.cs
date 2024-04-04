using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;


public class FailStates : MonoBehaviour
{
    public Rigidbody rb;
    private Keyboard controlKF;
    private GeneralCalculations calc;
    public float fuel;
    public float alt;
    public float roll;
    public float pitch;
    public float yaw; 

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        controlKF = GetComponent<Keyboard>(); // Replace 'ControlSystem' with the appropriate class name from the 'Calculator' namespace.
        calc = GetComponent<GeneralCalculations>();

        roll = Input.GetAxis("Roll");
        pitch = Input.GetAxis("Pitch");
        yaw = Input.GetAxis("Yaw");

    }

    // Update is called once per frame
    void Update()
    {
        fuel = calc.getFuel();
        alt = calc.getAlt();


        if (alt >= 50000f) {
//          add force to bound downwards
            Debug.Log("ship has reached bounds");
        }
        if (alt <= 0f){ //& rb.velocity.y <= 0){
            // gameover scene pause game
            Debug.Log("ship has reached ground");
        }
        if (fuel <= 0) {
            Debug.Log("You are out of fuel");
            //stop forces
        }

    }
}
