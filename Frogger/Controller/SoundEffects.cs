using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace Frogger.Controller
{
    /// <summary>
    ///     Sound effects of the game
    /// </summary>
    public class SoundEffects
    {
        #region Data members

        private MediaPlayer player = new MediaPlayer();

        #endregion

        #region Methods

        /// <summary>
        ///     Games the over sound.
        /// </summary>
        public async Task GameOverSound()
        {
            var soundsFolder =
                await Package.Current.InstalledLocation.GetFolderAsync("Sounds");
            var soundFile = await soundsFolder.GetFileAsync("frogger-gameOver-sound.wav");
            var mediaSource = MediaSource.CreateFromStorageFile(soundFile);
            this.player.Source = mediaSource;
            this.player.Play();
        }

        /// <summary>
        ///     Landings the home sounds.
        /// </summary>
        public async Task LandingHomeSounds()
        {
            this.player = new MediaPlayer();
            var soundsFolder =
                await Package.Current.InstalledLocation.GetFolderAsync("Sounds");
            var soundFile = await soundsFolder.GetFileAsync("frogger-homeLanding-sound.wav");
            var mediaSource = MediaSource.CreateFromStorageFile(soundFile);
            this.player.Source = mediaSource;
            this.player.Play();
        }

        /// <summary>
        ///     Dyings the sound.
        /// </summary>
        public async Task DyingSound()
        {
            var soundsFolder =
                await Package.Current.InstalledLocation.GetFolderAsync("Sounds");
            var soundFile = await soundsFolder.GetFileAsync("frogger-dying-sound.wav");
            var mediaSource = MediaSource.CreateFromStorageFile(soundFile);
            this.player.Source = mediaSource;
            this.player.Play();
        }

        /// <summary>
        ///     Levels up sound.
        /// </summary>
        public async Task LevelUpSound()
        {
            var soundsFolder =
                await Package.Current.InstalledLocation.GetFolderAsync("Sounds");
            var soundFile = await soundsFolder.GetFileAsync("frogger-levelUp-sound.wav");
            var mediaSource = MediaSource.CreateFromStorageFile(soundFile);
            this.player.Source = mediaSource;
            this.player.Play();
        }

        /// <summary>
        ///     Powers up sound.
        /// </summary>
        public async Task PowerUpSound()
        {
            var soundsFolder =
                await Package.Current.InstalledLocation.GetFolderAsync("Sounds");
            var soundFile = await soundsFolder.GetFileAsync("frogger-powerUp-sound.wav");
            var mediaSource = MediaSource.CreateFromStorageFile(soundFile);
            this.player.Source = mediaSource;
            this.player.Play();
        }

        #endregion
    }
}