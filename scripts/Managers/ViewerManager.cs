using Godot;

using System.Collections.Generic;

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

                Instance.RemoveChild(viewer);

                AssignedViewers.Remove(viewer);
                UnassignedViewers.Enqueue(viewer);
            }
        }

        [Export]
        private PackedScene _viewerScene;

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
        }

        #region Acquire

        public T AcquireViewer<T>() where T : Viewer
        {
            return _viewers.AcquireViewer<T>();
        }

        public void ReleaseViewer<T>(T viewer) where T : Viewer
        {
            viewer.OnRelease();

            _viewers.ReleaseViewer(viewer);
        }

        #endregion
    }
}
