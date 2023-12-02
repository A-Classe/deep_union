namespace Module.Player.State
{
    internal interface IPlayerState
    {
        public PlayerState GetState();

        public void Start();
        public void Stop();
        public void FixedUpdate();
    }
}