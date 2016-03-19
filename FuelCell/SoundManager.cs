using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace FuelCell
{
    /// <summary>
    /// A sound manager singleton for audio playback in the game.
    /// </summary>
    public static class SoundManager
    {
        /// <summary>
        /// A boolean representing whether or not the sound debugger is currently enabled.
        /// </summary>
        public static bool SoundDebuggerEnabled = false;

        /// <summary>
        /// The internally tracked game instance.
        /// </summary>
        private static Microsoft.Xna.Framework.Game InternalGame;

        /// <summary>
        /// A dictionary mapping short names to SoundEffects.
        /// </summary>
        private static SortedDictionary<String, SoundEffect> Sounds;

        /// <summary>
        /// The current SoundEffectInstance used for the music.
        /// </summary>
        private static SoundEffectInstance Music;

        /// <summary>
        /// The name of the currently playing music. Is null if none.
        /// </summary>
        public static String MusicName;

        /// <summary>
        /// The internally tracked game volume. This is used for everything except the music.
        /// </summary>
        private static float InternalGameVolume;

        /// <summary>
        /// A list of currently playing sound sources.
        /// </summary>
        private static List<SoundSource> SoundSources;

        /// <summary>
        /// A sound source is a wrapper class around XNA's SoundEffectInstance class types. It is intended
        /// to extend the functionality provided by providing callbacks for events such as ending the sound
        /// playback.
        /// </summary>
        public class SoundSource
        {
            /// <summary>
            /// A delegate type representing a method to be called in response to sound playback ending
            /// for a given sound source.
            /// </summary>
            public delegate void PlaybackEndCallback();

            /// <summary>
            /// The playback end responder to call when sound playback ends. If null, nothing is called.
            /// </summary>
            public PlaybackEndCallback OnPlaybackEndResponder;

            /// <summary>
            /// The bound SoundEffectInstance.
            /// </summary>
            private SoundEffectInstance Handle;

            /// <summary>
            /// A constructor accepting a native XNA SoundEffectInstance.
            /// </summary>
            /// <param name="handle">
            /// The SoundEffectInstance to bind.
            /// </param>
            public SoundSource(SoundEffectInstance handle)
            {
                this.Handle = handle;
            }

            /// <summary>
            /// Updates the sound source by checking if its currently playing and performing looping logic if necessary.
            /// It also calls playback end responders.
            /// </summary>
            public void Update()
            {
                if (!this.Playing)
                {
                    if (this.Looping)
                    {
                        this.Handle.Stop();
                        this.Handle.Play();
                    }

                    if (this.OnPlaybackEndResponder != null)
                        this.OnPlaybackEndResponder();

                    SoundSources.Remove(this);
                }
            }

            /// <summary>
            /// A wrapper read-only property that is used to read the playing state of this
            /// sound source.
            /// </summary>
            public bool Playing
            {
                get
                {
                    return this.Handle.State == SoundState.Playing;
                }
            }

            /// <summary>
            /// A wrapper property that can be used to modify or read the looping state of this
            /// sound source.
            /// </summary>
            public bool Looping
            {
                get
                {
                    return this.Handle.IsLooped;
                }
                set
                {
                    this.Handle.IsLooped = value;
                }
            }
        }

        /// <summary>
        /// A property that controls the current game volume which is clamped between 0.0f and 1.0f, inclusive.
        /// This is used for everything except the music.
        /// </summary>
        public static float GameVolume
        {
            set
            {
                InternalGameVolume = value < 0 || value > 1.0f ? 1.0f : value;
            }

            get
            {
                return InternalGameVolume;
            }
        }

        /// <summary>
        /// The internally tracked music volume. This is used for just the music.
        /// </summary>
        private static float InternalMusicVolume;

        /// <summary>
        /// A property that controls the current music volume which is clamped between 0.0f and 1.0f, inclusive.
        /// This is used for just the music.
        /// </summary>
        public static float MusicVolume
        {
            set
            {
                InternalMusicVolume = value < 0 || value > 1.0f ? 1.0f : value;
            }

            get
            {
                return InternalMusicVolume;
            }
        }

        /// <summary>
        /// Initializes the sound manager.
        /// </summary>
        /// <param name="game">
        /// The game instance to associate with.
        /// </param>
        public static void Create(Microsoft.Xna.Framework.Game game)
        {
            InternalGame = game;

            Sounds = new SortedDictionary<String, SoundEffect>();
            InternalGameVolume = 0.5f;
            InternalMusicVolume = 0.4f;
            SoundSources = new List<SoundSource>();
        }

        /// <summary>
        /// Updates the sound manager.
        /// </summary>
        public static void Update()
        {
            // We can't modify lists in foreach loops.
            List<SoundSource> sources = SoundSources.ToList();

            foreach (SoundSource sound in sources)
                sound.Update();
        }

        /// <summary>
        /// Draws sound debugging information to the screen if sound debugging is enabled.
        /// </summary>
        /// <param name="batch">
        /// The sprite batch to draw to.
        /// </param>
        public static void Draw(SpriteBatch batch)
        {
            if (SoundManager.SoundDebuggerEnabled)
            {
              //  batch.DrawString(InternalGame.Arial, String.Format("Sound Sources: {0}", SoundSources.Count), new Vector2(630, 40), Color.Red);

               // string displayedMusic = MusicName == null ? "<NULL>" : MusicName;
               // batch.DrawString(InternalGame.Arial, String.Format("Music: {0}", displayedMusic), new Vector2(630, 60), Color.Red);

               // string displayedPaused = Music == null ? "<NULL>" : (Music.State == SoundState.Paused).ToString();
               // batch.DrawString(InternalGame.Arial, String.Format("Paused: {0}", displayedPaused), new Vector2(630, 80), Color.Red);
            }
        }

        /// <summary>
        /// Loads a new sound, binding it to the specified shorthand name.
        /// </summary>
        /// <param name="path">
        /// The path to the sound file to load.
        /// </param>
        /// <param name="name">
        /// The name to give this new sound.
        /// </param>
        public static void Load(String path, String name)
        {
            Sounds[name.ToLower()] = InternalGame.Content.Load<SoundEffect>(path);
        }

        /// <summary>
        /// Plays a sound, looking it up by its shorthand name given in Load.
        /// </summary>
        /// <param name="name">
        /// The name of the sound to play.
        /// </param>
        public static SoundSource Play(String name)
        {
            name = name.ToLower();
            if (Sounds.ContainsKey(name))
            {
                SoundEffectInstance sound = Sounds[name].CreateInstance();
                sound.Volume = GameVolume;
                sound.Play();

                SoundSource source = new SoundSource(sound);
                SoundSources.Add(source);

                return source;
            }

            return null;
        }

        #region Music Methods
        /// <summary>
        /// Plays music. This is in a set of methods to manipulate music playback
        /// separately from the rest of the game sounds.
        /// </summary>
        /// <param name="name">
        /// The name of the sound to play as music.
        /// </param>
        public static void PlayMusic(String name)
        {
            name = name.ToLower();
            if (Sounds.ContainsKey(name))
            {
                StopMusic();

                Music = Sounds[name].CreateInstance();
                Music.Volume = MusicVolume;
                Music.IsLooped = true;
                Music.Play();

                MusicName = name;
            }
        }

        /// <summary>
        /// Stops the currently playing music, if anything.
        /// </summary>
        public static void StopMusic()
        {
            if (Music != null)
            {
                Music.Stop();
                Music.Dispose();

                Music = null;
                MusicName = null;
            }
        }

        /// <summary>
        /// Pauses the currently playing music. This allows for music resumes if desired.
        /// </summary>
        /// <param name="paused">
        /// A boolean representing whether or not the currently playing music should be paused.
        /// </param>
        public static void PauseMusic(bool paused)
        {
            if (Music != null)
                if (paused)
                    Music.Pause();
                else
                    Music.Play();
        }
        #endregion
    }
}
