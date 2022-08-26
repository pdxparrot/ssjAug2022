using Godot;

using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022.Managers
{
    public class AudioManager : SingletonNode<AudioManager>
    {
        private AudioStreamPlayer _musicAudioStreamPlayer;

        public bool IsMusic1Playing => _musicAudioStreamPlayer.Playing;

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();

            _musicAudioStreamPlayer = GetNode<AudioStreamPlayer>("Music");
        }

        #endregion

        #region Music

        // plays a music clip on the first audio source at no crossfade
        public void PlayMusic(AudioStream musicAudioStream)
        {
            StopAllMusic();

            _musicAudioStreamPlayer.Stream = musicAudioStream;
            _musicAudioStreamPlayer.Play();
        }

        public void StopMusic()
        {
            _musicAudioStreamPlayer.Stop();
        }

        public void StopAllMusic()
        {
            StopMusic();
        }

        #endregion
    }
}
