using Godot;

using System.Collections.Generic;

using pdxpartyparrot.ssjAug2022.Managers;
using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022
{
    public class DebugOverlay : SingletonUI<DebugOverlay>
    {
        private readonly HashSet<IDebugDraw> _draw = new HashSet<IDebugDraw>();

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();

            // disable processing if we aren't in the editor
            if(!PartyParrotManager.Instance.IsEditor) {
                SetProcess(false);
            }
        }

        public override void _Process(float delta)
        {
            Update();
        }

        public override void _Draw()
        {
            if(!PartyParrotManager.Instance.IsEditor || GameManager.Instance.Level == null || GameManager.Instance.Level.Viewer == null) {
                return;
            }

            var viewer = GameManager.Instance.Level.Viewer;
            foreach(var draw in _draw) {
                draw.DebugDraw(this, viewer.Camera);
            }
        }

        #endregion

        public void RegisterDebugDraw(IDebugDraw draw)
        {
            _draw.Add(draw);
        }

        public void UnRegisterDebugDraw(IDebugDraw draw)
        {
            _draw.Remove(draw);
        }
    }
}
