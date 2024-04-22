//This program is to keep the artificial horizion component (Modeled by Charles Walker) operating properly
using UnityEngine;

public class AHS_CTL : MonoBehaviour
{
    public Transform AH; //AH is shorthand for Artificial Horizion 
    public Rigidbody Shuttle;
    // Start is called before the first frame update
    void Start()
    {
        AH = GetComponent<Transform>();
        Shuttle = AH.GetComponentInParent<Rigidbody>();
    }

    private float GetDirection(float dot) {
        float SpeedDirection = 0;

        switch (dot) {
            case float n when n > 0:
                SpeedDirection = 1;
                break;
            case float n when n < 0:
                SpeedDirection = -1;
                break;
            case float n when n == 0:
                SpeedDirection = 0;
                break;
        }

        return SpeedDirection;
    }

    // Update is called once per frame
    void Update()
    {
        AH.transform.localEulerAngles = new Vector3 (Shuttle.transform.eulerAngles.x, 90, -Shuttle.transform.eulerAngles.z);
    }
}
