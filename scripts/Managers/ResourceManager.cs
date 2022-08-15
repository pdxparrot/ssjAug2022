using Godot;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022.Managers
{
    public class ResourceManager : SingletonNode<ResourceManager>
    {
        private class Notifier
        {
            public event EventHandler<EventArgs> ProgressEvent;

            public event EventHandler<EventArgs> SuccessEvent;

            public event EventHandler<EventArgs> FailureEvent;

            public void Progress(ResourceManager sender, float progress)
            {
                ProgressEvent?.Invoke(sender, EventArgs.Empty);
            }

            public void Success(ResourceManager sender, Resource resource)
            {
                SuccessEvent?.Invoke(sender, EventArgs.Empty);
            }

            public void Failure(ResourceManager sender, Error err)
            {
                FailureEvent?.Invoke(sender, EventArgs.Empty);
            }
        }

        private readonly Dictionary<string, Notifier> _loadingSet = new Dictionary<string, Notifier>();

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();
        }

        #endregion

        public async Task LoadResourceAsync<T>(string path, EventHandler<EventArgs> onSuccess = null, EventHandler<EventArgs> onFailure = null, EventHandler<EventArgs> onProgress = null) where T : Resource
        {
            // if we're already loading this resource
            // just sign up for notifications
            if(_loadingSet.TryGetValue(path, out Notifier notifier)) {
                notifier.SuccessEvent += onSuccess;
                notifier.FailureEvent += onFailure;
                notifier.ProgressEvent += onProgress;
                return;
            }

            notifier = new Notifier();
            _loadingSet[path] = notifier;

            using(var loader = ResourceLoader.LoadInteractive(path)) {
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
