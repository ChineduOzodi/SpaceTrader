static public class EditorVersion {
    
    private static UnityVer _info;

    public static int main { get { return info.main; } }
    public static int major { get { return info.major; } }
    public static int minor { get { return info.minor; } }


    public static bool IsUnity3 { get { return main == 3; }}
    public static bool IsUnity4 { get { return main == 4; }}
    public static bool IsUnity5 { get { return main == 5; }}

    static UnityVer ToUnityVer(this string str) {
        var arr = str.xSplit(".");
        var l = arr.Length;

        return new UnityVer(
            l > 0 ? int.Parse(arr[0]) : 0,
            l > 1 ? int.Parse(arr[1]) : 0,
            l > 2 ? int.Parse(arr[2]) : 0
        );
    }

    internal static UnityVer info {
        get {
            if (_info.main > 0) return _info;
            _info = new UnityVer(

#if   UNITY_3_4_0	//  UNITY 3
            3,4,0      
#elif UNITY_3_4_1	
            3,4,1      
#elif UNITY_3_4_2	
            3,4,2
      
#elif UNITY_3_5_0	
            3,5,0      
#elif UNITY_3_5_1	
            3,5,1      
#elif UNITY_3_5_2	
            3,5,2      
#elif UNITY_3_5_3	
            3,5,3      
#elif UNITY_3_5_4	
            3,5,4      
#elif UNITY_3_5_5	
            3,5,5      
#elif UNITY_3_5_6	
            3,5,6     
#elif UNITY_3_5_7	
            3,5,7


#elif UNITY_4_0_0	//  UNITY 4
            4,0,0      
#elif UNITY_4_0_1
            4,0,1
      
#elif UNITY_4_1_0	
            4,1,0      
#elif UNITY_4_1_2	
            4,1,2      
#elif UNITY_4_1_3	
            4,1,3      
#elif UNITY_4_1_4	
            4,1,4      
#elif UNITY_4_1_5	
            4,1,5
     
#elif UNITY_4_2_0	
            4,2,0    
#elif UNITY_4_2_1	
            4,2,1   
#elif UNITY_4_2_2	
            4,2,2
      
#elif UNITY_4_3_0	
            4,3,0  
#elif UNITY_4_3_1	
            4,3,1    
#elif UNITY_4_3_2	
            4,3,2      
#elif UNITY_4_3_3	
            4,3,3      
#elif UNITY_4_3_4	
            4,3,4

#elif UNITY_4_5_0	
            4,5,0     
#elif UNITY_4_5_1	
            4,5,1 
#elif UNITY_4_5_2	
            4,5,2      
#elif UNITY_4_5_3	
            4,5,3      
#elif UNITY_4_5_4	
            4,5,4
#elif UNITY_4_5_5	
            4,5,5

#elif UNITY_4_6_0	//FORECAST FOR UNITY_4_6
            4,6,0      
#elif UNITY_4_6_1	
            4,6,1
#elif UNITY_4_6_2	
            4,6,2      
#elif UNITY_4_6_3	
            4,6,3      
#elif UNITY_4_6_4	
            4,6,4
#elif UNITY_4_6_5	
            4,6,5


#elif UNITY_5_0_0	//FORECAST FOR UNITY_5
            5,0,0   
#elif UNITY_5_0_1	
            5,0,1   
#elif UNITY_5_0_2	
            5,0,2      
#elif UNITY_5_0_3	
            5,0,3      
#elif UNITY_5_0_4	
            5,0,4
#elif UNITY_5_0_5	
            5,0,5

#elif UNITY_5_1_0	
            5,1,0   
#elif UNITY_5_1_1	
            5,1,1   
#elif UNITY_5_1_2	
            5,1,2      
#elif UNITY_5_1_3	
            5,1,3      
#elif UNITY_5_1_4	
            5,1,4
#elif UNITY_5_1_5	
            5,1,5

#elif UNITY_5_2_0	
            5,2,0   
#elif UNITY_5_2_1	
            5,2,1   
#elif UNITY_5_2_2	
            5,2,2      
#elif UNITY_5_2_3	
            5,2,3      
#elif UNITY_5_2_4	
            5,2,4
#elif UNITY_5_2_5	
            5,2,5

#elif UNITY_5_3_0	
            5,3,0   
#elif UNITY_5_3_1	
            5,3,1   
#elif UNITY_5_3_2	
            5,3,2      
#elif UNITY_5_3_3	
            5,3,3      
#elif UNITY_5_3_4	
            5,3,4
#elif UNITY_5_3_5	
            5,3,5

#else
            0,0,0
#endif                
            );

            return _info;
        }
    }
}

public struct UnityVer {
    public int main;
    public int major;
    public int minor;

    public UnityVer(int main, int major, int minor) {
        this.main = main;
        this.major = major;
        this.minor = minor;
    }

    public override bool Equals(object obj) {
        var ver = (UnityVer) obj;
        return (ver.main == main) && (ver.major==major) && (ver.minor == minor);
    }

    public override int GetHashCode() { return base.GetHashCode(); }


    public static explicit operator int(UnityVer v) {
        return v.main * 1000 + v.major * 100 + v.minor;
    }

    public static bool operator ==(UnityVer a, UnityVer b) {
        return (a.main == b.main) && (a.major == b.major) && (a.minor == b.minor);
    }

    public static bool operator !=(UnityVer a, UnityVer b) {
        return (a.main != b.main) || (a.major != b.major) || (a.minor != b.minor);
    }

    public static bool operator <(UnityVer a, UnityVer b) {
        if (a.main < b.main) return true;
        if (a.main > b.main) return false;

        if (a.major < b.major) return true;
        if (a.major > b.major) return false;
        
        return a.minor < b.minor;
    }

    public static bool operator >(UnityVer a, UnityVer b) { return b < a; }
    public static bool operator <=(UnityVer a, UnityVer b) { return (a == b) || a < b; }
    public static bool operator >=(UnityVer a, UnityVer b) { return (a == b) || a > b; }
}


/*public enum UnityVer {
    UNKNOWN,

    UNITY_3_0,
    UNITY_3_1,
    UNITY_3_2,
    UNITY_3_3,
    UNITY_3_4,
    UNITY_3_5,

    UNITY_4_0,
    UNITY_4_1,
    UNITY_4_2,
    UNITY_4_3,
    UNITY_4_5,
    UNITY_4_6,

    UNITY_5_0
}*/