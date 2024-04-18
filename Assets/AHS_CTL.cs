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

    // Update is called once per frame
    void Update()
    {
        AH.transform.eulerAngles = new Vector3 (2*Shuttle.transform.eulerAngles.x, 0, -Shuttle.transform.eulerAngles.z);
    }
}
