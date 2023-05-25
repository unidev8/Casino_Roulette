using System;

[Serializable]
public class Chip
{
    public string name { get; set; }
    //public string parentName { get; set; }
    public string buttonName { get; set; } // 01 to 36 and Outside buttons ** should be 2 digit
    public byte positionInButton { get; set; } // 1 to 8 ** Should be a digit
    public float chipValue { get; set; } // 0.1 to 100f
    public byte historyIndex { get; set; } // 0 ~sequence of instantiate;	
}
