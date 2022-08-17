using Godot;

using System.Collections.Generic;
using System.Linq;

using pdxpartyparrot.ssjAug2022.Camera;
using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022.Managers
{
    public class ViewerManager : SingletonNode<ViewerManager>
    {
        private class ViewerSet
        {
            public readonly HashSet<Viewer> Viewers = new HashSet<Viewer>();

            public readonly HashSet<Viewer> AssignedViewers = new HashSet<Viewer>();

            public readonly Queue<Viewer> UnassignedViewers = new Queue<Viewer>();

            public void InstanceViewers(int count, PackedScene viewerScene)
            {
                int actualCount = count - Viewers.Count;
                if(actualCount <= 0) {
                    return;
                }

                GD.Print($"[ViewerManager] Allocating {actualCount} viewers...");

                for(int i = 0; i < actualCount; ++i) {
                    Viewer viewer = (Viewer)viewerScene.Instance();

                    Viewers.Add(viewer);
                    UnassignedViewers.Enqueue(viewer);
                }
            }

            public T AcquireViewer<T>() where T : Viewer
            {
                if(UnassignedViewers.Count < 1) {
                    GD.PushWarning($"Attempt to acquire a viewer of type {typeof(T)} when there are none!");
                    return null;
                }

                var viewer = UnassignedViewers.Dequeue();
                Instance.AddChild(viewer);

                AssignedViewers.Add(viewer);

                if(Instance.EnableDebug) {
                    GD.Print($"[ViewerManager] Acquired viewer (type: {typeof(T)}, assigned: {AssignedViewers.Count}, unassigned: {UnassignedViewers.Count})");
                }
                return viewer as T;
            }

            public void ReleaseViewer<T>(T viewer) where T : Viewer
            {
                if(!AssignedViewers.Contains(viewer)) {
                    // TODO: log a warning?
                    return;
                }

                if(Instance.EnableDebug) {
                    GD.Print($"Releasing viewer (type: {typeof(T)}, assigned: {AssignedViewers.Count}, unassigned: {UnassignedViewers.Count})");
                }

                //                viewer.ResetViewer();

                Instance.RemoveChild(viewer);

                AssignedViewers.Remove(viewer);
                UnassignedViewers.Enqueue(viewer);
            }
        }

        [Export]
        private PackedScene _viewerScene;

        [Export]
        private float _viewportEpsilon = 0.005f;

        public float ViewportEpsilon => _viewportEpsilon;

        #region Debug

        [Export]
        private bool _enableDebug;

        public bool EnableDebug => _enableDebug;

        #endregion

        private readonly ViewerSet _viewers = new ViewerSet();

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();
        }

        #endregion

        public void InstanceViewers(int count)
        {
            _viewers.InstanceViewers(count, _viewerScene);

            ResizeViewports();
        }

        #region Acquire

        public T AcquireViewer<T>() where T : Viewer
        {
            return _viewers.AcquireViewer<T>();
        }

        public void ReleaseViewer<T>(T viewer) where T : Viewer
        {
            _viewers.ReleaseViewer(viewer);
        }

        #endregion

        public void ResizeViewports()
        {
            if(_viewers.AssignedViewers.Count > 0) {
                ResizeViewports(_viewers.AssignedViewers);
            } else if(_viewers.UnassignedViewers.Count > 0) {
                ResizeViewports(_viewers.UnassignedViewers);
            }
        }

        private void ResizeViewports(IReadOnlyCollection<Viewer> viewers)
        {
            int gridCols = Mathf.CeilToInt(Mathf.Sqrt(viewers.Count));
            int gridRows = gridCols;

            // remove any extra full colums
            int extraCols = (gridCols * gridRows) - viewers.Count;
            gridCols -= extraCols / gridRows;

            float viewportWidth = 1.0f / gridCols;
            float viewportHeight = 1.0f / gridRows;

            GD.Print($"[ViewerManager] Resizing {viewers.Count} viewports, Grid Size: {gridCols}x{gridRows} Viewport Size: {viewportWidth}x{viewportHeight}");

            for(int row = 0; row < gridRows; ++row) {
                for(int col = 0; col < gridCols; ++col) {
                    int viewerIdx = (row * gridCols) + col;
                    if(viewerIdx >= viewers.Count) {
                        break;
                    }
                    //viewers.ElementAt(viewerIdx).SetViewport(col, gridRows - 1 - row, viewportWidth, viewportHeight);
                }
            }
        }

    }
}
