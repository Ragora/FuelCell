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

namespace FuelCell.GUI
{
    /// <summary>
    /// A static manager used to manage the GUI interaction in the game.
    /// </summary>
    public class GUIManager
    {
        /// <summary>
        /// A boolean representing whether or not the GUI debugger is currently enabled.
        /// </summary>
        public static bool GUIDebuggerEnabled = false;

        /// <summary>
        /// The game instance that the GUI manager singleton is currently associated with.
        /// </summary>
        private static Microsoft.Xna.Framework.Game InternalGame;

        /// <summary>
        /// The currently active GUI.
        /// </summary>
        private static GUIContext CurrentGUI = null;

        /// <summary>
        /// A dictionary mapping GUI name to their respective GUI instances.
        /// </summary>
        private static SortedDictionary<String, GUIContext> GUIInstances;

        /// <summary>
        /// The currently selected button.
        /// </summary>
        public static Elements.Button SelectedButton;

        /// <summary>
        /// The name of the currently active GUI.
        /// </summary>
        private static string ActiveGUIName;

        /// <summary>
        /// Creates the GUI manager. This should only ever be called once.
        /// </summary>
        public static void Create(Microsoft.Xna.Framework.Game game)
        {
            ActiveGUIName = "<NONE>";
            GUIInstances = new SortedDictionary<String, GUIContext>();
            InternalGame = game;
        }

        /// <summary>
        /// Adds a new GUI to the manager.
        /// </summary>
        /// <param name="gui">
        /// The GUI instance to store.
        /// </param>
        /// <param name="name">
        /// The name of the GUI.
        /// </param>
        public static void AddGUI(GUIContext gui, String name)
        {
            GUIInstances[name] = gui;
        }

        /// <summary>
        /// Sets a GUI to be active by its name.
        /// </summary>
        /// <param name="name">
        /// The name of the GUI to become active.
        /// </param>
        public static void SetGUI(String name)
        {
            if (GUIInstances.ContainsKey(name))
            {
                GUIContext newGUI = GUIInstances[name];

                if (newGUI == CurrentGUI)
                    return;

                if (SelectedButton != null)
                    SelectedButton.IsDown = false;

                SelectedButton = null;
                ActiveGUIName = name;

                if (CurrentGUI == null) 
                {
                    CurrentGUI = newGUI;
                    CurrentGUI.OnWake();
                }
                else if (CurrentGUI != newGUI)
                {
                    CurrentGUI.OnSleep();
                    CurrentGUI = newGUI;
                    CurrentGUI.OnWake();
                }
            }
            else if (name == null)
            {
                ActiveGUIName = "<NONE>";
                CurrentGUI = null;
            }
        }

        /// <summary>
        /// Updates the GUI manager by updating the currently active GUI, if there is one.
        /// </summary>
        /// <param name="time">
        /// The GameTime passed in by the game's main update method.
        /// </param>
        public static void Update(GameTime time)
        {
            if (CurrentGUI != null)
                CurrentGUI.Update(time);
        }

        /// <summary>
        /// Draws the currently active GUI to the screen.
        /// </summary>
        /// <param name="batch">
        /// The sprite batch to draw to.
        /// </param>
        public static void Draw(SpriteBatch batch)
        {
            if (CurrentGUI != null)
                CurrentGUI.Draw(batch);

         //   if (GUIManager.GUIDebuggerEnabled)
              //  batch.DrawString(InternalGame.Arial, String.Format("GUI: {0}", ActiveGUIName), new Vector2(630, 20), Color.Red);
        }

        /// <summary>
        /// Initializes the GUI manager by initializing all registered GUI's. This should be called after
        /// registering all of the GUI's that will ever be used.
        /// </summary>
        public static void Initialize()
        {
            foreach (KeyValuePair<String, GUIContext> guiInfo in GUIInstances.ToList())
                guiInfo.Value.Initialize();
        }

        /// <summary>
        /// Helper method for finding the next button on the currently active GUI.
        /// </summary>
        /// <param name="forward">
        /// A boolean representing whether or not we're going forward or backwards.
        /// </param>
        /// <returns>
        /// The next button to be selected.
        /// </returns>
        private static Elements.Button NextButton(bool forward)
        {
            if (CurrentGUI == null || CurrentGUI.Buttons.Count() == 0)
                return null;

            if (SelectedButton == null)
                return CurrentGUI.Buttons[0];

            int lastIndex = CurrentGUI.Buttons.IndexOf(SelectedButton);
            if (forward)
                return CurrentGUI.Buttons[++lastIndex % CurrentGUI.Buttons.Count()];
            else
            {
                --lastIndex;
                lastIndex = lastIndex < 0 ? CurrentGUI.Buttons.Count() + lastIndex : lastIndex;
                return CurrentGUI.Buttons[lastIndex];
            }
        }

        #region Controller Button Responders
        /// <summary>
        /// Listener method to be called when the Directional Pad down is pressed.
        /// </summary>
        /// <param name="pressed">
        /// A boolean representing the current state of the DPad button. It is called once for
        /// each state transition: Up & Down.
        /// </param>
        public static void DPadDownListener(bool pressed)
        {
            if (pressed)
            {
                Elements.Button nextButton = NextButton(false);

                if (nextButton != null && nextButton.Visible)
                {
                    if (SelectedButton != null)
                        SelectedButton.IsDown = false;

                    nextButton.IsDown = true;
                    SelectedButton = nextButton;
                }
            }
        }

        /// <summary>
        /// Listener method to be called when the Directional Pad up is pressed.
        /// </summary>
        /// <param name="pressed">
        /// A boolean representing the current state of the DPad button. It is called once for
        /// each state transition: Up & Down.
        /// </param>
        public static void DPadUpListener(bool pressed)
        {
            if (pressed)
            {
                Elements.Button nextButton = NextButton(true);

                if (nextButton != null && nextButton.Visible)
                {
                    if (SelectedButton != null)
                        SelectedButton.IsDown = false;

                    nextButton.IsDown = true;
                    SelectedButton = nextButton;
                }
            }
        }

        /// <summary>
        /// Listener method to be called when the Directional Pad left is pressed.
        /// </summary>
        /// <param name="pressed">
        /// A boolean representing the current state of the DPad button. It is called once for
        /// each state transition: Up & Down.
        /// </param>
        public static void DPadLeftListener(bool pressed)
        {
            DPadDownListener(pressed);
        }

        /// <summary>
        /// Listener method to be called when the Directional Pad right is pressed.
        /// </summary>
        /// <param name="pressed">
        /// A boolean representing the current state of the DPad button. It is called once for
        /// each state transition: Up & Down.
        /// </param>
        public static void DPadRightListener(bool pressed)
        {
            DPadUpListener(pressed);
        }

        /// <summary>
        /// Listener method to be called when the A button on the controller is pressed.
        /// </summary>
        /// <param name="pressed">
        /// A boolean representing the current state of the A button. It is called once for
        /// each state transition: Up & Down.
        /// </param>
        public static void AButtonListener(bool pressed)
        {
            if (pressed && SelectedButton != null && SelectedButton.Responder != null)
                SelectedButton.Responder(SelectedButton);
        }
        #endregion

        /// <summary>
        /// Helper method that binds all controller listeners for interaction with the GUI.
        /// </summary>
        public static void BindControllerListeners()
        {
            if (SelectedButton != null)
                SelectedButton.IsDown = false;

            SelectedButton = null;

            InputManager.DPadDownListener = DPadDownListener;
            InputManager.DPadUpListener = DPadUpListener;
            InputManager.DPadLeftListener = DPadLeftListener;
            InputManager.DPadRightListener = DPadRightListener;
            InputManager.AButtonListener = AButtonListener;
        }

        /// <summary>
        /// Private constructor to prevent direct construction.
        /// </summary>
        private GUIManager() { }
    }
}
