using Godot;

using System.Threading.Tasks;

using pdxpartyparrot.ssjAug2022.Managers;
using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022
{
    public class SplashScreen : Node
    {
        [Export]
        private Texture[] _splashImages = new Texture[0];

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

            if(!_fadeTimer.IsStopped()) {
                _splashImage.Modulate = new Color(1.0f, 1.0f, 1.0f, _fadeTimer.TimeLeft / _fadeTimer.WaitTime);
            }
        }

        #endregion

        private async Task ShowNextSplashImageAsync()
        {
            if(_currentSplashImage >= _splashImages.Length) {
                QueueFree();

                await SceneManager.Instance.LoadMainMenuAsync().ConfigureAwait(false);
                return;
            }

            // TODO: this is just a jank hack for ssj2022, please remove it
            if(_currentSplashImage == 1 && _splashImages.Length > 2) {
                _currentSplashImage = 1 + PartyParrotManager.Instance.Random.CoinFlip();
            } else if(_currentSplashImage == 2 && _splashImages.Length > 2) {
                _currentSplashImage++;
                return;
            }

            _splashImage.Texture = _splashImages[_currentSplashImage];
            _splashImage.Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);

            _displayTimer.Start();

            _currentSplashImage++;
        }

        #region Signals

        private void _on_Timer_timeout()
        {
            _fadeTimer.Start();
        }

        private void _on_Fade_Timer_timeout()
        {
            _splashImage.Modulate = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        }

        #endregion
    }
}
