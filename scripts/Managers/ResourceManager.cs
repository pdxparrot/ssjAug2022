using Godot;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022.Managers
{
    public class ResourceManager : SingletonNode<ResourceManager>
    {
        public class ProgressEventArgs : EventArgs
        {
            public float Progress { get; set; }
        }

        public class SuccessEventArgs : EventArgs
        {
            public Resource Resource { get; set; }
        }

        public class FailureEventArgs : EventArgs
        {
            public Error Error { get; set; }
        }


        private class Notifier
        {
            public event EventHandler<ProgressEventArgs> ProgressEvent;

            public event EventHandler<SuccessEventArgs> SuccessEvent;

            public event EventHandler<FailureEventArgs> FailureEvent;

            public void Progress(ResourceManager sender, float progress)
            {
                ProgressEvent?.Invoke(sender, new ProgressEventArgs {
                    Progress = progress,
                });
            }

            public void Success(ResourceManager sender, Resource resource)
            {
                SuccessEvent?.Invoke(sender, new SuccessEventArgs {
                    Resource = resource,
                });
            }

            public void Failure(ResourceManager sender, Error err)
            {
                FailureEvent?.Invoke(sender, new FailureEventArgs {
                    Error = err,
                });
            }
        }

        private readonly Dictionary<string, Notifier> _loadingSet = new Dictionary<string, Notifier>();

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();
        }

        #endregion

        public async Task LoadResourceAsync(string path, EventHandler<SuccessEventArgs> onSuccess = null, EventHandler<FailureEventArgs> onFailure = null, EventHandler<ProgressEventArgs> onProgress = null)
        {
            // if we're already loading this resource
            // just subscribe for notifications
            if(_loadingSet.TryGetValue(path, out Notifier notifier)) {
                notifier.SuccessEvent += onSuccess;
                notifier.FailureEvent += onFailure;
                notifier.ProgressEvent += onProgress;
                return;
            }

            notifier = new Notifier();
            notifier.SuccessEvent += onSuccess;
            notifier.FailureEvent += onFailure;
            notifier.ProgressEvent += onProgress;

            _loadingSet[path] = notifier;

            GD.Print($"[ResourceManager] Loading resource {path}...");
            using(var loader = ResourceLoader.LoadInteractive(path)) {
                GD.Print($"[ResourceManager] {loader.GetStageCount()} stage(s)");

                notifier.Progress(this, 0.0f);

                Error err;
                while(true) {
                    err = loader.Poll();
                    if(err != Error.Ok) {
                        break;
                    }

                    notifier.Progress(this, loader.GetStage() / (float)loader.GetStageCount());
                    await ToSignal(GetTree(), "idle_frame");
                }

                if(err == Error.FileEof) {
                    notifier.Progress(this, 1.0f);
                    notifier.Success(this, loader.GetResource());
                } else {
                    notifier.Failure(this, err);
                }
            }

            _loadingSet.Remove(path);
        }
    }
}
