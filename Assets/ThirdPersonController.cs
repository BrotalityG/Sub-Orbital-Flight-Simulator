using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    [Tooltip("An array of transforms representing camera positions")]
    [SerializeField] Transform[] povs;
    [Tooltip("The speed at which the camera will follow the plane")]
    [SerializeField] float speed;

    private int index = 0;
    private Vector3 target;

    private void Update() {

        if (Input.GetKeyDown(KeyCode.Alpha1)) index = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2)) index = 1;
        target = povs[index].position; // this chooses the position
    }

    private void FixedUpdate() {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime); //camera follows the part modified by the serialized speed that is provided
        transform.forward = povs[index].forward; // this chooses the POV
    }



}
