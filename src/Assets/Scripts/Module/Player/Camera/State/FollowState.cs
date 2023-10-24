namespace Module.Player.Camera.State
{
    internal class FollowState : ICameraState
    {
        private CameraController controller;
        public CameraState GetState() => CameraState.Follow;

        public FollowState(CameraController controller)
        {
            this.controller = controller;
        }

        public void Update()
        {
            
        }
    }
}