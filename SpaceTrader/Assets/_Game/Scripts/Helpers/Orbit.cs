using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Orbit {

    //public static double massConversion = Mathd.Pow(10, 24);
    //public static double distanceConversion = Units.G * 100; // Multiplication factor to used to view orbits. Base unit will be meters

    public double sma { get; set; } // Semi-major axis.
    public double ecc { get; set; }
    public double inc { get; set; } // Not used
    public double lpe { get; set; } // Longitude of periapsis. Angle in radians
    public double lan { get; set; } // Longitude of ascending node. Not used.
    public double mna { get; set; } // Mean anomaly at epoch. Between -2pi and 2pi.
    public double eph { get; set; } // Epoch, a reference time.

    /// <summary>
    /// Creates the orbit of an object given the correct parameters
    /// </summary>
    /// <param name="SMA">Semi-major axis.</param>
    /// <param name="ECC"></param>
    /// <param name="MNA">Mean anomaly at epoch. Between -2pi and 2pi.</param>
    /// <param name="EPH">Epoch, a reference time.</param>
    /// <param name="LPE">Longitude of periapsis. Angle in radians</param>
    public Orbit(double SMA, double ECC, double MNA, double EPH, double LPE)
    {
        sma = SMA;
        ecc = ECC;
        mna = MNA;
        eph = EPH;
        lpe = LPE;

        inc = 0;
        lan = 0;
    }    

    public string GetOrbitalInfo()
    {
        return string.Format("Semi-major Axis:{0}\nEcc: {1}\nMean Anomaly at Epoch: {2}\nEpoch: {3}\nLong of Periapsis: {4}",
        Units.ReadDistance(sma),
        ecc, // 1
        mna,
        eph,
        lpe // 4
        );
    }
    
}
