using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;

namespace GameCore.Objects.Items.Weapons
{
    /// <summary>
    /// Basic axe.
    /// </summary>
    public class Axe : AbstractWeapon
    {
        public const int DAMAGE = 3;

        public Axe(string name, MapBlock position) : base(name, position, DAMAGE)
        {
        }

        public override string ToString()
        {
            return $"Sword [name={Name}, damage={Damage}]";
        }

        public override bool Equals(object obj)
        {
            var sword = obj as Axe;
            return sword != null &&
                   Damage == sword.Damage &&
                   Name == sword.Name;
        }

        public override int GetHashCode()
        {
            var hashCode = 435508838;
            hashCode = hashCode * -1521134295 + Damage.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            return hashCode;
        }
    }
}
