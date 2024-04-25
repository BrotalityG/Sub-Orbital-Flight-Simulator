using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.Rendering;

public class Skybox_AtmosphereAdj : MonoBehaviour
{
    //Constants 
    private static float Atmos_Thick_Start = 0.65f;
    private static float Space_Altitude = 100000f;
    private static float Transition_Distance = 90000f;

    public GeneralCalculations cs;
    // Start is called before the first frame update
    void Start()
    {
         RenderSettings.skybox.SetFloat("_AtmosphereThickness", Atmos_Thick_Start);
    }

    // Update is called once per frame
    void Update()
    {
        AtmosphereUpdater();
    }

    private void AtmosphereUpdater ()
    {
        if (cs.getAlt() > 10000f && cs.getAlt() < Space_Altitude){

            RenderSettings.skybox.SetFloat("_AtmosphereThickness", Atmos_Thick_Start*((Space_Altitude-cs.getAlt())/Transition_Distance));

        }else if (cs.getAlt() >= Space_Altitude){

            RenderSettings.skybox.SetFloat("_AtmosphereThickness", 0.0f);

        }else {

            RenderSettings.skybox.SetFloat("_AtmosphereThickness", Atmos_Thick_Start);
            
        }
    }
}