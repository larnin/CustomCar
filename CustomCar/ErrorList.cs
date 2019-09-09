using System;
using System.Collections.Generic;

namespace CustomCar
{
    public static class ErrorList
    {
        static List<string> errors = new List<string>();

        public static void add(string error)
        {
            errors.Add(error);
            Console.Out.WriteLine("Custom car - Error: " +  error);
        }

        public static List<string> get()
        {
            return errors;
        }

        public static void clear()
        {
            errors.Clear();
        }

        public static bool haveErrors()
        {
            return errors.Count > 0;
        }

        public static void show()
        {
            if (errors.Count == 0)
                return;
            
            string error = "Can't load the cars correctly:" + errors.Count + " error(s)\n";
            for(int i = 0; i < errors.Count; i++)
                error += errors[i] + "\n";

            G.Sys.MenuPanelManager_.ShowError(error, "Custom cars error");
        }
    }
}
