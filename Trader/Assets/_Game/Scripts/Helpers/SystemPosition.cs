using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// An alternate position storage system to that allows for multilayered navigation to increase 
/// the range past the bounds of float limitations
/// </summary>
public class SystemPosition {

    //----------------Needed variables to be set by the user----------------//
    private static int numSystems = 3;                                          //number of coordinate systems
    private static float[] systemBounds = { 10, 10 };                      //Bounds on the lower system(s) in which the units will be converted to the upper system
    private static string[] systemUnitNames = { "Lightyear", "AU", "Km" };      //units of each coordinate system

    //----------------Private Variables--------------------------------//
    private Vector3[] coordinateList;

    //----------------Public Fields----------------------------------//
    public Vector3[] location
    {
        get { return coordinateList; }
        set { coordinateList = Clamp(value); }
    }
    //------------------------------------------------------------------//
    public SystemPosition() { }
    public SystemPosition(Vector3[] _location)
    {
        location = _location;
    }
    //----------------------------------------------//
    public Vector3 ToWorldPosition(string unit)
    {
        for (int i = 0; i < numSystems; i++)
        {
            if (systemUnitNames[i] == unit)
            {
                return ToWorldPosition(i);
            }
        }
        throw new System.Exception("Could not find unit " + unit + "in systemUnitNames");
    }

    public Vector3 ToWorldPosition(int systemIndex)
    {
        if (systemIndex < 0 || systemIndex >= numSystems)
        {
            throw new System.Exception("Invalid unitIndex " + systemIndex + ", needs to be between 0 and " + numSystems);
        }
        Vector3 position = location[systemIndex];
        float multiplier = 1;
        for (int i = systemIndex + 1; i < numSystems; i++)
        {
            multiplier *= systemBounds[i - 1];
            position += location[i] / multiplier; ;
        }

        return position;
    }

    public void ToSystemPosition(Vector3 position, int unitIndex)
    {
        location = Add(location, position, unitIndex);
    }

    //-----------------------Addition----------------------------------//
    /// <summary>
    /// Adds to coordinate lists
    /// </summary>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    /// <returns></returns>
    public static Vector3[] Add(Vector3[] pos1, Vector3[] pos2)
    {
        for (int i = 0; i < numSystems; i++)
        {
            pos1 = Add(pos1, pos2[i], i);
        }

        return pos1;
    }
    /// <summary>
    /// Adds a Vector3 to the correct position in coordinate list based on the unit index
    /// </summary>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    /// <param name="unitIndex"></param>
    /// <returns></returns>
    public static Vector3[] Add(Vector3[] pos1, Vector3 pos2, int unitIndex)
    {
        if (unitIndex < 0 || unitIndex >= numSystems)
        {
            throw new System.Exception("Invalid unitIndex " + unitIndex + ", needs to be between 0 and " + numSystems);
        }

        if (unitIndex == 0)
        {
            pos1[0] += pos2;
            return pos1;
        }

        pos1[unitIndex] += pos2;
        pos1 = Clamp(pos1);
        return pos1;
    }
    /// <summary>
    /// Adds a Vector3 to the correct position in coordinate list based on the unit name
    /// </summary>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
    public static Vector3[] Add(Vector3[] pos1, Vector3 pos2, string unit)
    {
        for (int i = 0; i < numSystems; i++)
        {
            if (systemUnitNames[i] == unit)
            {
                return Add(pos1, pos2, i);
            }
        }
        throw new System.Exception("Could not find unit " + unit + "in systemUnitNames");
    }


    //-------------------------Subtraction--------------------------//
    /// <summary>
    /// Subtracts 2 coordinate lists
    /// </summary>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    /// <returns></returns>
    public static Vector3[] Subtract(Vector3[] pos1, Vector3[] pos2)
    {
        for (int i = 0; i < numSystems; i++)
        {
            pos1 = Subtract(pos1, pos2[i], i);
        }

        return pos1;
    }
    /// <summary>
    /// Subtracts a Vector3 from the correct position in coordinate list based on the unit index
    /// </summary>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    /// <param name="unitIndex"></param>
    /// <returns></returns>
    public static Vector3[] Subtract(Vector3[] pos1, Vector3 pos2, int unitIndex)
    {
        if (unitIndex < 0 || unitIndex >= numSystems)
        {
            throw new System.Exception("Invalid unitIndex " + unitIndex + ", needs to be between 0 and " + numSystems);
        }

        if (unitIndex == 0)
        {
            pos1[0] -= pos2;
            return pos1;
        }

        pos1[unitIndex] -= pos2;
        pos1 = Clamp(pos1);
        return pos1;
    }
    //------------------------Helper Functions-----------------------------------//
    /// <summary>
    /// Makes sure each system coordinate is within the bounds specified by the systemBounds variable
    /// </summary>
    /// <param name="pos">the coordinate list to clamp</param>
    /// <returns>A clamped version of the coordinate list</returns>
    public static Vector3[] Clamp(Vector3[] pos)
    {
        if (pos.Length != numSystems)
        {
            throw new System.Exception("Position is not the correct length: " + numSystems);
        }
        for (int i = 0; i < numSystems-1; i++) //Removes decimals from all but the lowest coordinate system
        {
            for (int b = 0; b < 3; b++) //Checks each index in Vector3
            {
                float ints = Mathf.Floor(pos[i][b]);
                float decimals = pos[i][b] - ints;
                if (decimals != 0)
                {
                    decimals *= systemBounds[i];
                    pos[i + 1][b] += decimals;
                    pos[i][b] = ints;
                }
            }
        }
        for (int i = numSystems - 1; i > 0; i--)
        {
            for (int b = 0; b < 3; b++) //Checks each index in Vector3
            {
                while (pos[i][b] > systemBounds[i-1])
                {
                    pos[i][b] -= systemBounds[i-1];
                    pos[i - 1][b] += 1;
                }
                while (pos[i][b] < -systemBounds[i-1])
                {
                    pos[i][b] += systemBounds[i-1];
                    pos[i - 1][b] -= 1;
                }
            }
        }
        return pos;
    }

    public Vector3 this[int index]
    {
        get
        {
            return ToWorldPosition(index);
        }
        set
        {
            location[index] = value;
        }
    }
}
