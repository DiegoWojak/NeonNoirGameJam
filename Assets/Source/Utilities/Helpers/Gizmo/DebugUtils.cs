using System;


namespace Assets.Source.Utilities.Helpers.Gizmo
{
    public static class DebugUtils
    {

        enum Color { red, green, blue, black, white, yellow, orange };


        /// <summary>
        /// Crea un formato colorido usando lo siguiente 0: red, 1: green,2 :blue, 3: black, 4: white, 5: yellow, :orange;
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static String GetMessageFormat(string message, int color)
        {
            if (color < 0 || color > 6) { color = 0; }

            return String.Format("{0}{1}{2}{3}{4}",
                "<color=", ((Color)color).ToString(), ">",
                message,
                "</color>");
        }
    }
}
