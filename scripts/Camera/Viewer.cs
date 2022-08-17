using Godot;

namespace pdxpartyparrot.ssjAug2022.Camera
{
    // this goes on the viewer's pivot container
    // not the camera itself
    public abstract class Viewer : Spatial
    {
        #region Events

        public virtual void OnRelease()
        {
        }

        #endregion
    }
}
