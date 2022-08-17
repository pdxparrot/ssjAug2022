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
                var transform = Transform;
                transform.origin.x = _target.Transform.origin.x + _offset.x;
                transform.origin.y = _target.Transform.origin.y + _offset.y;
                transform.origin.z = _target.Transform.origin.z + _offset.z;
                Transform = transform;
            }
        }

        public void Follow(Spatial target)
        {
            _target = target;
        }
    }
}
