using GameCore.Map;
using GameCore.Objects.Items;
using System.Collections.Generic;

namespace GameCore.Objects.Creatures
{
    /// <summary>
    /// Base class for all 'player-like' objects. Those are AI player and human player.
    /// </summary>
    public abstract class AbstractPlayer : AbstractCreature
    {
        /// <summary>
        /// Max number of items in inventory. 
        /// </summary>
        public const int DEFAULT_INVENTORY_SIZE = 10;

        /// <summary>
        /// Default base HP for all players.
        /// </summary>
        public const int DEFAULT_BASE_HP = 100;

        /// <summary>
        /// Default base attack for all players.
        /// </summary>
        public const int DEFAULT_BASE_ATTACK = 20;

        /// <summary>
        /// Default base defense for all players.
        /// </summary>
        public const int DEFAULT_BASE_DEFFENSE = 15;

        /// <summary>
        /// Player's inventory. 
        /// </summary>
        public List<AbstractInventoryItem> Inventory { get; protected set; }
        
        /// <summary>
        /// Armor equipped by player. Null if no armor is equipped.
        /// </summary>
        public AbstractArmor Armor { get; protected set; }

        /// <summary>
        /// Weapon equipped by player. Null if no weapon is equipped.
        /// </summary>
        public AbstractWeapon Weapon { get; protected set; }

        /// <summary>
        /// Returns the value of all items in player's inventory.
        /// </summary>
        public int TotalInventoryValue
        {
            get
            {
                int totalValue = 0;
                if (Inventory != null && Inventory.Count > 0)
                {
                    foreach(AbstractInventoryItem inventoryItem in Inventory)
                    {
                        totalValue += inventoryItem.ItemValue;
                    }
                }

                return totalValue;
            }
        }

        /// <summary>
        /// Property for getting max inventory size. Override it to use custom value.
        /// </summary>
        public virtual int InventorySize
        {
            get
            {
                return DEFAULT_INVENTORY_SIZE;
            }
        }

        public override double MaxHitPoints
        {
            get
            {
                int baseHp = BaseHitPoints;

                // any effect should be put here in the future

                return baseHp;
            }
        }

        public override double TotalAttack
        {
            get
            {
                int baseAtt = BaseAttack;

                if (Weapon != null )
                {
                    baseAtt += Weapon.Damage;
                }

                return baseAtt;
            }
        }

        public override double TotalDeffense
        {
            get
            {
                int baseDef = BaseDeffense;

                if (Armor != null)
                {
                    baseDef += Armor.Defense;
                }

                return baseDef;
            }
        }

        /// <summary>
        /// Constructor which initializes player with name, position and default values.
        /// </summary>
        /// <param name="name">Player's name.</param>
        /// <param name="position">Players position.</param>
        public AbstractPlayer(string name, MapBlock position) : base(name, position, DEFAULT_BASE_HP, DEFAULT_BASE_ATTACK, DEFAULT_BASE_DEFFENSE)
        {
            Inventory = new List<AbstractInventoryItem>();
        }


        /// <summary>
        /// Returns true if the number of items in inventory has reached InventorySize.
        /// </summary>
        /// <returns>True if the inventory is full, false otherwise.</returns>
        public bool IsInventoryFull()
        {
            return Inventory.Count == InventorySize;
        }


        /// <summary>
        /// Adds item to inventory. If the inventory is full nothing happens. Use IsInventoryFull() method first to check
        /// whether there's room for another item in inventory.
        /// </summary>
        /// <param name="item">Item to be added.</param>
        public void AddItemToInventory(AbstractInventoryItem item)
        {
            if (IsInventoryFull())
            {
                return;
            }

            Inventory.Add(item);
        }
       
        /// <summary>
        /// Returns item from inventory by its index.
        /// </summary>
        /// <param name="index">Index of item in inventory. If the index is out of range, null is returned.</param>
        /// <returns>Item from inventory.</returns>
        public AbstractInventoryItem GetItemFromInventory(int index)
        {
            if (index < 0 || index >= Inventory.Count)
            {
                return null;
            }

            return Inventory[index];
        }

        /// <summary>
        /// Removes item from inventory and returns it.
        /// </summary>
        /// <param name="index">Idenx of the item to be removed. If the index is out of range, null is returned.</param>
        /// <returns>Item removed from inventory.</returns>
        public AbstractInventoryItem RemoveItemFromInventory(int index)
        {
            if (index < 0 || index >= Inventory.Count)
            {
                return null;
            }

            AbstractInventoryItem item = Inventory[index];
            Inventory.RemoveAt(index);

            return item;
        }

        /// <summary>
        /// Swaps current armor with new one and returns the old one. If the current armor is null,
        /// null is returned.
        /// 
        /// Use this method when picking up new armor.
        /// </summary>
        /// <param name="newArmor">New armor.</param>
        /// <returns>Old armor or null if no armor was equipped.</returns>
        public AbstractArmor SwapArmor(AbstractArmor newArmor)
        {
            AbstractArmor oldArmor = Armor;
            Armor = newArmor;
            return oldArmor;
        }

        /// <summary>
        /// Swaps current weapon with new one and returns the old one. If the current weapon is null,
        /// null is returned.
        /// 
        /// Use this method when picking up new weapon.
        /// </summary>
        /// <param name="newWeapon">New weapon.</param>
        /// <returns>New weapon or null if no weapon was equipped.</returns>
        public AbstractWeapon SwapWeapon(AbstractWeapon newWeapon)
        {
            AbstractWeapon oldWeapon = Weapon;
            Weapon = newWeapon;
            return oldWeapon;
        }

    }
}
