namespace Module.Player.Camera.State
{
    internal class IdleState : ICameraState
    {
        public CameraState GetState() => CameraState.Idle;

        public void Update()
        {
            // NOP.
        }
    }
}