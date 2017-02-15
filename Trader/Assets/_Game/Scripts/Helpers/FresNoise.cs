using UnityEngine;
using System.Collections;
using System;

[Serializable]
public static class FresNoise
{
    //public int pixHeight;
    //public float xOrg;
    //public float yOrg;
    //public float scale = 1.0F;
    /// <summary>
    /// Return the displacement of from 0
    /// </summary>
    /// <param name="seed"></param>
    /// <param name="polar">polar coords with radius and angle in radians</param>
    /// <returns></returns>
    public static float GetTerrian(string seed, Polar2 polar)
    {
        float fDetial = 1;
        float fWeight = 10000f;
        float fValue = 0;
        int nOctaves = 10;
        float scale = .00005f;
        int disp = seed.Length;
        polar.radius*= scale;

        Vector2 coord = polar.cartesian;
        for (int i = 0; i < nOctaves; i++)
        {
            fValue += (Mathf.PerlinNoise(((float)coord.x + disp) * fDetial, ((float) coord.y + disp) * fDetial) -.5f) * fWeight ; //Sum wegihted noise value
            fWeight *= .3f; //adjust weight
            fDetial *= 3;
        }
        return fValue;
    }

    public static float[,] CalcNoise(int pixWidth, int pixHeight, float xOrg, float yOrg, float scale = 1f)
    {
        float[,] map = new float[pixWidth, pixHeight];
        int y = 0;
        while (y < pixHeight)
        {
            int x = 0;
            while (x < pixWidth)
            {
                float xCoord = xOrg + x / pixWidth * scale;
                float yCoord = yOrg + y / pixHeight * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                map[x, y] = sample;
                x++;
            }
            y++;
        }

        return map;
    }

    public static float[,] CalcNoise(int pixWidth, int pixHeight, string seed = null, float scale = 10f)
    {
        if (seed == null)
        {
            seed = Time.time.ToString();
        }
        System.Random randNum = new System.Random(seed.GetHashCode());

        float xOrg = randNum.Next(pixHeight);
        float yOrg = xOrg;

        float[,] map = new float[pixWidth, pixHeight];
        int y = 0;
        while (y < pixHeight)
        {
            int x = 0;
            while (x < pixWidth)
            {
                float xCoord = xOrg + (float)x / (float)pixWidth * scale;
                float yCoord = yOrg + (float)y / (float)pixHeight * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                map[x, y] = sample;
                x++;
            }
            y++;
        }

        return map;
    }

    public static int[,] CalcNoise(int pixWidth, int pixHeight,float[] heightMap, string seed = null, float scale = 10f)
    {
        if (seed == null)
        {
            seed = Time.time.ToString();
        }
        System.Random randNum = new System.Random(seed.GetHashCode());


        float xOrg = randNum.Next(pixHeight);
        float yOrg = xOrg;

        int[,] map = new int[pixWidth, pixHeight];
        float[,] floatMap = new float[pixWidth, pixHeight];
        int y = 0;
        while (y < pixHeight)
        {
            int x = 0;
            while (x < pixWidth)
            {
                float xCoord = xOrg + (float)x / (float)pixWidth * scale;
                float yCoord = yOrg + (float)y / (float)pixHeight * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                floatMap[x, y] = sample;
                map[x, y] = ScaleFloatToInt(sample,heightMap);
                x++;
            }
            y++;
        }

        return map;

    }

    public static int ScaleFloatToInt(float sample, float[] heightMap)
    {
        int num = heightMap.Length;

        for (int i = 0; i < heightMap.Length; i++)
        {
            if (heightMap[i] >= sample)
            {
                num = i;
                break;
            }
        }

        return num;
    }

}
