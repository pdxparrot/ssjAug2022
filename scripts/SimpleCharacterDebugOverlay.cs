using Godot;

using pdxpartyparrot.ssjAug2022.Managers;

namespace pdxpartyparrot.ssjAug2022
{
    public class SimpleCharacterDebugOverlay : Control
    {
        private SimpleCharacter _owner;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _owner = GetOwner<SimpleCharacter>();
        }

        public override void _Draw()
        {
            var viewer = GameManager.Instance.Level.Viewer;

            var color = new Color(0, 1, 0);
            var start = viewer.Camera.UnprojectPosition(_owner.GlobalTransform.origin);
            var end = viewer.Camera.UnprojectPosition(_owner.GlobalTransform.origin + _owner.Velocity);
            DrawLine(start, end, color, 2.0f);
        }

        #endregion
    }
}
