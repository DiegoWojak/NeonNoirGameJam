
using Assets.Source.Managers;
using UnityEngine;

namespace Assets.Source.Utilities.Helpers
{
    public class ApplyEffectFunctionsHelper : MonoBehaviour
    {
        public bool _hasTVGlasses;
        public bool _HasRGBGlasses;
        public bool _CanDash;
        public bool _CanDoubleJump;
        public bool _CanWallJump;
        public bool _CanSwim;
        public bool _CanSpeakAnimal;
        public void EquipTVGlasses(bool equip) {
            _hasTVGlasses = equip;
        }

        public void EquipRGBGlasses(bool equip)
        {
            _HasRGBGlasses = equip;
        }
        public void EquipWolfSkill(bool equip) // Dash
        {
            _CanDash = equip;
        }
        public void EquipCatSkill(bool equip) //WallJump
        {
            _CanWallJump = equip;
        }
        public void EquipWings(bool equip) // Double Jump
        {
            _CanDoubleJump = equip;
        }
        public void EquipSwimEquip(bool equip)
        {
            _CanSwim = equip;
        }

        public void HeartMode(bool equip)
        {
            _CanSpeakAnimal = equip;
        }

    }
}
