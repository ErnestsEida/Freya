using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Debugger {
    public enum Color {
        Success,
        Error,
        Warning,
        Info,
    }

    static private Dictionary<Color, ConsoleColor> _colors = new Dictionary<Color, ConsoleColor>() {
        { Color.Success, ConsoleColor.Green },
        { Color.Error, ConsoleColor.Red },
        { Color.Warning, ConsoleColor.Yellow },
        { Color.Info, ConsoleColor.Cyan },
    };

    static public void Say(bool debug, Color color, string message) {
        if (debug)
        {
            Console.ForegroundColor = _colors[color];
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
