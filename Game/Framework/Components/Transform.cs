using Microsoft.Xna.Framework;

namespace TopDownShooter.Game.Framework.Components;

public interface IComponent
{
    Entity Entity { get; set; }
}

public interface IUpdateable
{
    void Update();
}

public interface IDrawable
{
    void Draw();
}

public class Transform : IComponent
{
    public Entity Entity { get; set; }

    public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }
    private Vector3 Scale { get; set; } = Vector3.One;

    public Vector3 Forward => Vector3.Transform(-Vector3.UnitZ, Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z));
    public Vector3 Right => Vector3.Transform(Vector3.UnitX, Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z));
    public Vector3 Up => Vector3.Transform(Vector3.UnitY, Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z));

    public Matrix WorldMatrix => Matrix.CreateScale(Scale) *
                                Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z) *
                                Matrix.CreateTranslation(Position);
}