using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TopDownShooter.Game.Rendering;

public static class DebugDraw
{
    private static BasicEffect _effect;
    private static readonly List<VertexPositionColor> Lines = [];

    public static bool IsEnabled { get; set; }

    public static void Initialize(GraphicsDevice device)
    {
        _effect = new BasicEffect(device)
        {
            VertexColorEnabled = true,
            LightingEnabled = false
        };
    }
    
    public static void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        if (!IsEnabled) return;
        
        Lines.Add(new VertexPositionColor(start, color));
        Lines.Add(new VertexPositionColor(end, color));
    }
    
    public static void DrawBox(BoundingBox box, Color color)
    {
        if (!IsEnabled) return;
        
        var corners = box.GetCorners();
        
        // Bottom face
        DrawLine(corners[0], corners[1], color);
        DrawLine(corners[1], corners[2], color);
        DrawLine(corners[2], corners[3], color);
        DrawLine(corners[3], corners[0], color);
        
        // Top face  
        DrawLine(corners[4], corners[5], color);
        DrawLine(corners[5], corners[6], color);
        DrawLine(corners[6], corners[7], color);
        DrawLine(corners[7], corners[4], color);
        
        // Vertical edges
        DrawLine(corners[0], corners[4], color);
        DrawLine(corners[1], corners[5], color);
        DrawLine(corners[2], corners[6], color);
        DrawLine(corners[3], corners[7], color);
    }
    
    public static void DrawCapsule(Vector3 position, float radius, float halfHeight, Color color)
    {
        if (!IsEnabled) return;
        
        // Draw cylinder part
        const int segments = 16; // More segments for smoother appearance
        for (var i = 0; i < segments; i++)
        {
            var angle1 = (float)i / segments * MathHelper.TwoPi;
            var angle2 = (float)(i + 1) / segments * MathHelper.TwoPi;
            
            var p1Top = position + new Vector3((float)Math.Cos(angle1) * radius, halfHeight, (float)Math.Sin(angle1) * radius);
            var p2Top = position + new Vector3((float)Math.Cos(angle2) * radius, halfHeight, (float)Math.Sin(angle2) * radius);
            var p1Bot = position + new Vector3((float)Math.Cos(angle1) * radius, -halfHeight, (float)Math.Sin(angle1) * radius);
            var p2Bot = position + new Vector3((float)Math.Cos(angle2) * radius, -halfHeight, (float)Math.Sin(angle2) * radius);
            
            // Top and bottom circles
            DrawLine(p1Top, p2Top, color);
            DrawLine(p1Bot, p2Bot, color);
            
            // Vertical lines - draw more of them
            if (i % 2 == 0)
                DrawLine(p1Top, p1Bot, color);
        }
        
        // Add hemisphere caps
        DrawCapsuleHemispheres(position, radius, halfHeight, color);
    }
    
    public static void DrawCapsuleOutline(Vector3 position, float radius, float halfHeight, Color color, float outlineThickness = 0.05f)
    {
        if (!IsEnabled) return;
        
        // Draw multiple concentric capsules to create thick outline effect
        for (var layer = 0; layer < 3; layer++)
        {
            var layerRadius = radius + (layer * outlineThickness);
            var layerHeight = halfHeight + (layer * outlineThickness * 0.5f);
            
            const int segments = 20; // High detail for outline
            for (var i = 0; i < segments; i++)
            {
                var angle1 = (float)i / segments * MathHelper.TwoPi;
                var angle2 = (float)(i + 1) / segments * MathHelper.TwoPi;
                
                var p1Top = position + new Vector3((float)Math.Cos(angle1) * layerRadius, layerHeight, (float)Math.Sin(angle1) * layerRadius);
                var p2Top = position + new Vector3((float)Math.Cos(angle2) * layerRadius, layerHeight, (float)Math.Sin(angle2) * layerRadius);
                var p1Bot = position + new Vector3((float)Math.Cos(angle1) * layerRadius, -layerHeight, (float)Math.Sin(angle1) * layerRadius);
                var p2Bot = position + new Vector3((float)Math.Cos(angle2) * layerRadius, -layerHeight, (float)Math.Sin(angle2) * layerRadius);
                
                // Fade color based on layer
                var layerColor = Color.Lerp(color, Color.Transparent, layer * 0.3f);
                
                // Top and bottom circles
                DrawLine(p1Top, p2Top, layerColor);
                DrawLine(p1Bot, p2Bot, layerColor);
                
                // Vertical lines
                DrawLine(p1Top, p1Bot, layerColor);
            }
            
            // Draw hemisphere outlines
            DrawCapsuleHemispheres(position, layerRadius, layerHeight, Color.Lerp(color, Color.Transparent, layer * 0.3f));
        }
    }
    
    private static void DrawCapsuleHemispheres(Vector3 position, float radius, float halfHeight, Color color)
    {
        // Draw hemisphere caps (simplified as circles at different heights)
        const int hemisphereSegments = 8;
        
        // Top hemisphere
        for (var h = 1; h <= hemisphereSegments; h++)
        {
            var heightRatio = (float)h / hemisphereSegments;
            var sphereRadius = radius * (float)Math.Sin(Math.PI * 0.5 * (1.0 - heightRatio));
            var sphereHeight = halfHeight + radius * heightRatio;
            
            // Draw circle at this height
            for (var i = 0; i < 12; i++)
            {
                var angle1 = (float)i / 12 * MathHelper.TwoPi;
                var angle2 = (float)(i + 1) / 12 * MathHelper.TwoPi;
                
                var p1 = position + new Vector3((float)Math.Cos(angle1) * sphereRadius, sphereHeight, (float)Math.Sin(angle1) * sphereRadius);
                var p2 = position + new Vector3((float)Math.Cos(angle2) * sphereRadius, sphereHeight, (float)Math.Sin(angle2) * sphereRadius);
                
                DrawLine(p1, p2, color);
            }
        }
        
        // Bottom hemisphere
        for (var h = 1; h <= hemisphereSegments; h++)
        {
            var heightRatio = (float)h / hemisphereSegments;
            var sphereRadius = radius * (float)Math.Sin(Math.PI * 0.5 * (1.0 - heightRatio));
            var sphereHeight = -halfHeight - radius * heightRatio;
            
            // Draw circle at this height
            for (var i = 0; i < 12; i++)
            {
                var angle1 = (float)i / 12 * MathHelper.TwoPi;
                var angle2 = (float)(i + 1) / 12 * MathHelper.TwoPi;
                
                var p1 = position + new Vector3((float)Math.Cos(angle1) * sphereRadius, sphereHeight, (float)Math.Sin(angle1) * sphereRadius);
                var p2 = position + new Vector3((float)Math.Cos(angle2) * sphereRadius, sphereHeight, (float)Math.Sin(angle2) * sphereRadius);
                
                DrawLine(p1, p2, color);
            }
        }
    }
    
    public static void DrawSphere(Vector3 center, float radius, Color color)
    {
        if (!IsEnabled) return;
        
        const int segments = 12;
        
        // Draw three circles (XY, XZ, YZ planes)
        for (var i = 0; i < segments; i++)
        {
            var angle1 = (float)i / segments * MathHelper.TwoPi;
            var angle2 = (float)(i + 1) / segments * MathHelper.TwoPi;
            
            // XY plane
            var p1Xy = center + new Vector3((float)Math.Cos(angle1) * radius, (float)Math.Sin(angle1) * radius, 0);
            var p2Xy = center + new Vector3((float)Math.Cos(angle2) * radius, (float)Math.Sin(angle2) * radius, 0);
            DrawLine(p1Xy, p2Xy, color);
            
            // XZ plane  
            var p1Xz = center + new Vector3((float)Math.Cos(angle1) * radius, 0, (float)Math.Sin(angle1) * radius);
            var p2Xz = center + new Vector3((float)Math.Cos(angle2) * radius, 0, (float)Math.Sin(angle2) * radius);
            DrawLine(p1Xz, p2Xz, color);
            
            // YZ plane
            var p1Yz = center + new Vector3(0, (float)Math.Cos(angle1) * radius, (float)Math.Sin(angle1) * radius);
            var p2Yz = center + new Vector3(0, (float)Math.Cos(angle2) * radius, (float)Math.Sin(angle2) * radius);
            DrawLine(p1Yz, p2Yz, color);
        }
    }
    
    public static void Render(GraphicsDevice device, Matrix view, Matrix projection)
    {
        if (!IsEnabled || Lines.Count == 0) return;
        
        if (_effect == null)
            Initialize(device);

        if (_effect != null)
        {
            _effect.World = Matrix.Identity;
            _effect.View = view;
            _effect.Projection = projection;

            device.DepthStencilState = DepthStencilState.Default;
            device.RasterizerState = RasterizerState.CullNone;
            device.BlendState = BlendState.AlphaBlend;

            foreach (var pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                if (Lines.Count >= 2)
                {
                    device.DrawUserPrimitives(
                        PrimitiveType.LineList,
                        Lines.ToArray(),
                        0,
                        Lines.Count / 2);
                }
            }
        }

        Lines.Clear();
    }
}