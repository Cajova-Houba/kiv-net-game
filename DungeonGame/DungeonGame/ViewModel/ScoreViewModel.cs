using GameCore.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.ViewModel
{
    /// <summary>
    /// View model for score window.
    /// </summary>
    public class ScoreViewModel
    {
        public const int DEF_MAP_WIDTH = 10;
        public const int DEF_MAP_HEIGHT = 5;
        public const int DEF_AI_COUNT = 3;
        public const int DEF_MONSTER_COUNT = 4;
        public const int DEF_MOVE_COUNT = 153;
        public const int DEF_GAME_LENGTH = 5520;

        private int mapWidth;
        private int mapHeight;
        public String MapSize
        {
            get { return $"{mapWidth}x{mapHeight}"; }
        }

        private int aiCount;
        public int AiCount
        {
            get { return aiCount; }
        }

        private int monsterCount;
        public int MonsterCount
        {
            get { return monsterCount; }
        }

        private int moveCount;
        public int MoveCount
        {
            get { return moveCount; }
        }

        /// <summary>
        /// Game length in seconds.
        /// </summary>
        private int gameLength;
        public String GameLength
        {
            get
            {
                int h = gameLength / 3600;
                gameLength -= (h * 3600);
                int m = gameLength / 60;
                gameLength -= (m * 60);
                if (h > 0)
                {
                    return $"{h}h {m}m {gameLength}s";
                } else if (m > 0)
                {
                    return $"{m}m {gameLength}s";
                } else
                {
                    return $"{gameLength}s";
                }
            }
        }

        private List<LeaderboardItem> leaderBoardItems;
        public List<LeaderboardItem> LeaderBoardItems
        {
            get { return leaderBoardItems.OrderByDescending(it => it.TotalScore).ToList(); }
        }

        /// <summary>
        /// Initializes this model with default data.
        /// </summary>
        public ScoreViewModel()
        {
            mapWidth = DEF_MAP_WIDTH;
            mapHeight = DEF_MAP_HEIGHT;
            aiCount = DEF_AI_COUNT;
            monsterCount = DEF_MONSTER_COUNT;
            moveCount = DEF_MOVE_COUNT;
            gameLength = DEF_GAME_LENGTH;
            leaderBoardItems = new List<LeaderboardItem>();
            leaderBoardItems.Add(new LeaderboardItem() { PlayerName = "Květoslav", TotalScore = 5471 });
            leaderBoardItems.Add(new LeaderboardItem() { PlayerName = "Hvězdoslav", TotalScore = 7418 });
            leaderBoardItems.Add(new LeaderboardItem() { PlayerName = "Slavslav", TotalScore = 8931 });
        }


        /// <summary>
        /// Initializes this view model with values from passed game instance object.
        /// </summary>
        /// <param name="gameInstance">Object containing game data.</param>
        /// <param name="totalTime">Total game time.</param>
        public ScoreViewModel(Game gameInstance, int totalTime)
        {
            gameLength = totalTime;
            mapWidth = gameInstance.GameMap.Width;
            mapHeight = gameInstance.GameMap.Height;
            aiCount = gameInstance.AiPlayers.Count;
            monsterCount = gameInstance.Monsters.Count;
            moveCount = 0;

            leaderBoardItems = new List<LeaderboardItem>();
            leaderBoardItems.Add(new LeaderboardItem() { PlayerName = gameInstance.HumanPlayers[0].Name, TotalScore = gameInstance.HumanPlayers[0].TotalInventoryValue });

            gameInstance.AiPlayers.ForEach(aiPlayer => leaderBoardItems.Add(new LeaderboardItem() { PlayerName = aiPlayer.Name, TotalScore = aiPlayer.TotalInventoryValue }));
        }
    }


    /// <summary>
    /// View model class for leaderboard.
    /// </summary>
    public class LeaderboardItem
    {
        public String PlayerName { get; set; }
        public int TotalScore { get; set; }
    }
}
