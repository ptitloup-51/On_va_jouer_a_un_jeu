using System;
using System.IO.Compression;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace On_va_jouer_a_un_jeu
{
    class Program
    {
        // Déclare la constante TileSize comme statique
        public const int TileSize = 32;

        static void Main(string[] args)
        {
            // Création de la fenêtre SFML
            RenderWindow window = new RenderWindow(new VideoMode(800, 600), "Gestion de Map avec Tiled");
            window.Closed += (sender, e) => window.Close();
            
            luaInterpreter interpreter = new luaInterpreter();
            interpreter.Interprete(File.ReadAllText("test.lua"));

            // Créer une instance de la classe Map
            Map map = new Map();

            // Charger la carte spécifiée, ici "level2" est le nom de la carte sans l'extension .tmx
            map.LoadMap("level2");

            // Boucle principale
            while (window.IsOpen)
            {
                window.DispatchEvents();

                // Affichage de la carte
                window.Clear(Color.Black);
                map.DrawMap(window);  // Utiliser la méthode DrawMap pour dessiner la carte
                window.Display();
            }
        }
    }
}