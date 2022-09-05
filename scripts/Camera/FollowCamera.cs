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

        #region Godot Lifecycle

        public override void _Process(float delta)
        {
            if(_target != null) {
                GlobalTranslation = _target.GlobalTranslation + _offset;
            }
        }

        #endregion

        public void Follow(Spatial target)
        {
            _target = target;
        }

        #region Events

        public override void OnRelease()
        {
            base.OnRelease();

            _target = null;
        }

        #endregion
    }
}
