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
    /// Base class for all GUI's to be used in the game.
    /// </summary>
    public class GUIContext
    {
        /// <summary>
        /// The game this GUI is associated with.
        /// </summary>
        protected Game InternalGame;
        
        /// <summary>
        /// A list of all elements on this GUI.
        /// </summary>
        public List<Elements.Element> Elements;

        /// <summary>
        /// A list of all buttons on this GUI. It is used for cycling them all when using
        /// controller support.
        /// </summary>
        public List<Elements.Button> Buttons;

        /// <summary>
        /// A constructor accepting a game instance.
        /// </summary>
        /// <param name="game">
        /// The game instance to associate this GUI with.
        /// </param>
        public GUIContext(Game game)
        {
            Elements = new List<Elements.Element>();
            Buttons = new List<Elements.Button>();
            InternalGame = game;
        }

        /// <summary>
        /// Base update method that updates all elements contained in the GUI.
        /// </summary>
        /// <param name="time">
        /// The GameTime passed in by the game's main Update method.
        /// </param>
        public virtual void Update(GameTime time)
        {
            foreach (Elements.Element element in Elements)
                element.Update(time);
        }

        /// <summary>
        /// Draws the GUI to the screen.
        /// </summary>
        /// <param name="batch">
        /// The sprite batch to draw to.
        /// </param>
        public virtual void Draw(SpriteBatch batch)
        {
            foreach (Elements.Element element in Elements)
                element.Draw(batch);
        }

        /// <summary>
        /// Adds a new GUI element to this GUI.
        /// </summary>
        /// <param name="element">
        /// The element to add to the GUI.
        /// </param>
        public void AddElement(Elements.Element element)
        {
            Elements.Add(element);

            if (element is Elements.Button)
                Buttons.Add((Elements.Button)element);
        }

        /// <summary>
        /// Removes a GUI element from this GUI.
        /// </summary>
        /// <param name="element">
        /// The element to be removed.
        /// </param>
        public void RemoveElement(Elements.Element element)
        {
            Elements.Remove(element);
        }

        /// <summary>
        /// Base method that is called when the GUI is first set as the active GUI.
        /// </summary>
        public virtual void OnWake()
        {
        }

        /// <summary>
        /// Base method that is called when the GUI is no longer the active GUI.
        /// </summary>
        public virtual void OnSleep()
        {
        }

        /// <summary>
        /// Base method that is called to initialize the GUI and all of its elements.
        /// </summary>
        public virtual void Initialize()
        {
            foreach (Elements.Element element in this.Elements)
                element.Initialize();
        }
    }
}
