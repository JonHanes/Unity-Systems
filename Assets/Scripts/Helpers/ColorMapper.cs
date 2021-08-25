namespace SystemExample.Helpers {
    public static class ColorMapper {
       
        struct ColorTag {

            public ColorTag(string code, int min, int max) {
                Code = code;
                MinRange = min;
                MaxRange = max;
            }
            public string Code;
            public int MinRange;
            public int MaxRange;
        }

        static ColorTag[] PercentageColors = new ColorTag[] {
            new ColorTag("#595959", 0, 1),
            new ColorTag("#cc3300", 1, 25),
            new ColorTag("#ff9933", 25, 50),
            new ColorTag("#ace600", 50, 75),
            new ColorTag("#00b33c", 75, int.MaxValue)
        };

        static string[] InventoryCheckColors = new string[] { "#009933", "#990033"};
        
        public static string GetColorBasedOnPerc(int perc) {
            foreach (var t in PercentageColors) {
                if (perc >= t.MinRange && perc < t.MaxRange) {
                    return t.Code;
                }
            }

            return "#00b33c";
        }

        public static string GetInventoryCheckColor(bool hasEnough) {
            return hasEnough ? InventoryCheckColors[1] : InventoryCheckColors[0];
        }
    }
}