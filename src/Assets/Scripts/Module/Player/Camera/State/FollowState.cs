namespace Module.Player.Camera.State
{
    internal class FollowState : ICameraState
    {
        private CameraController controller;

        public FollowState(CameraController controller)
        {
            this.controller = controller;
        }

        public CameraState GetState()
        {
            return CameraState.Follow;
        }

        public void Update()
        {
        }
    }
}