using System.Net.Sockets;
using NLua;

namespace On_va_jouer_a_un_jeu;

public class luaInterpreter
{
    // Dictionnaire pour stocker les fonctions C#
    static Dictionary<string, Delegate> functions = new Dictionary<string, Delegate>();

    public void Interprete(string luaCode)
    {
        using (var lua = new Lua())
        {
            // Ajouter des fonctions C# au dictionnaire
            lua["greet"] = new Action<string>((name) => { Console.WriteLine($"Hello, {name}!"); });

            lua["add"] = new Func<int, int, int>((a, b) => { return a + b; });
            


            // Exposer les fonctions C# à Lua
            lua["functions"] = functions;
            try
            {
                // Exécuter le code Lua
                lua.DoString(luaCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing Lua code: " + ex.Message);
            }
        }
    }
}