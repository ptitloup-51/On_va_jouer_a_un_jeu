using System;
using System.Collections.Generic;
using System.IO;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using TiledSharp;

class Program
{
    // Déclare la constante TileSize comme statique
    public const int TileSize = 32;

    static void Main(string[] args)
    {
        // Création de la fenêtre SFML
        RenderWindow window = new RenderWindow(new VideoMode(800, 600), "Gestion de Map avec Tiled");
        window.Closed += (sender, e) => window.Close();

        // Charger la carte TMX
        string levelName = "level1";  // Nom du fichier TMX à charger
        TileManager tileManager = new TileManager("tileset.png");
        Map map = new Map("levels", levelName, tileManager);

        // Boucle principale
        while (window.IsOpen)
        {
            window.DispatchEvents();

            // Affichage de la carte
            window.Clear(Color.Black);
            map.Draw(window);
            window.Display();
        }
    }
}

class Tile
{
    public IntRect TextureRect { get; private set; }

    public Tile(IntRect textureRect)
    {
        TextureRect = textureRect;
    }
}

class TileManager
{
    private Texture texture;
    private Dictionary<int, Tile> tiles;

    public TileManager(string textureFile)
    {
        texture = new Texture(textureFile);
        tiles = new Dictionary<int, Tile>();

        // Supposons que vous avez un tileset de 8x8 tiles de 32x32 pixels chacun
        ConfigureTiles();
    }

    private void ConfigureTiles()
    {
        // Exemple de configuration pour 64 tiles d'une texture de 256x256 pixels
        for (int i = 0; i < 64; i++)
        {
            int x = (i % 8) * Program.TileSize;
            int y = (i / 8) * Program.TileSize;
            tiles[i] = new Tile(new IntRect(x, y, Program.TileSize, Program.TileSize));
        }
    }

    public Sprite GetSprite(int tileId)
    {
        if (tiles.ContainsKey(tileId))
        {
            Sprite sprite = new Sprite(texture);
            sprite.TextureRect = tiles[tileId].TextureRect;
            return sprite;
        }
        return null;
    }
}

class Map
{
    private TmxMap tmxMap;
    private TileManager tileManager;

    public Map(string levelDirectory, string levelName, TileManager tileManager)
    {
        string levelPath = Path.Combine(levelDirectory, levelName + ".tmx");
        tmxMap = new TmxMap(levelPath);  // Charger le fichier TMX
        this.tileManager = tileManager;
    }

    public void Draw(RenderWindow window)
    {
        // Parcours des couches et affichage des tuiles
        foreach (var layer in tmxMap.Layers)
        {
            if (layer.Visible)
            {
                // Dimensions de la couche : la largeur et la hauteur sont basées sur la collection de tuiles
                int layerHeight = layer.Tiles.Count / tmxMap.Width; // Nombre de lignes
                int layerWidth = tmxMap.Width; // Nombre de colonnes

                for (int y = 0; y < layerHeight; y++)
                {
                    for (int x = 0; x < layerWidth; x++)
                    {
                        // Calculer l'indice 1D pour accéder à la tuile
                        int index = x + y * layerWidth;

                        // Récupération de l'ID de la tuile (GID)
                        int tileId = layer.Tiles[index].Gid;

                        // Si tileId est valide (0 signifie aucune tuile)
                        if (tileId != 0)
                        {
                            Sprite sprite = tileManager.GetSprite(tileId - 1);  // Tile ID commence à 1 dans TMX, donc on soustrait 1
                            sprite.Position = new Vector2f(x * Program.TileSize, y * Program.TileSize);
                            window.Draw(sprite);
                        }
                    }
                }
            }
        }
    }
}
