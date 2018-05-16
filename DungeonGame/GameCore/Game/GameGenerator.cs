using GameCore.Map.Generator;
using GameCore.Objects.Creatures;
using GameCore.Objects.Creatures.AIPlayers;
using GameCore.Objects.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Game
{
    /// <summary>
    /// Library class used to generate new game isntances from given parameters.
    /// </summary>
    public class GameGenerator
    {
        /// <summary>
        /// Generates new game instance from existing map.
        /// Good for using imported maps.
        /// </summary>
        /// <param name="map">Map to be used for generation.</param>
        /// <param name="humanPlayerName">
        /// If the provided map doesn't contain any human players, one will be randomly generated (if there's place left on the map) and this name will be used.
        /// If the provided map does contain human palyers, name of the first player will be set to this.
        /// </param>
        /// <returns>Generated game instance.</returns>
        public static Game GenerateGame(Map.Map map, string humanPlayerName)
        {
            int blockCount = map.Width * map.Height;
            List<AbstractPlayer> humanPlayers = new List<AbstractPlayer>();
            List<AbstractPlayer> aiPlayers = new List<AbstractPlayer>();
            List<AbstractCreature> monsters = new List<AbstractCreature>();

            foreach(Map.MapBlock mapBlock in map.Grid)
            {
                if (mapBlock.Occupied)
                {
                    if (mapBlock.Creature is HumanPlayer) { humanPlayers.Add((HumanPlayer)mapBlock.Creature); }
                    else if (mapBlock.Creature is AbstractPlayer ) { aiPlayers.Add((AbstractPlayer)mapBlock.Creature); }
                    else { monsters.Add(mapBlock.Creature); }
                }
            }

            // check if there's human player and that there's also free map blocks to place him
            if (humanPlayers.Count == 0 && (blockCount > (aiPlayers.Count + monsters.Count)))
            {
                Random r = new Random();
                bool placed = false;

                // if the player is not placed after this number of attempts, something is probably wrong
                int maxAttempts = 1000;
                int attempt = 0;
                while(!placed && attempt < maxAttempts)
                {
                    int px = r.Next(map.Width);
                    int py = r.Next(map.Height);

                    if (!map.Grid[px,py].Occupied)
                    {
                        HumanPlayer hp = new HumanPlayer(humanPlayerName, map.Grid[px, py]);
                        map.AddCreature(hp);
                        humanPlayers.Add(hp);
                        placed = true;
                    }

                    attempt++;
                }
            } else
            {
                humanPlayers[0].Name = humanPlayerName;
            }

            // generate game instance
            Game game = new Game()
            {
                GameMap = map,
                HumanPlayers = humanPlayers,
                AiPlayers = aiPlayers,
                Monsters = monsters
            };

            return game;
        }

        /// <summary>
        /// Generates new game with given parameters. If aiCount+1 (1 for human player) is greater than width*heght, exception is thrown.
        /// </summary>
        /// <param name="width">Width of the map.</param>
        /// <param name="height">Height of the map.</param>
        /// <param name="mapSeed">Map seed.</param>
        /// <param name="aiCount">Number of AIs (simple AI is used).</param>
        /// <param name="monsterDensity">Density of monsters 0..1</param>
        /// <param name="itemDensity">Density of items 0..1</param>
        /// <param name="humanPlayerName">Name of the human player.</param>
        /// <returns>Generated game instance.</returns>
        public static Game GenerateGame(int width, int height, int mapSeed, int aiCount, double monsterDensity, double itemDensity, string humanPlayerName)
        {
            // check
            if (aiCount+1 > width*height)
            {
                throw new Exception($"Počet protihráčů {aiCount} je na plochu {width}x{height} moc velký!");
            }

            Map.Map gameMap = MapGeneratorFactory.CreateSimpleMapGenerator().GenerateMap(width, height, mapSeed);
            Game game = new Game() { GameMap = gameMap };
            Random r = new Random();


            // sets of occupied position for creatures and items, '{x}:{y}'
            HashSet<String> creatureOccupiedPositions = new HashSet<string>();
            HashSet<string> itemsOccupiedPositions = new HashSet<string>();

            // place human player
            int x = r.Next(width);
            int y = r.Next(height);
            AbstractPlayer player = new HumanPlayer(humanPlayerName, gameMap.Grid[x, y]);
            game.AddHumanPlayer(player);
            creatureOccupiedPositions.Add($"{x}:{y}");

            // place AI players
            int pCount = 1;
            AddObjectsToGame(width, height, aiCount, 20, (ox, oy) =>
            {
                if (!creatureOccupiedPositions.Contains($"{ox}:{oy}"))
                {
                    game.AddAIPlayer(AIPlayerFactory.CreateSimpleAIPLayer($"Simple AI Player {pCount}", gameMap.Grid[ox, oy]));
                    creatureOccupiedPositions.Add(($"{ox}:{oy}"));
                    pCount++;
                    return true;
                } else
                {
                    return false;
                }
            });

            // monster count from density:
            // density is expected to be in range 0..1 and will be mapped to the count of remaining free map blocks
            // this number is then lowered to 2/3 and this result is used as a base
            // then monsterCount = base + random.next(base/3)
            double monsterCountBase = 2 * (monsterDensity * (width * height - aiCount)) / 3;
            int monsterCount = (int)(monsterCountBase + r.NextDouble() * (monsterCountBase / 3));

            // place monsters
            AddObjectsToGame(width, height, monsterCount, 20, (ox, oy) =>
            {
                if (!creatureOccupiedPositions.Contains($"{ox}:{oy}"))
                {
                    game.AddMonster(MonsterFactory.CreateRandomMonster(gameMap.Grid[ox, oy]));
                    creatureOccupiedPositions.Add(($"{ox}:{oy}"));
                    return true;
                } else
                {
                    return false;
                }
            });


            // item count is calculated the same way as monster count is
            double itemCountBase = 2 * (itemDensity * (width * height - aiCount)) / 3;
            int itemCount = (int)(itemCountBase + r.NextDouble() * (itemCountBase / 3));

            // place items
            AddObjectsToGame(width, height, itemCount, 20, (ox, oy) =>
            {
                if (!itemsOccupiedPositions.Contains($"{ox}:{oy}"))
                {
                    game.AddItem(ItemFactory.CreateRandomItem(gameMap.Grid[ox, oy]));
                    itemsOccupiedPositions.Add(($"{ox}:{oy}"));
                    return true;
                } else
                {
                    return false;
                }
            });

            return game;
        }

        /// <summary>
        /// Wrapper for placing game objects to game. 
        /// </summary>
        /// <param name="width">Width of the map. Used to generate x-coordinate of item's new position.</param>
        /// <param name="height">Height of the map. Used to generate y-coordinate of item's new position.</param>
        /// <param name="objectCount">Number of items to be placed. Random coordinates are generated.</param>
        /// <param name="maxAttempts">Max number of attempts to randomly place one item.</param>
        /// <param name="placeFunction">Function which tries to place the item to the game. Should return true if the placement was successfull. Parameters are x and y.</param>
        private static void AddObjectsToGame(int width, int height, int objectCount, int maxAttempts, Func<int,int, bool> placeFunction)
        {
            Random r = new Random();
            for(int i = 0; i < objectCount; i++)
            {
                int attempt = 0;
                bool placed = false;
                while (!placed && (attempt < maxAttempts))
                {
                    attempt++;
                    int x = r.Next(width);
                    int y = r.Next(height);
                    placed = placeFunction(x, y);
                }
            }
        }
    }
}
