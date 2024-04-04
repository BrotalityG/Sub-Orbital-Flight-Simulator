using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class Keyboard : MonoBehaviour {
        private int RCSRemaining = 500; //Just a placeholder for Sprint 2. Will update as implemented
        [SerializeField]
        private int throttleIncrement = 1;
        private int maxThrottle = 100;
        [SerializeField]
        private int throttle;
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
        private float Responsiveness = 100000f;
        private float MaximumThrottle = 5255000f; // According to European Space Agency: https://www.esa.int/Science_Exploration/Human_and_Robotic_Exploration/Space_Shuttle/Shuttle_technical_facts
        private Rigidbody rb;
        private GeneralCalculations gc;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>(); // Get the rigidbody so we don't need to manually assign it.
            gc = GetComponent<GeneralCalculations>(); // Get the GeneralCalculations script so we don't need to manually assign it.
        }

        // Update is called once per frame
        void Update()
        {
            gc.setFuel(gc.getFuel() - throttle/ maxThrottle * Time.deltaTime * .1f); // Subtract fuel based on throttle
            HandleInputs();
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
            rb.AddRelativeTorque(Vector3.forward * roll * Mathf.Clamp((IAS-25)/100, 0, 1) * (gc.getDensity()/1.225f) * Responsiveness);
        }

        private void applyPitch() {
            rb.AddRelativeTorque(Vector3.right * -pitch * Mathf.Clamp((IAS-25)/100, 0, 1) * (gc.getDensity()/1.225f) * Responsiveness);
        }

        private void applyYaw() {
            rb.AddRelativeTorque(Vector3.up * yaw * Mathf.Clamp((IAS-25)/100, 0, 1) * (gc.getDensity()/1.225f) * Responsiveness);
        }

        private void applyThrottle() {
            rb.AddRelativeForce(Vector3.forward * (((float) throttle) / ((float) maxThrottle)) * MaximumThrottle);
        }

        private void HandleInputs() {
            roll = Input.GetAxis("Roll");
            pitch = Input.GetAxis("Pitch");
            yaw = Input.GetAxis("Yaw");

            if (Input.GetKey(KeyCode.LeftShift)) throttle += throttleIncrement;
            else if (Input.GetKey(KeyCode.LeftControl)) throttle -= throttleIncrement;
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
