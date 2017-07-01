using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit {

    public static double massConversion = Mathd.Pow(10, 24);
    public static double distanceConversion = Units.Gm * 100; // Multiplication factor to used to view orbits. Base unit will be meters

    public double G = 6.67408 * Mathd.Pow(10, -11); // Gravitational constant. TODO: move to a better more localized location.

    public double Mass { get; set; }
    public double OrbitalPeriod { get { return 2 * Mathf.PI / mm; } }
    public double soi { get { return sma * Mathd.Pow(Mass / parentMass, .4f); } }
    public double bodyRadius { get; set; }

    public double sma { get; set; } // Semi-major axis.
    public double ecc { get; set; }
    public double inc { get; set; }
    public double lpe { get; set; } // Longitude of periapsis. Angle in radians
    public double lan { get; set; } // Longitude of ascending node. Not needed.
    public double mna { get; set; } // Mean anomaly at epoch. Between -2pi and 2pi.
    public double eph { get; set; } // Epoch, a reference time.
    public double parentMass { get; set; }
    private SolarType type;

    private double mm // Mean motion of orbit in radians per second.
    {
        get
        {
            return Mathd.Sqrt((G * parentMass) / (Mathd.Pow(sma, 3)));
        }
    }

    public Orbit() { }

    /// <summary>
    /// Creates the orbit of an object given the correct parameters
    /// </summary>
    /// <param name="Mass"></param>
    /// <param name="SMA">Semi-major axis.</param>
    /// <param name="ECC"></param>
    /// <param name="MNA">Mean anomaly at epoch. Between -2pi and 2pi.</param>
    /// <param name="EPH">Epoch, a reference time.</param>
    /// <param name="LPE">Longitude of periapsis. Angle in radians</param>
    /// <param name="Parent">Parent Orbit</param>
    public Orbit(double Mass, double SMA, double ECC, double MNA, double EPH, double LPE, double parentMass)
    {
        this.Mass = Mass;
        sma = SMA;
        ecc = ECC;
        mna = MNA;
        eph = EPH;
        lpe = LPE;
        this.parentMass = parentMass;
    }

    public Orbit(double Mass)
    {
        this.Mass = Mass;
    }
   



    public Vector3 Radius(double time)
    {
        if (sma == 0)
        {
            return Vector3.zero;
        }
        var ena = ENA(time);
        var pos = new Vector3d(sma * (Mathd.Cos(ena) - ecc), sma * Mathd.Sqrt(1 - (ecc * ecc)) * Mathd.Sin(ena));
        var pol = new Polar2d(pos);
        pol.angle += lpe;
        return (Vector2) (pol.cartesian / distanceConversion);
        
    }

    /// <summary>
    /// Eccentric anomaly at a future time given the current eccentric anomaly
    /// </summary>
    /// <returns></returns>
    private double ENA(double time)
    {
        double M = mna + (mm * (time - eph));
        double E = M;
        for (int i = 0; i < 10; i++) // How many times it iterates to solve the equation.
        {
            E = M + ecc * Mathd.Sin(E);
        }
        return E;
    }

    public string GetOrbitalInfo(double time)
    {
        return string.Format("Mass: {0}\nOrbital Period; {1}\nSemi-major Axis:{2}\nEcc: {3}\nMean Anomaly at Epoch: {4}\nEpoch: {5}\nEccentric Anomaly: {6}\nLong of Periapsis: {7}",
        Mass,
        Dated.ReadTime(OrbitalPeriod),
        Units.ReadDistance(sma),
        ecc, // 3
        mna,
        eph,
        ENA(time),
        lpe // 7
        );
    }
    
}
