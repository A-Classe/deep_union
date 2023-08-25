namespace Module.Player.State
{
    internal interface IPlayerState
    {
        public PlayerState GetState();

        public void Update();
    }
}