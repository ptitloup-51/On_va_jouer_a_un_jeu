using System.Diagnostics;
using SFML.Graphics;
using SFML.System;
using System.IO;
using System.Xml.Linq;
using TiledSharp;

namespace On_va_jouer_a_un_jeu
{
    public class Map
    {
        private bool _IsInit = false;
        private TmxMap _tmxMap;
        private TileManager _tileManager;

        // Méthode pour charger la carte TMX et initialiser la carte
        public void LoadMap(string mapName)
        {
            if (!_IsInit)
            {
                // La map n'a pas encore été initialisée, nous procédons à son chargement

                // Charger le tileset
                string tilesetFile = "tileset2.tsx";  // Chemin vers votre fichier .tsx
                _tileManager = new TileManager(tilesetFile);

                // Charger la carte TMX
                string levelPath = Path.Combine("levels", mapName + ".tmx");  // Chemin vers le fichier TMX de la carte
                _tmxMap = new TmxMap(levelPath);

                // Marquer la carte comme initialisée
                _IsInit = true;
                Debug.WriteLine("Map initialisée avec succès");
            }
            else
            {
                Debug.WriteLine("La map a déjà été initialisée");
            }
        }

        // Vérifier si la carte a été initialisée
        public bool IsInit()
        {
            return _IsInit;
        }

        // Méthode pour dessiner la carte
        public void DrawMap(RenderWindow window)
        {
            if (!_IsInit)
            {
                Debug.WriteLine("La map n'a pas été initialisée");
                return;
            }

            // Dessiner la carte en parcourant chaque couche et en dessinant les tuiles
            foreach (var layer in _tmxMap.Layers)
            {
                if (layer.Visible)
                {
                    int layerWidth = _tmxMap.Width;
                    int layerHeight = _tmxMap.Height;

                    // Parcours des tuiles dans la couche
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
                                // Récupérer la sprite correspondant à l'ID de la tuile
                                Sprite sprite = _tileManager.GetSprite(tileId - 1);  // Tile ID commence à 1 dans TMX, donc on soustrait 1
                                sprite.Position = new Vector2f(x * Program.TileSize, y * Program.TileSize);
                                window.Draw(sprite);
                            }
                        }
                    }
                }
            }
        }
    }

    // Classe TileManager pour gérer le tileset
    class TileManager
    {
        private Texture texture;
        private Dictionary<int, Tile> tiles;
        private int tileWidth;
        private int tileHeight;

        public TileManager(string tilesetFile)
        {
            // Charger le tileset depuis un fichier .tsx
            XElement xTileset = XElement.Load(tilesetFile);  // Charger le fichier .tsx en tant qu'XElement
            TmxTileset tmxTileset = new TmxTileset(xTileset, Path.GetDirectoryName(tilesetFile));  // Créer le TmxTileset

            texture = new Texture(tmxTileset.Image.Source);
            tileWidth = tmxTileset.TileWidth;
            tileHeight = tmxTileset.TileHeight;

            tiles = new Dictionary<int, Tile>();
            ConfigureTiles(tmxTileset);
        }

        private void ConfigureTiles(TmxTileset tmxTileset)
        {
            // Charger les tuiles à partir du tileset
            long tilesPerRow = texture.Size.X / tileWidth; // Nombre de tuiles par ligne
            int tileIndex = 0;

            for (int y = 0; y < texture.Size.Y; y += tileHeight)
            {
                for (int x = 0; x < texture.Size.X; x += tileWidth)
                {
                    // Crée un objet Tile pour chaque tuile
                    tiles[tileIndex] = new Tile(new IntRect(x, y, tileWidth, tileHeight));
                    tileIndex++;
                }
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

    // Classe Tile représentant une tuile
    class Tile
    {
        public IntRect TextureRect { get; private set; }

        public Tile(IntRect textureRect)
        {
            TextureRect = textureRect;
        }
    }
   
}
