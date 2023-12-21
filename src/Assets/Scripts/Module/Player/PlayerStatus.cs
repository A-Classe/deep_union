using System;
using Core.Model.Player;

namespace Module.Player
{
    /// <summary>
    ///     ゲームの進行に関するプレイヤーのステータスの管理
    /// </summary>
    public class PlayerStatus
    {
        public PlayerStatus(PlayerStatusModel model)
        {
            if (model.maxHp.HasValue)
            {
                MaxHp = model.maxHp.Value;
            }

            if (model.hp.HasValue)
            {
                Hp = model.hp.Value;
            }
        }

        public short Hp { get; private set; }

        public short MaxHp { get; }

        /// <summary>
        ///     現在のHPと比較して変更があった場合のみ呼ばれる
        /// </summary>
        public event Action<short> OnHpChanged;

        public event Action OnCallHpZero;

        public void AddHp(uint value)
        {
            SetHp((short)(Hp + Parse(value)));
        }

        public void RemoveHp(uint value)
        {
            SetHp((short)(Hp - Parse(value)));
        }

        private void SetHp(short newHp)
        {
            var newer = Math.Clamp(newHp, (short)0, MaxHp);
            if (Hp == newer)
            {
                return;
            }

            Hp = newer;
            OnHpChanged?.Invoke(Hp);
            if (Hp == 0)
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