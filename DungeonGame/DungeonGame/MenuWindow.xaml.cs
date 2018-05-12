﻿using DungeonGame.Model;
using GameCore.Game;
using GameCore.Map;
using GameCore.Map.Generator;
using GameCore.Objects.Creatures;
using GameCore.Objects.Creatures.AIPlayers;
using GameCore.Objects.Items;
using GameCore.Objects.Items.Armors;
using GameCore.Objects.Items.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DungeonGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        public MenuWindow()
        {
            InitializeComponent();
        }

        private void newGameBtn_Click(object sender, RoutedEventArgs e)
        {
            GameWindow gameWindow = new GameWindow(new GameViewModel(GenerateNewGame()));
            App.Current.MainWindow = gameWindow;
            this.Close();
            gameWindow.Show();
        }

        private Game GenerateNewGame()
        {
            Random r = new Random();
            Map gameMap = MapGeneratorFactory.CreateSimpleMapGenerator().GenerateMap(40, 40, r.Next(99999));
            AbstractPlayer player = new HumanPlayer("Test player", gameMap.Grid[2, 2]);
            Game game = new Game() { GameMap = gameMap };
            game.AddHumanPlayer(player);
            game.AddAIPlayer(AIPlayerFactory.CreateSimpleAIPLayer("Test AI Player", gameMap.Grid[1, 1]));
            game.AddAIPlayer(AIPlayerFactory.CreateSimpleAIPLayer("Test AI Player 2", gameMap.Grid[6, 6]));
            game.AddMonster(MonsterFactory.CreateGoblin("Goblin", gameMap.Grid[4, 7]));
            game.AddItem(new LeatherArmor("Leather armor", game.GameMap.Grid[1, 1]));
            game.AddItem(new Axe("Rusty axe", game.GameMap.Grid[3, 4]));
            game.AddItem(new BasicItem("Golden ring", game.GameMap.Grid[6, 7], 15));

            return game;
        }
    }
}
