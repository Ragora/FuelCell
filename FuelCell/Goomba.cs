using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FuelCell
{
    /// <summary>
    /// A class representing the dangerous goombas in the game.
    /// </summary>
    public class Goomba : Model
    {
        public Goomba(Game game) : base(game, "Shapes/Goomba")
        {
            OnCollideResponders.Add(OnGoombaCollide);
        }

        public void OnGoombaCollide(Model model)
        {
            MapManager.Pickups.Remove(this);
            MapManager.DrawnItems.Remove(this);

            ScoreManager.Score -= 400;
            Game.Player.Energy -= 10.0f;

            SoundManager.Play("goomba");
        }
    }
}
