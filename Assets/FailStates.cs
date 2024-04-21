using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FailStates : MonoBehaviour
{
    public Rigidbody rb;
    private Keyboard controlKF;
    private GeneralCalculations calc;
    [SerializeField]
    private TextMeshProUGUI CrashUI;
    public float fuel;
    public float alt;
    public float roll;
    public float pitch;
    public float yaw; 
    public AudioSource crashPipe;

    // Start is called before the first frame update
    void Start()
    {
        crashPipe = GetComponent<AudioSource>();
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


        if (alt >= 150000f) {
            Debug.Log("ship has reached bounds");
            rb.AddForce(0f, -5255000f*1.5f*((alt-150000f)/50000f), 0f);

        }
        if (alt <= 5f && rb.velocity.y <= 0){
            // gameover scene pause game
            if (Time.timeScale == 0) {
                Debug.Log("game not applicable");
            }
            else
            {
                CrashUI.text = "CRASHED";
                Time.timeScale = 0;
                crashPipe.Play();
            }
        }
        if (fuel <= 0) {
            Debug.Log("You are out of fuel");
            //stop forces
        }
        if (rb.transform.position.x >= 100000f || rb.transform.position.x <= -100000f || rb.transform.position.z >= 100000f || rb.transform.position.z <= -100000f) {
            Debug.Log("ship has reached bounds");
            rb.transform.position = new Vector3(0f, rb.transform.position.y, 0f);
        }

    }
}
