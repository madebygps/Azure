using Newtonsoft.Json;

namespace MadeByGPS.Function
{
    public class LightSetting {

        public int hue { get; set; }

        public string make_call (string color) {

            this.hue = get_color (color);

            string json = JsonConvert.SerializeObject (this, Formatting.Indented);

            return json;
        }

        public int get_color (string color) {
            int color_value = 0;

            /* 
        
            blue: {"sat":255, "bri":100,"hue": 46920}
                    pink: {"sat":255, "bri":100,"hue": 36920}
                    white: {"sat":255, "bri":100,"hue": 36920}
                    yellow: {"sat":255, "bri":100,"hue": 16920}
                    orange: {"sat":255, "bri":100,"hue": 5999}
        
        
            */

            switch (color) {

                case "blue":
                    color_value = 46920;
                    break;

                case "orange":
                    color_value = 5999;
                    break;

                case "pink":
                    color_value = 36920;
                    break;

                case "yellow":
                    color_value = 16920;
                    break;

                default:
                    color_value = 0;
                    break;
            }

            return color_value;

        }
    }
}