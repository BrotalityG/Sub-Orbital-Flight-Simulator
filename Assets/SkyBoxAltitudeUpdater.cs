using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.Rendering;

public class Skybox_AtmosphereAdj : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
         RenderSettings.skybox.SetFloat("_AtmosphereThickness", 0.65f);
    }

    // Update is called once per frame
    void Update()
    {
        //Until the sky limit issue is fixed: in luie to the 70km to 100km change in atmosphere thickness, this will use
        // 7km to 10km.
        if (gameObject.transform.position.y > 7000f && gameObject.transform.position.y < 10000f){

            RenderSettings.skybox.SetFloat("_AtmosphereThickness", .65f*((3000f-(gameObject.transform.position.y - 7000f))/3000f));

        }else if (gameObject.transform.position.y >= 10000f){

            RenderSettings.skybox.SetFloat("_AtmosphereThickness", 0.0f);

        }
    }
}
