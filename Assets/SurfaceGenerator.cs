using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SurfaceGenerator : MonoBehaviour
{
    [SerializeField]
    public Rigidbody rb;

    [SerializeField]
    public GameObject surface;



    private void Start()
    {
        surface.transform.position = new Vector3(rb.transform.position.x, -100000, rb.transform.position.z);

    }

    private void Update()
    {
        surface.transform.position = new Vector3(rb.transform.position.x, -100000, rb.transform.position.z);
    }
}
    