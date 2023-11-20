namespace Module.Player.Camera.State
{
    internal class IdleState : ICameraState
    {
        public CameraState GetState()
        {
            return CameraState.Idle;
        }

        public void Update()
        {
            // NOP.
        }
    }
}