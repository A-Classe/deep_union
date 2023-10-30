using System;
using Core.Model.Player;
using Wanna.DebugEx;

namespace Module.Player
{
    public class PlayerStatus
    {
        private readonly short maxHp;
        private short hp;

        public short Hp => hp;
        public short MaxHp => maxHp;

        public event Action<short> OnHpChanged;

        public PlayerStatus(PlayerStatusModel model)
        {
            if (model.maxHp.HasValue) maxHp = model.maxHp.Value;

            if (model.hp.HasValue) hp = model.hp.Value;
        }

        public void AddHp(uint value)
        {
            SetHp((short)(hp + Parse(value)));
        }

        public void RemoveHp(uint value)
        {
            SetHp((short)(hp - Parse(value)));
        }

        private void SetHp(short newHp)
        {
            hp = Math.Clamp(newHp, (short)0, maxHp);
            OnHpChanged?.Invoke(hp);
        }

        private static short Parse(uint value)
        {
            return checked((short)value);
        }
    }
}