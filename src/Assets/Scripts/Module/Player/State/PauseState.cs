namespace Module.Player.State
{
    public class PauseState : IPlayerState
    {
        public PlayerState GetState() => PlayerState.Pause;

        public void Update()
        {
            // NOP.
        }
    }
}