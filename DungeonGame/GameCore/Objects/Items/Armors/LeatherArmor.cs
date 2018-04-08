using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;

namespace GameCore.Objects.Items.Armors
{
    /// <summary>
    /// Basic leather armor.
    /// </summary>
    public class LeatherArmor : AbstractArmor
    {
        public const int DEFENSE = 2;

        public LeatherArmor(string name, MapBlock position) : base(name, position, DEFENSE)
        {
        }

        public override string ToString()
        {
            return $"LeatherArmor [name={Name}, defense={Defense}]";
        }

        public override bool Equals(object obj)
        {
            var armor = obj as LeatherArmor;
            return armor != null &&
                   Name == armor.Name &&
                   Defense == armor.Defense;
        }

        public override int GetHashCode()
        {
            var hashCode = 484231913;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<int>.Default.GetHashCode(Defense);
            return hashCode;
        }
    }
}
