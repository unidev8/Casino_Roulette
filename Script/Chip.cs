namespace CED_Roulette
{
    using System;

    [Serializable]
    public class Chip
    {
        public string name { get; set; } // ex Chip_09_4, RoundChip_13_0
                                         //public string parentName { get; set; }
        public string buttonName { get; set; } // 01 to 36 and Outside buttons : 09, RoundGroup_1, 
        public byte positionInButton { get; set; } // 1 to 8 ** Should be a digit
        public float chipValue { get; set; } // 0.1 to 100f
        public int historyIndex { get; set; } // 0 ~ sequence index of instans;	
    }

}
