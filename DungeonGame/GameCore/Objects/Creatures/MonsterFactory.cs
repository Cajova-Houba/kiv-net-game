using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;

namespace GameCore.Objects.Creatures
{
    /// <summary>
    /// Factory class for generating various monsters. 
    /// Methods here encapsulate  constructor calls so it's not necessary to specify stat of same monsters again and again.
    /// </summary>
    public class MonsterFactory
    {
        /// <summary>
        /// Number of types of monster this factory can produce.
        /// </summary>
        public const int MONSTER_TYPES_COUNT = 2;

        // rat stats
        public const int RAT_HP = 5;
        public const int RAT_DMG = 2;
        public const int RAT_DEF = 1;

        // goblin stats
        public const int GOBLIN_HP = 10;
        public const int GOBLIN_DMG = 5;
        public const int GOBLIN_DEF = 2;

        public static Monster CreateRat(string name, MapBlock position)
        {
            return new Monster(name, position, RAT_HP, RAT_DMG, RAT_DEF);
        }

        public static Monster CreateGoblin(string name, MapBlock position)
        {
            return new Monster(name, position, GOBLIN_HP, GOBLIN_DMG, GOBLIN_DEF);
        }

        /// <summary>
        /// Creates random monster (uniform distribution). 
        /// </summary>
        /// <param name="position">Position of random monster.</param>
        /// <returns>Monster.</returns>
        public static Monster CreateRandomMonster(MapBlock position)
        {
            Random r = new Random();
            int randVal = r.Next(MONSTER_TYPES_COUNT);

            switch(randVal)
            {
                case 1:
                    return CreateGoblin("Goblin", position);
                default:
                    return CreateRat("Rat", position);
            }
        }
        

    }
}
