using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Core.GameSystems;
using Core.Rendering;

namespace Core.Framework.Components;

public class ModelRenderer : IComponent, IDrawable
{
    public Entity Entity { get; set; }
    public Color Color { get; set; } = Color.White;
    public PrimitiveType PrimitiveType { get; set; } = PrimitiveType.Box;
    public Vector3 Size { get; set; } = Vector3.One;

    private static BasicEffect _effect;
    private static VertexBuffer _boxVertexBuffer;
    private static IndexBuffer _boxIndexBuffer;
    private static int _boxIndexCount;

    public static void InitializeSharedResources(GraphicsDevice device)
    {
        _effect = new BasicEffect(device)
        {
            VertexColorEnabled = true,
            LightingEnabled = false
        };

        CreateBoxGeometry(device);
    }

    private static void CreateBoxGeometry(GraphicsDevice device)
    {
        var vertices = new VertexPositionColor[24];
        var indices = new ushort[36];

        // Box vertices (6 faces, 4 vertices each)
        var positions = new Vector3[]
        {
            // Front face
            new Vector3(-0.5f, -0.5f,  0.5f), new Vector3( 0.5f, -0.5f,  0.5f),
            new Vector3( 0.5f,  0.5f,  0.5f), new Vector3(-0.5f,  0.5f,  0.5f),
            // Back face  
            new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-0.5f,  0.5f, -0.5f),
            new Vector3( 0.5f,  0.5f, -0.5f), new Vector3( 0.5f, -0.5f, -0.5f),
            // Top face
            new Vector3(-0.5f,  0.5f, -0.5f), new Vector3(-0.5f,  0.5f,  0.5f),
            new Vector3( 0.5f,  0.5f,  0.5f), new Vector3( 0.5f,  0.5f, -0.5f),
            // Bottom face
            new Vector3(-0.5f, -0.5f, -0.5f), new Vector3( 0.5f, -0.5f, -0.5f),
            new Vector3( 0.5f, -0.5f,  0.5f), new Vector3(-0.5f, -0.5f,  0.5f),
            // Right face
            new Vector3( 0.5f, -0.5f, -0.5f), new Vector3( 0.5f,  0.5f, -0.5f),
            new Vector3( 0.5f,  0.5f,  0.5f), new Vector3( 0.5f, -0.5f,  0.5f),
            // Left face
            new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-0.5f, -0.5f,  0.5f),
            new Vector3(-0.5f,  0.5f,  0.5f), new Vector3(-0.5f,  0.5f, -0.5f)
        };

        var normals = new Vector3[]
        {
            Vector3.Forward, Vector3.Forward, Vector3.Forward, Vector3.Forward,
            Vector3.Backward, Vector3.Backward, Vector3.Backward, Vector3.Backward,
            Vector3.Up, Vector3.Up, Vector3.Up, Vector3.Up,
            Vector3.Down, Vector3.Down, Vector3.Down, Vector3.Down,
            Vector3.Right, Vector3.Right, Vector3.Right, Vector3.Right,
            Vector3.Left, Vector3.Left, Vector3.Left, Vector3.Left
        };

        for (var i = 0; i < 24; i++)
        {
            vertices[i] = new VertexPositionColor(positions[i], Color.White);
        }

        // Box indices (2 triangles per face)
        var boxIndices = new ushort[]
        {
            0,1,2, 0,2,3,   // Front
            4,5,6, 4,6,7,   // Back
            8,9,10, 8,10,11, // Top
            12,13,14, 12,14,15, // Bottom
            16,17,18, 16,18,19, // Right
            20,21,22, 20,22,23  // Left
        };

        Array.Copy(boxIndices, indices, boxIndices.Length);
        _boxIndexCount = boxIndices.Length;

        _boxVertexBuffer = new VertexBuffer(device, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
        _boxVertexBuffer.SetData(vertices);

        _boxIndexBuffer = new IndexBuffer(device, typeof(ushort), indices.Length, BufferUsage.WriteOnly);
        _boxIndexBuffer.SetData(indices);
    }

    public void Draw()
    {
        var device = GameRoot.Instance.GraphicsDevice;
        var camera = Camera.Current;

        if (camera == null) return;

        if (_effect == null)
            InitializeSharedResources(device);

        var world = Matrix.CreateScale(Size) * Entity.Transform.WorldMatrix;

        if (_effect == null) return;
        _effect.World = world;
        _effect.View = camera.View;
        _effect.Projection = camera.Projection;
        _effect.DiffuseColor = Color.ToVector3();

        device.SetVertexBuffer(_boxVertexBuffer);
        device.Indices = _boxIndexBuffer;

        foreach (var pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            device.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0,
                _boxIndexCount / 3);
        }
    }
}

public enum PrimitiveType
{
    Box,
    Quad,
    Sphere
}