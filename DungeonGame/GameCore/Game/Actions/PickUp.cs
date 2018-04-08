using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;
using GameCore.Objects.Items;
using GameCore.Objects.Creatures;
using GameCore.Game.Actions.Exceptions;

namespace GameCore.Game.Actions
{
    /// <summary>
    /// Picks up the current item in the map block. If the item is weapon or armor and player already has one of these, original one will be dropped.
    /// Note that if the actor is not child of AbstractPlayer class, nothing will happen.
    /// </summary>
    public class PickUp : AbstractAction
    {
        public override void Execute()
        {

            if (!(Actor is AbstractPlayer))
            {
                return;
            }

            AbstractPlayer player = (AbstractPlayer)Actor;
            MapBlock currentBlock = player.Position;
            if (currentBlock == null || currentBlock.Item == null)
            {
                throw new NoItemToPickUpException(player);
            } else if (currentBlock.Item is AbstractInventoryItem)
            {
                // normal item, just pick it up
                if (player.IsInventoryFull())
                {
                    throw new InventoryIsFullException(player);
                 
                }

                player.AddItemToInventory((AbstractInventoryItem)(currentBlock.PickUpItem()));

            } else if (currentBlock.Item is AbstractWeapon)
            {
                // weapon
                AbstractWeapon oldWeapon = player.SwapWeapon((AbstractWeapon)(currentBlock.PickUpItem()));
                currentBlock.Item = oldWeapon;

            } else if (currentBlock.Item is AbstractArmor)
            {
                // armor
                AbstractArmor oldArmor = player.SwapArmor((AbstractArmor)(currentBlock.PickUpItem()));
                currentBlock.Item = oldArmor;
            } else
            {
                throw new PickUpUnknownItemException(player, currentBlock.Item);
            }
        }
    }
}
