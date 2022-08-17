using Godot;

using pdxpartyparrot.ssjAug2022.Managers;

namespace pdxpartyparrot.ssjAug2022.Camera
{
    // this goes on the viewer's container
    // not the camera itself
    public abstract class Viewer : Spatial
    {
        private Godot.Camera _camera;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _camera = GetNode<Godot.Camera>("Camera");
        }

        #endregion

        /*public void SetViewport(int x, int y, float viewportWidth, float viewportHeight)
        {
            float viewportX = x * viewportWidth;
            float viewportY = y * viewportHeight;

            Rect2 viewport = new Rect2(
                viewportX + ViewerManager.Instance.ViewportEpsilon,
                viewportY + ViewerManager.Instance.ViewportEpsilon,
                viewportWidth - (ViewerManager.Instance.ViewportEpsilon * 2.0f),
                viewportHeight - (ViewerManager.Instance.ViewportEpsilon * 2.0f));

            Camera.rect = viewport;
            UICamera.rect = viewport;

            UICameraAspectRatio aspectRatio = UICamera.GetComponent<UICameraAspectRatio>();
            if(null != aspectRatio) {
                aspectRatio.UpdateAspectRatio();
            }
        }*/
    }
}
