using Xamarin.Forms;

namespace Tojeero.Core
{
    public static class Colors
    {
        public static Color DarkBlack = Color.FromHex("#29353a");
        public static Color Black = Color.FromHex("#38474f");
        public static Color DarkText = Color.FromHex("#323c3f");

        public static Color DarkGray = Color.FromHex("#a6a6ad");
        public static Color Gray = Color.FromHex("#eaebeb");
        public static Color LightGray = Color.FromHex("#e8e8e8");
        public static Color UltraLightGray = Color.FromHex("#f8f8f8");

        public static Color Orange = Color.FromHex("#f6846a");
        public static Color Blue = Color.FromHex("#67b0d6");
        public static Color Green = Color.FromHex("#88cb56");
        public static Color White = Color.White;

        public static Color CellBackground = Color.White;
        public static Color LargeCellBackground = Color.FromHex("#f2f4f4");
        public static Color CellBorder = Color.FromHex("#dbdfdf");
        public static Color Border = Color.FromHex("#c9cecf");
        public static Color LightBackground = Color.FromHex("#eaeeef");
        public static Color Separator = Color.FromHex("#e6e8e9");

        public static Color Main = Color.FromHex("#2a414e");
        public static Color MainDark = Color.FromHex("#223743");
        public static Color Secondary = Color.FromHex("#f15a24");
        public static Color Subtitle = Color.FromHex("#9e9e9e");
        public static Color Placeholder = Color.FromHex("#dde4e6");
        public static Color ImagePlaceholder = Color.White;
        public static Color PlaceholderDark = Color.FromHex("#afb8bb");
        public static Color Invalid = Color.Red;
        public static Color Warning = Color.FromHex("#f6846a");
        public static Color SideMenuText = Color.FromHex("#96a9b4");

        public static Color ChatMessageSenderBackgroundColor = Placeholder;
        public static Color ChatMessageReceiverBackgroundColor = Secondary;

        public static Color HeaderPositive = Color.FromHex("#dbeed3");
        public static Color HeaderPositiveText = Color.FromHex("#356b35");
        public static Color HeaderWarning = Color.FromHex("#fcf7df");
        public static Color HeaderWarningText = Color.FromHex("#7f6234");

        public static Color Button1 = Color.FromHex("#08d2c1");
        public static Color Button2 = Color.FromHex("#f6846a");
        public static Color Button3 = Color.FromHex("#eaeeef");
        public static Color Hyperlink = Color.FromHex("#007aff");

        //public static Color Color.FromHex(string hexColor)
        //{
        //    var color = hexColor.Replace("#", "");
        //    try
        //    {
        //        var value = Int64.Parse(color);

        //        var a = value & 0xFF000000 >> 6;
        //        var r = value & 0x00FF0000 >> 4;
        //        var g = value & 0x0000FF00 >> 2;
        //        var b = value & 0x000000FF;
        //        if (hexColor.Length > 6)
        //        {
        //            return Color.FromArgb((int)a, (int)r, (int)g, (int)b);
        //        }
        //        else
        //        {
        //            return Color.FromArgb((int)r, (int)g, (int)b);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw new ArgumentException($"Can not convert {hexColor} to color.", ex);
        //    }
        //}
    }
}