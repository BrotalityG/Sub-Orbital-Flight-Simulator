using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skybox_DayNightToggle : MonoBehaviour
{
    // Start is called before the first frame update
    bool day = true;
    Color DaySky = new Color(38f/255f, 134f/255f, 164f/255f, 255f/255f);
    Color NightSky = new Color(0f/255f, 0f/255f, 0f/255f, 255f/255f);
    Color DayGround = new Color(82f/255f, 130f/255f, 58f/255f, 255f/255f);
    Color NightGround = new Color(35f/255f, 55f/255f, 24f/255f, 255f/255f);
    // Start is called before the first frame update
    void Start()
    {
        day = true;
        RenderSettings.skybox.SetColor("_SkyTint", DaySky);
        RenderSettings.skybox.SetColor("_GroundColor", DayGround);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)){
            if (day){
                RenderSettings.skybox.SetColor("_SkyTint", NightSky);
                RenderSettings.skybox.SetColor("_GroundColor", NightGround);
                day = false;
            }else {
                RenderSettings.skybox.SetColor("_SkyTint", DaySky);
                RenderSettings.skybox.SetColor("_GroundColor", DayGround);
                day = true;
            }
        }
    }
}
