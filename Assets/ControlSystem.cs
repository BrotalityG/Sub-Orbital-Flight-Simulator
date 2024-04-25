using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Keyboard : MonoBehaviour {
    [SerializeField]
    private int throttleIncrement = 1;
    private int maxThrottle = 100;
    [SerializeField]
    private int throttle = 25;
    [SerializeField]
    private float roll;
    [SerializeField]
    private float pitch;
    [SerializeField]
    private float yaw;
    [SerializeField]
    private float IAS;
    [SerializeField]
    private bool valRCS = false;
    [SerializeField]
    private float impulseRCS = 1179.561615048318f; //Need to verify, current value is in newton meters
    [SerializeField]
    private float Responsiveness = 100000f;
    private float MaximumThrottle = 5255000f; // According to European Space Agency: https://www.esa.int/Science_Exploration/Human_and_Robotic_Exploration/Space_Shuttle/Shuttle_technical_facts
    private Rigidbody rb;
    private GeneralCalculations gc;
    [SerializeField]
    private TextMeshProUGUI PausedText;
    [SerializeField]
    private TextMeshProUGUI HelpUIText;

    [SerializeField]
    private GameObject EscapeMenu;

    public bool PausedBoolean = false;
    public bool helpBool = false;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
        rb = GetComponent<Rigidbody>(); // Get the rigidbody so we don't need to manually assign it.
        gc = GetComponent<GeneralCalculations>(); // Get the GeneralCalculations script so we don't need to manually assign it.
    }

    // Update is called once per frame
    void Update()
    {      
        if (gc.getFuel() > 0f) {
            gc.setFuel(gc.getFuel() - throttle/maxThrottle * Time.deltaTime * .25f); // Subtract fuel based on throttle
        }
        HandleInputs();


            if (Input.GetKeyDown(KeyCode.Escape)) // Pause the game
        {
            if (PausedBoolean == false)
            {
                EscapeMenu.SetActive(true);
                Time.timeScale = 0;
                PausedBoolean = true;
            }
            else
            {
                EscapeMenu.SetActive(false);
                Time.timeScale = 1;
                PausedBoolean = false;

            }
        }

        // Current control list (aside from dynamic roll pitch & yaw from unity.)
    /**
        T flap
        B brake
        Shift & Ctrl throttle & RCS forward/backward
        R RCS up
        F RCS down
        X RCS right
        Z RCS left
        S RCS pitch up
        W RCS pitch down
        E RCS yaw right
        Q RCS yaw left
        A RCS roll left
        D RCS roll right
        CapsLock toggle RCS
        H show help menu
        ESC pause
    **/

        if (Input.GetKeyDown(KeyCode.H)) {
            if (helpBool == false) {
                helpBool = true;
                HelpUIText.text = "Controls: \n W - Pitch Up \n S - Pitch Down \n A - Roll Left \n D - Roll Right \n Q - Yaw Left \n E - Yaw Right \n Left Shift - Increase Throttle/RCS Forward \n Ctrl - Decrease Throttle/RCS Backward \n R - RCS Up \n F - RCS Down \n X - RCS Right \n Z - RCS Left \n T - Toggle Flaps \n B - Toggle Brakes \n CapsLock - Toggle RCS \n H - Toggle Help Menu \n ESC - Pause";
            } else {
                helpBool = false;
                HelpUIText.text = " ";
            }
    }

    }   

    // FixedUpdate is called once per physics update; run physics here
    void FixedUpdate() {
        IAS = gc.getIAS();

        applyRoll();
        applyPitch();
        applyYaw();
        applyThrottle();
    }

    private void applyRoll() {
        rb.AddRelativeTorque(Vector3.forward * roll * Mathf.Clamp(IAS/100, 0, 1) * (gc.getDensity()/1.225f) * Responsiveness);
    }

    private void applyPitch() {
        rb.AddRelativeTorque(Vector3.right * -pitch * Mathf.Clamp(IAS/100, 0, 1) * (gc.getDensity()/1.225f) * Responsiveness);
    }

    private void applyYaw() {
        rb.AddRelativeTorque(Vector3.up * yaw * Mathf.Clamp(IAS/100, 0, 1) * (gc.getDensity()/1.225f) * Responsiveness);
    }

    private void applyThrottle() {
        if (gc.getFuel() <= 0) {
            throttle = 0;
        }

        rb.AddRelativeForce(Vector3.forward * (((float) throttle) / ((float) maxThrottle)) * MaximumThrottle);
    }

    private void HandleInputs() {
        roll = Input.GetAxis("Roll");
        pitch = Input.GetAxis("Pitch");
        yaw = Input.GetAxis("Yaw");

        if(Input.GetKeyDown(KeyCode.T))
        {
            gc.toggleFlapPos();
        }


        if(Input.GetKeyDown(KeyCode.B))
        {
            gc.toggleBrakePos();
        }

        if(valRCS != true) //Need to double check movement
        {
            if (Input.GetKey(KeyCode.LeftShift)) throttle += throttleIncrement;
            else if (Input.GetKey(KeyCode.LeftControl)) throttle -= throttleIncrement;
        } else {
            //Need to populate with movement
            if(gc.getFuelRCS() > 0)
            {
                if(Input.GetKey(KeyCode.LeftShift))
                {
                    //Translate X positive
                    rb.AddRelativeForce(Vector3.forward * impulseRCS);
                    gc.setRCSFuel(gc.getFuelRCS() - Time.deltaTime * .125f);
                } 
                if(Input.GetKey(KeyCode.LeftControl))
                {
                    //Translate X negative
                    rb.AddRelativeForce(Vector3.back * impulseRCS);
                    gc.setRCSFuel(gc.getFuelRCS() - Time.deltaTime * .125f);
                }
                if(Input.GetKey(KeyCode.R))
                {
                    //Translate Y positive
                    rb.AddRelativeForce(Vector3.up* impulseRCS);
                    gc.setRCSFuel(gc.getFuelRCS() - Time.deltaTime * .125f);
                }
                if(Input.GetKey(KeyCode.F))
                {
                    //Translate Y negative
                    rb.AddRelativeForce(Vector3.down * impulseRCS);
                    gc.setRCSFuel(gc.getFuelRCS() - Time.deltaTime * .125f);
                }
                if(Input.GetKey(KeyCode.X))
                {
                    //Translate Z positive
                    rb.AddRelativeForce(Vector3.right * impulseRCS);
                    gc.setRCSFuel(gc.getFuelRCS() - Time.deltaTime * .125f);
                }
                if(Input.GetKey(KeyCode.Z)) //Need to confirm
                {
                    //Translate Z negative
                    rb.AddRelativeForce(Vector3.left * impulseRCS);
                    gc.setRCSFuel(gc.getFuelRCS() - Time.deltaTime * .125f);
                }


                if(Input.GetKey(KeyCode.S))
                {
                    //Rotate X negative
                    rb.AddRelativeTorque(Vector3.left * impulseRCS);
                    gc.setRCSFuel(gc.getFuelRCS() - Time.deltaTime * .125f);
                }
                if(Input.GetKey(KeyCode.W))
                {
                    //Rotate X positive
                    rb.AddRelativeTorque(Vector3.right * impulseRCS);
                    gc.setRCSFuel(gc.getFuelRCS() - Time.deltaTime * .125f);
                }
                if(Input.GetKey(KeyCode.E))
                {
                    //Rotate Y positive
                    rb.AddRelativeTorque(Vector3.up * impulseRCS);
                    gc.setRCSFuel(gc.getFuelRCS() - Time.deltaTime * .125f);
                }
                if(Input.GetKey(KeyCode.Q))
                {
                    //Rotate Y negative
                    rb.AddRelativeTorque(Vector3.down * impulseRCS);
                    gc.setRCSFuel(gc.getFuelRCS() - Time.deltaTime * .125f);
                }
                if(Input.GetKey(KeyCode.A)) //Need to check for inversion
                {
                    //Rotate Z positive
                    rb.AddRelativeTorque(Vector3.forward * impulseRCS);
                    gc.setRCSFuel(gc.getFuelRCS() - Time.deltaTime * .125f);
                }
                if(Input.GetKey(KeyCode.D))
                {
                    //Rotate Z negative
                    rb.AddRelativeTorque(Vector3.back * impulseRCS);
                    gc.setRCSFuel(gc.getFuelRCS() - Time.deltaTime * .125f);

                }  
            }

        }
        throttle = Math.Clamp(throttle, 0, maxThrottle);

        if(Input.GetKeyDown(KeyCode.CapsLock)) //Flip flop for now, original hypothesis was incorrect
        {
            if(valRCS == true)
            {
                valRCS = false;
            } else {
                valRCS = true;
            }
        }
    }

    public int getThrottle()
    {
        return throttle;
    }

    public bool getRCS()
    {
        return valRCS;
    }


}
