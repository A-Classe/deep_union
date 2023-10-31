using System;
using Core.Model.Player;

namespace Module.Player
{
    /// <summary>
    /// ゲームの進行に関するプレイヤーのステータスの管理
    /// </summary>
    public class PlayerStatus
    {
        private readonly short maxHp;
        private short hp;

        public short Hp => hp;
        public short MaxHp => maxHp;

        /// <summary>
        /// 現在のHPと比較して変更があった場合のみ呼ばれる
        /// </summary>
        public event Action<short> OnHpChanged;

        public event Action OnCallHpZero;

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
            var newer = Math.Clamp(newHp, (short)0, maxHp);
            if (hp == newer) return;
            hp = newer;
            OnHpChanged?.Invoke(hp);
            if (hp == 0)
            {
                OnCallHpZero?.Invoke();
            }
        }

        private static short Parse(uint value)
        {
            return checked((short)value);
        }
    }
}