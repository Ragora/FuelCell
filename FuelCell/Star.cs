using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FuelCell
{
    /// <summary>
    /// A class representing the collectible stars in the game.
    /// </summary>
    public class Star : Model
    {
        public Star(Game game) : base(game, "Shapes/star")
        {
            OnCollideResponders.Add(OnStarCollide);
        }

        public override void Update(GameTime time)
        {
            base.Update(time);

            Rotation += new Vector3(1.0f / 60.0f, 0, 0);
            UpdateTransformation();
        }

        public void OnStarCollide(Model model)
        {
            MapManager.Pickups.Remove(this);
            MapManager.DrawnItems.Remove(this);

            ScoreManager.Score += 200;
            Game.Player.Energy += 10.0f;
            Game.Player.Adrenaline += 10.0f;

            SoundManager.Play("star");
        }
    }
}
