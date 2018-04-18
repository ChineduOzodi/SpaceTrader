using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Position storage with solar body referencing solarIndex as well as the tracking of 
/// galaxy position and solar position. There is also the ability to scale solar position to
/// numbers more that can be handle by Unity's floating point coordinate system.
/// </summary>
public class Position {
    //---------------Public Fields------------------------------------------//
    /// <summary>
    /// The position of the entity that is in galaxy coordinate system.
    /// </summary>
    public Vector3d StarPosition {
        get
        {
            if (referenceId == "")
            {
                return referencePosition;
            }

            return GameManager.instance.locations[referenceId].StarPosition;
        }

        }
    /// <summary>
    /// Position in relation to the center of the solar system in km.
    /// </summary>
    public Vector3d SystemPosition {
        get
        {
            if (referenceId == "")
            {
                return Vector3d.zero;
            }
            return GameManager.instance.locations[referenceId].SystemPosition + referencePosition;
            //if (solarIndex.Count == 1) //Entity orbiting star
            //{
                
            //}
            //else if (solarIndex.Count == 2) //Entity orbiting Planet
            //{
            //    return 1 / SystemConversion[2];
            //}
            //else if (solarIndex.Count == 3) //Entity orbiting Moon, close to a planet
            //{
            //    return 1 / SystemConversion[3];
            //}
            //else if (solarIndex.Count == 4) //Entity near another entity/space station
            //{
            //    return 1 / SystemConversion[4];
            //}
            //else if (solarIndex.Count == 5)
            //    return 1 / SystemConversion[4];
            //else
            //    throw new Exception("Local Scale is off, " + solarIndex.Count.ToString());
        }
        set
        {
            referencePosition = value - GameManager.instance.locations[referenceId].SystemPosition;
        }
    }
    /// <summary>
    /// Position in relationship to the refence solar body in solarIndex in km; This is the main location of solar position. If there is no reference, then the units are in ly.
    /// </summary>
    public Vector3d referencePosition { get; set; }
    /// <summary>
    /// The parenting id location.
    /// </summary>
    public string referenceId { get; set; }
    /// <summary>
    /// Assumes that the reference id is that of a solar body.
    /// </summary>
    public SolarBody ReferenceBody
    {
        get
        {
            if (referenceId == "") throw new System.Exception("No reference");
            if (referenceId == "Galaxy")
            {
                if (this.GetType() == typeof(SolarBody))
                {
                    return this as SolarBody;
                }
                else
                    throw new System.Exception("Referenced to Galaxy");
            }
            return ((SolarBody)GameManager.instance.locations[referenceId]);
        }
    }

    //----------------Needed variables to be set by the user----------------//
    public static int numSystems = 5;                                          //number of coordinate systems
    /// <summary>
    /// Conversion rates scaled to km.
    /// </summary>
    public static double[] SystemConversion = { 9.461e+12, 1000000, 1000, 10, .001 };
    public static string[] systemUnitNames = { "Lightyears", "Gm", "Mm" , "Hm", "m" };      //units of each coordinate system

    //----------------Private Variables--------------------------------//

    //----------------Public Fields----------------------------------//

    //------------------------------------------------------------------//
    /// <summary>
    /// Build the systemConversion field
    /// </summary>
    public Position() {   }
    public Position(string _refernceId)
    {
        referenceId = _refernceId;
        referencePosition = Vector3d.zero;
    }
    public Position(string _referenceId, Vector3d _localPosition)
    {
        referenceId = _referenceId;
        referencePosition = _localPosition;
    }

    //-------------------------Position Funtions----------------------//
    /// <summary>
    /// Used by the orbit Function to make sure planetary orbits are in correct game scale
    /// </summary>
    public static double KmToLocalScale(int index)
    {
        return 1 / SystemConversion[index];
    }

    //----------------------------------------------//

    /// <summary>
    /// Used to determine the real position of object at given index level taking into account the lower index levels
    /// </summary>
    /// <param name="systemIndex"></param>
    /// <returns></returns>
    public Vector3d SystemToGalaxyPosition()
    {
        if (referenceId == "")
        {
            return referencePosition;
        }
        return StarPosition + SystemPosition / SystemConversion[0];
    }

    //-----------------------Addition----------------------------------//
    ///// <summary>
    ///// Adds to coordinate lists
    ///// </summary>
    ///// <param name="pos1"></param>
    ///// <param name="pos2"></param>
    ///// <returns></returns>
    //public static List<Vector3> Add(List<Vector3> pos1, List<Vector3> pos2)
    //{

    //    for (int i = 0; i < numSystems; i++)
    //    {
    //        pos1 = Add(pos1, pos2[i], i);
    //    }

    //    return pos1;
    //}
    /// <summary>
    /// Adds a Vector3 to the correct position in coordinate list based on the unit index
    /// </summary>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    /// <param name="unitIndex"></param>
    /// <returns></returns>
    public static List<Vector3> Add(List<Vector3> pos1, Vector3 pos2, int unitIndex)
    {
        if (unitIndex < 0 || unitIndex >= pos1.Count)
        {
            throw new System.Exception("Invalid unitIndex " + unitIndex + ", needs to be between 0 and " + pos1.Count);
        }

        if (unitIndex == 0)
        {
            pos1[0] += pos2;
            return pos1;
        }

        pos1[unitIndex] += pos2;
        return pos1;
    }
    /// <summary>
    /// Adds a Vector3 to the correct position in coordinate list based on the unit name
    /// </summary>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
    public static List<Vector3> Add(List<Vector3> pos1, Vector3 pos2, string unit)
    {
        for (int i = 0; i < pos1.Count; i++)
        {
            if (systemUnitNames[i] == unit)
            {
                return Add(pos1, pos2, i);
            }
        }
        throw new System.Exception("Could not find unit " + unit + "in systemUnitNames");
    }


    //-------------------------Subtraction--------------------------//
    ///// <summary>
    ///// Subtracts 2 coordinate lists
    ///// </summary>
    ///// <param name="pos1"></param>
    ///// <param name="pos2"></param>
    ///// <returns></returns>
    //public static List<Vector3> Subtract(List<Vector3> pos1, List<Vector3> pos2)
    //{
    //    for (int i = 0; i < numSystems; i++)
    //    {
    //        pos1 = Subtract(pos1, pos2[i], i);
    //    }

    //    return pos1;
    //}
    /// <summary>
    /// Subtracts a Vector3 from the correct position in coordinate list based on the unit index
    /// </summary>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    /// <param name="unitIndex"></param>
    /// <returns></returns>
    public static List<Vector3> Subtract(List<Vector3> pos1, Vector3 pos2, int unitIndex)
    {
        if (unitIndex < 0 || unitIndex >= pos1.Count)
        {
            throw new System.Exception("Invalid unitIndex " + unitIndex + ", needs to be between 0 and " + pos1.Count);
        }

        if (unitIndex == 0)
        {
            pos1[0] -= pos2;
            return pos1;
        }

        pos1[unitIndex] -= pos2;
        return pos1;
    }

    //------------------------Helper Functions-----------------------------------//
}
