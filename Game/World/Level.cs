using Microsoft.Xna.Framework;
using TopDownShooter.Game.Framework;
using TopDownShooter.Game.Framework.Components;
using TopDownShooter.Game.Physics;

namespace TopDownShooter.Game.World;

public class Level
{
    public static void GenerateArena(Scene scene, CollisionWorld collisionWorld)
    {
        // Create outer walls
        CreateWall(scene, collisionWorld, new Vector3(0, 2.5f, 15), new Vector3(30, 5, 1)); // North
        CreateWall(scene, collisionWorld, new Vector3(0, 2.5f, -15), new Vector3(30, 5, 1)); // South  
        CreateWall(scene, collisionWorld, new Vector3(15, 2.5f, 0), new Vector3(1, 5, 30)); // East
        CreateWall(scene, collisionWorld, new Vector3(-15, 2.5f, 0), new Vector3(1, 5, 30)); // West

        // Create inner obstacles
        CreateWall(scene, collisionWorld, new Vector3(5, 1.5f, 5), new Vector3(3, 3, 3));
        CreateWall(scene, collisionWorld, new Vector3(-5, 1.5f, -5), new Vector3(3, 3, 3));
        CreateWall(scene, collisionWorld, new Vector3(8, 1.5f, -8), new Vector3(2, 3, 4));
        CreateWall(scene, collisionWorld, new Vector3(-8, 1.5f, 8), new Vector3(4, 3, 2));

        // Create level transition trigger (green box)
        var levelTrigger = new LevelTransitionTrigger(collisionWorld, new Vector3(12, 1, 12), new Vector3(2, 2, 2));
        scene.AddEntity(levelTrigger);

        // Create floor with collision
        CreateFloor(scene, collisionWorld, new Vector3(0, 0f, 0), new Vector3(30, 0.1f, 30), Color.Gray);
    }

    private static void CreateWall(Scene scene, CollisionWorld collisionWorld, Vector3 position, Vector3 size)
    {
        // Visual representation
        var wall = new Entity();
        wall.Transform.Position = position;
        var renderer = wall.AddComponent<ModelRenderer>();
        renderer.Color = Color.DarkGray;
        renderer.Size = size;
        scene.AddEntity(wall);

        // Physics representation
        var collider = new AabbCollider
        {
            Position = position,
            Size = size
        };
        collisionWorld.AddStatic(collider);
    }

    private static void CreateFloor(Scene scene, CollisionWorld collisionWorld, Vector3 position, Vector3 size, Color color)
    {
        // Visual representation
        var floor = new Entity();
        floor.Transform.Position = position;
        var renderer = floor.AddComponent<ModelRenderer>();
        renderer.Color = color;
        renderer.Size = size;
        scene.AddEntity(floor);

        // Physics representation
        var collider = new AabbCollider
        {
            Position = position,
            Size = size
        };
        collisionWorld.AddStatic(collider);
    }
}