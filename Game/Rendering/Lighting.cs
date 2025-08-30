using Microsoft.Xna.Framework;

namespace TopDownShooter.Game.Rendering;

public static class Lighting
{
    public static Vector3 DirectionalLightDirection = Vector3.Normalize(new Vector3(-0.5f, -1f, -0.5f));
    public static Vector3 DirectionalLightColor = Vector3.One;
    public static Vector3 AmbientColor = new Vector3(0.2f, 0.2f, 0.3f);
}