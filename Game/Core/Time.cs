using Microsoft.Xna.Framework;

namespace TopDownShooter.Game.Core;

public static class Time
{
    public static float Delta { get; private set; }
    public static float Total { get; private set; }
    public static float DeltaFixed => 1f / 60f;

    public static void Update(GameTime gameTime)
    {
        Delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Total = (float)gameTime.TotalGameTime.TotalSeconds;
    }
}