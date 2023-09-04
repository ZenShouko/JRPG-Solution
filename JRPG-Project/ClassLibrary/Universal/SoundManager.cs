using System;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Media;

namespace JRPG_Project.ClassLibrary.Universal
{
    /// <summary>
    /// Handles all sound effects and music.
    /// </summary>
    public static class SoundManager
    {
        private static MediaPlayer effectsPlayer = new MediaPlayer();
        private static MediaPlayer musicPlayer = new MediaPlayer();

        public static void PlaySound(string soundName)
        {
            if (!File.Exists(Path.Combine(Environment.CurrentDirectory, "Resources/Sfx/Effects/" + soundName)))
            {
                throw new FileNotFoundException();
            }

            effectsPlayer.Open(new Uri(Path.Combine(Environment.CurrentDirectory, "Resources/Sfx/Effects/", soundName)));
            effectsPlayer.Play();
        }
    }
}
