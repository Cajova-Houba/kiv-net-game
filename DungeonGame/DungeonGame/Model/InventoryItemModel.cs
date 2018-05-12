using GameCore.Objects.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DungeonGame.Model
{
    /// <summary>
    /// Simple model which wraps inventory item.
    /// </summary>
    public class InventoryItemModel
    {
        public AbstractItem ModelObject { get; set; }

        public String ItemName {
            get
            {
                return ModelObject == null ? "" : ModelObject.Name;
            }
        }

        public BitmapImage ItemImage
        {
            get
            {
                return new BitmapImage();
            }
        }
    }
}
