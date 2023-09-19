namespace Module.Player.State
{
    internal class WaitState : IPlayerState
    {
        public PlayerState GetState() => PlayerState.Wait;

        public void Update()
        {
            // NOP.
        }
    }
}