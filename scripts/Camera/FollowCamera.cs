using Godot;

namespace pdxpartyparrot.ssjAug2022.Camera
{
    // this goes on the camera's container
    // not the camera itself
    public class FollowCamera : Viewer
    {
        [Export]
        private Vector3 _offset;

        private Spatial _target;

        public override void _Process(float delta)
        {
            if(_target != null) {
                Translation = _target.Translation + _offset;
            }
        }

        public void Follow(Spatial target)
        {
            _target = target;
        }

        public override void OnRelease()
        {
            _target = null;
        }
    }
}
