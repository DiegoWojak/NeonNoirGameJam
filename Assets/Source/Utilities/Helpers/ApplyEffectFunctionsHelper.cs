
using Assets.Source.Managers;
using Assets.Source.Render.Characters;
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

        public float _AdditionalAirVelocity;
        public float _addtionalHeighJump;
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


        public void AddAdditionalAirVelocity(float value) {
            _AdditionalAirVelocity = value;
            My3DHandlerPlayer.Instance.Character.ModifyAdditionalAirMaxFromItem(_AdditionalAirVelocity);
        }

        public void RemoveAdditionalAirVelocity(float value) {
            _AdditionalAirVelocity = Mathf.Clamp(0, _AdditionalAirVelocity, _AdditionalAirVelocity - value);

            My3DHandlerPlayer.Instance.Character.ModifyAdditionalAirMaxFromItem(0);
        }

        public void AddAdditionalHeighJump(float value) {

            // My3DHandlerPlayer.Instance.Character.ModifyAdditionalAirMaxFromItem(_AdditionalAirVelocity);
            _addtionalHeighJump = value;
            My3DHandlerPlayer.Instance.Character.ModifyAdditionalHeightMaxJump(_addtionalHeighJump);
        } 

        public void RemoveAdditionalHeighJump(float value)
        {
            _addtionalHeighJump = 0;
            My3DHandlerPlayer.Instance.Character.ModifyAdditionalHeightMaxJump(_addtionalHeighJump);
        }
    }
}
