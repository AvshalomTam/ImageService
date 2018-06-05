using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.Converter
{
    class TypeToBrushConverter
    {
        public static Dictionary<string, string> brushConverter = new Dictionary<string, string> {
            { "INFO", "LightGreen" }, { "ERROR", "Red" }, { "WARNING", "Yellow" } };

        /// <summary>
        /// Given a Log type returns the brush.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static string getBrush(string type)
        {
            try
            {
                if (brushConverter.TryGetValue(type, out string brush))
                    return brush;
                else
                    return "Black";
            }
            catch
            {
                return "Black";
            }
        }
    }
}
