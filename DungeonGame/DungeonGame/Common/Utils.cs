using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DungeonGame.Common
{
    /// <summary>
    /// Library class which contains some usefull methods.
    /// </summary>
    public class Utils
    {

        /// <summary>
        /// Shows simple error message dialog with OK button.
        /// </summary>
        /// <param name="errorMessage"></param>
        public static void ShowErrorMessage(string errorMessage)
        {
            MessageBox.Show(errorMessage, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Tries to parse integer from source and checks it's range.
        /// </summary>
        /// <param name="source">String with integer to be checked.</param>
        /// <param name="min">Min allowed value.</param>
        /// <param name="max">Max allowed value.</param>
        /// <param name="parseErrorMessage">Error message to be displayed if error occurs during parsing.</param>
        /// <param name="rangeErrorMessage">Error message to be displayed if error occurs during rarnge check.</param>
        /// <returns>True if source is ok integer.</returns>
        public static bool CheckRangedInt(string source, int min, int max, string parseErrorMessage, string rangeErrorMessage)
        {
            int res = 0;
            if (!Int32.TryParse(source, out res))
            {
                MessageBox.Show(parseErrorMessage, "Chybe", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            else
            {
                if (res < min || res > max)
                {
                    MessageBox.Show(rangeErrorMessage, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            return true;
        }
    }
}
