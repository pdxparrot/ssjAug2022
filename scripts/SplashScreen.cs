using Godot;

using System.Threading.Tasks;

using pdxpartyparrot.ssjAug2022.Managers;

namespace pdxpartyparrot.ssjAug2022
{
    public class SplashScreen : Node
    {
        [Export]
        private Image[] _splashImages = new Image[0];

        private TextureRect _splashImage;

        private Timer _fadeTimer;

        private Timer _displayTimer;

        private int _currentSplashImage = 0;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _splashImage = GetNode<TextureRect>("Canvas/CanvasLayer/Splash Image");

            _fadeTimer = GetNode<Timer>("Timers/Fade Timer");
            _displayTimer = GetNode<Timer>("Timers/Timer");
        }

        public override async void _Process(float delta)
        {
            if(_fadeTimer.IsStopped() && _displayTimer.IsStopped()) {
                await ShowNextSplashImageAsync().ConfigureAwait(false);
                return;
            }
        }

        #endregion

        private async Task ShowNextSplashImageAsync()
        {
            if(_currentSplashImage >= _splashImages.Length) {
                QueueFree();

                await SceneManager.Instance.LoadMainMenuAsync().ConfigureAwait(false);
            }

            _currentSplashImage++;
        }
    }
}
