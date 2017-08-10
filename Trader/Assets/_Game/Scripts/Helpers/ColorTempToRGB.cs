using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ColorTempToRGB {

    public static Color TempToRGB(float kelvin)
    {
        float temp = kelvin / 100;

        float red;
        float green;
        float blue;

        if (temp <= 66)
        {

            red = 255;

            green = temp;
            green = 99.4708025861f * Mathf.Log(green) - 161.1195681661f;


            if (temp <= 19)
            {

                blue = 0;

            }
            else
            {

                blue = temp - 10;
                blue = 138.5177312231f * Mathf.Log(blue) - 305.0447927307f;

            }

        }
        else
        {

            red = temp - 60;
            red = 329.698727446f * Mathf.Pow(red, -0.1332047592f);

            green = temp - 60;
            green = 288.1221695283f * Mathf.Pow(green, -0.0755148492f);

            blue = 255;

        }


        return new Color(red / 255, green / 255, blue / 255);
    } 
}
