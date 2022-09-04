using Godot;

namespace pdxpartyparrot.ssjAug2022.Camera
{
    // this goes on the viewer's pivot container
    // not the camera itself
    public abstract class Viewer : Spatial
    {
        public Godot.Camera Camera { get; private set; }

        #region Godot Lifecycle

        public override void _Ready()
        {
            Camera = GetNode<Godot.Camera>("Camera");
        }

        #endregion

        #region Events

        public virtual void OnRelease()
        {
        }

        #endregion
    }
}
