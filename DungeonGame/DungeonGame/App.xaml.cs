using DungeonGame.Common;
using DungeonGame.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DungeonGame
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // call instance of global configuration so that it is initialized
            GlobalConfiguration.GetInstance();
        }

    }
}
