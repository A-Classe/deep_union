namespace Module.Player.State
{
    internal class WaitState : IPlayerState
    {
        public PlayerState GetState()
        {
            return PlayerState.Wait;
        }

        public void Update()
        {
            // NOP.
        }
    }
}