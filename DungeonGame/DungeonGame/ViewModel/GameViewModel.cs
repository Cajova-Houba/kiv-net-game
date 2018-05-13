using GameCore.Game;
using GameCore.Game.Actions;
using GameCore.Map;
using GameCore.Map.Generator;
using GameCore.Objects.Creatures;
using GameCore.Objects.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.ViewModel 
{
    /// <summary>
    /// View model for game related things.
    /// </summary>
    public class GameViewModel : INotifyPropertyChanged
    {
        public const int MAP_SEED = 123456;

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<String> gameMessages;
        /// <summary>
        /// Returns game messages to be displayed in bottom panel.
        /// </summary>
        public ObservableCollection<String> GameMessages
        {
            get
            {
                if (gameMessages == null)
                {
                    gameMessages = new ObservableCollection<string>(new String[] {
                        "Enjoy!",
                        "Game started.",
                        "Players generated.",
                        "Map generated.",
                        "New game initialization."
                    });
                }

                return gameMessages;
            }
        }
        
        /// <summary>
        /// Returns player's inventory with armor and weapon onf the first place (if any).
        /// </summary>
        public ObservableCollection<InventoryItemModel> Inventory {
            get
            {
                ObservableCollection<InventoryItemModel> inv = new ObservableCollection<InventoryItemModel>();
                AbstractPlayer player = Player;
                if(player == null)
                {
                    return inv;
                }

                if (player.Armor != null)
                {
                    inv.Add(new InventoryItemModel() { ModelObject = player.Armor });
                }
                if (player.Weapon != null)
                {
                    inv.Add(new InventoryItemModel() { ModelObject = player.Weapon });
                }
                foreach (AbstractInventoryItem invItem in player.Inventory)
                {
                    inv.Add(new InventoryItemModel() { ModelObject = invItem });
                }

                return inv;
            }
        }

        /// <summary>
        /// Returns the total value of items in inventory.
        /// </summary>
        public int InventoryValue
        {
            get
            {
                int val = 0;
                AbstractPlayer player = Player;
                if (player != null)
                {
                    foreach (AbstractInventoryItem invItem in player.Inventory)
                    {
                        val += invItem.ItemValue;
                    }
                }


                return val;
            }
        }
        
        /// <summary>
        /// Returns map of the current game.
        /// </summary>
        public Map GameMap { get; protected set; }
        
        /// <summary>
        /// Returns instance of current player.
        /// May return null.
        /// </summary>
        public AbstractPlayer Player {
            get {
                if (GameInstance != null && GameInstance.HumanPlayers != null && GameInstance.HumanPlayers.Count > 0)
                {
                    return GameInstance.HumanPlayers[0];
                } else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Returns current player HP.
        /// </summary>
        public int CurrentPlayerHP
        {
            get
            {
                if (Player != null)
                {
                    return Player.CurrentHitPoints;
                } else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Returns current player total attack.
        /// </summary>
        public double CurrentPlayerTotalAttack
        {
            get
            {
                return Player == null ? 0 : Player.TotalAttack;
            }
        }

        /// <summary>
        /// Returns current player total deffense.
        /// </summary>
        public double CurrentPlayerTotalDeffense
        {
            get
            {
                return Player == null ? 0 : Player.TotalDeffense;
            }
        }

        /// <summary>
        /// Returns current player position as "X,Y" string.
        /// </summary>
        public String CurrentPlayerPosition
        {
            get
            {
                if (Player == null || Player.Position == null)
                {
                    return "-";
                } else
                {
                    MapBlock pos = Player.Position;
                    return $"{pos.X},{pos.Y}";
                }
            }
        }

        /// <summary>
        /// Returns true if there's something on the current map block player can pick up.
        /// </summary>
        /// <returns></returns>
        public bool CanPickUp
        {
            get
            {
                if (Player == null || Player.Position == null)
                {
                    return false;
                } else
                {
                    return Player.Position.Item != null;
                }
            }
        }


        public Game GameInstance { get; protected set; }

        /// <summary>
        /// Initializes this view model with game instance.
        /// </summary>
        /// <param name="game">Game instance.</param>
        public GameViewModel(Game game)
        {
            GameMap = game.GameMap;
            GameInstance = game;

        }

        public GameViewModel()
        {
            // game initialization will be somehow placed here
            GameMap = MapGeneratorFactory.CreateSimpleMapGenerator().GenerateMap(20, 20, MAP_SEED);
            AbstractPlayer player = new HumanPlayer("Test player", GameMap.Grid[2,2]);
            Player.Inventory.AddRange(new BasicItem[]
            {
                new BasicItem("Test item", new MapBlock(), 10),
                new BasicItem("Test item 2", new MapBlock(), 10),
                new BasicItem("Some cool item", new MapBlock(), 10)
            });

            GameInstance = new Game();
            GameInstance.GameMap = GameMap;
            GameInstance.AddHumanPlayer(player);
        }

        /// <summary>
        /// Send move action for player to the game core.
        /// </summary>
        /// <param name="direction">Direction of move.</param>
        public void Move(Direction direction)
        {
            Player.NextAction = new Move() { Actor = Player, Direction = direction };
        }

        /// <summary>
        /// Send attack action for player to the game core.
        /// </summary>
        /// <param name="direction">Direction of attack.</param>
        public void Attack(Direction direction)
        {
            Player.NextAction = new Attack() { Actor = Player, Direction = direction };
        }

        /// <summary>
        /// Sends pickup action for current player to the game core.
        /// </summary>
        public void PickUp()
        {
            Player.NextAction = new PickUp() { Actor = Player };
        }

        /// <summary>
        /// Perform game loop step.
        /// </summary>
        public void GameLoopStep()
        {
            GameInstance.GameLoopStep();
        }

        /// <summary>
        /// Calls OnPropertyChanged() on multiple properties to update view.
        /// </summary>
        public void NotifyPropertyChanges()
        {
            OnPropertyChanged("CurrentPlayerHP");
            OnPropertyChanged("CurrentPlayerPosition");
            OnPropertyChanged("CurrentPlayerTotalAttack");
            OnPropertyChanged("CurrentPlayerTotalDeffense");
            OnPropertyChanged("Inventory");
            OnPropertyChanged("InventoryValue");
            OnPropertyChanged("GameMessages");
            OnPropertyChanged("CanPickUp");
        }

        /// <summary>
        /// Adds message and calls OnPropertyChanged.
        /// </summary>
        /// <param name="message"></param>
        public void AddGameMessage(string message)
        {
            GameMessages.Add(message);
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

    }
}
