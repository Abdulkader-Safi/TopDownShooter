using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TopDownShooter.Game.Rendering;

public static class DebugDraw
{
    private static BasicEffect _effect;
    private static List<VertexPositionColor> _lines = new List<VertexPositionColor>();
    private static bool _isEnabled = false;
    
    public static bool IsEnabled 
    { 
        get => _isEnabled; 
        set => _isEnabled = value; 
    }
    
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
        if (!_isEnabled) return;
        
        _lines.Add(new VertexPositionColor(start, color));
        _lines.Add(new VertexPositionColor(end, color));
    }
    
    public static void DrawBox(BoundingBox box, Color color)
    {
        if (!_isEnabled) return;
        
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
        if (!_isEnabled) return;
        
        // Draw cylinder part
        int segments = 16; // More segments for smoother appearance
        for (int i = 0; i < segments; i++)
        {
            float angle1 = (float)i / segments * MathHelper.TwoPi;
            float angle2 = (float)(i + 1) / segments * MathHelper.TwoPi;
            
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
        if (!_isEnabled) return;
        
        // Draw multiple concentric capsules to create thick outline effect
        for (int layer = 0; layer < 3; layer++)
        {
            float layerRadius = radius + (layer * outlineThickness);
            float layerHeight = halfHeight + (layer * outlineThickness * 0.5f);
            
            int segments = 20; // High detail for outline
            for (int i = 0; i < segments; i++)
            {
                float angle1 = (float)i / segments * MathHelper.TwoPi;
                float angle2 = (float)(i + 1) / segments * MathHelper.TwoPi;
                
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
        int hemisphereSegments = 8;
        
        // Top hemisphere
        for (int h = 1; h <= hemisphereSegments; h++)
        {
            float heightRatio = (float)h / hemisphereSegments;
            float sphereRadius = radius * (float)Math.Sin(Math.PI * 0.5 * (1.0 - heightRatio));
            float sphereHeight = halfHeight + radius * heightRatio;
            
            // Draw circle at this height
            for (int i = 0; i < 12; i++)
            {
                float angle1 = (float)i / 12 * MathHelper.TwoPi;
                float angle2 = (float)(i + 1) / 12 * MathHelper.TwoPi;
                
                var p1 = position + new Vector3((float)Math.Cos(angle1) * sphereRadius, sphereHeight, (float)Math.Sin(angle1) * sphereRadius);
                var p2 = position + new Vector3((float)Math.Cos(angle2) * sphereRadius, sphereHeight, (float)Math.Sin(angle2) * sphereRadius);
                
                DrawLine(p1, p2, color);
            }
        }
        
        // Bottom hemisphere
        for (int h = 1; h <= hemisphereSegments; h++)
        {
            float heightRatio = (float)h / hemisphereSegments;
            float sphereRadius = radius * (float)Math.Sin(Math.PI * 0.5 * (1.0 - heightRatio));
            float sphereHeight = -halfHeight - radius * heightRatio;
            
            // Draw circle at this height
            for (int i = 0; i < 12; i++)
            {
                float angle1 = (float)i / 12 * MathHelper.TwoPi;
                float angle2 = (float)(i + 1) / 12 * MathHelper.TwoPi;
                
                var p1 = position + new Vector3((float)Math.Cos(angle1) * sphereRadius, sphereHeight, (float)Math.Sin(angle1) * sphereRadius);
                var p2 = position + new Vector3((float)Math.Cos(angle2) * sphereRadius, sphereHeight, (float)Math.Sin(angle2) * sphereRadius);
                
                DrawLine(p1, p2, color);
            }
        }
    }
    
    public static void DrawSphere(Vector3 center, float radius, Color color)
    {
        if (!_isEnabled) return;
        
        int segments = 12;
        
        // Draw three circles (XY, XZ, YZ planes)
        for (int i = 0; i < segments; i++)
        {
            float angle1 = (float)i / segments * MathHelper.TwoPi;
            float angle2 = (float)(i + 1) / segments * MathHelper.TwoPi;
            
            // XY plane
            var p1XY = center + new Vector3((float)Math.Cos(angle1) * radius, (float)Math.Sin(angle1) * radius, 0);
            var p2XY = center + new Vector3((float)Math.Cos(angle2) * radius, (float)Math.Sin(angle2) * radius, 0);
            DrawLine(p1XY, p2XY, color);
            
            // XZ plane  
            var p1XZ = center + new Vector3((float)Math.Cos(angle1) * radius, 0, (float)Math.Sin(angle1) * radius);
            var p2XZ = center + new Vector3((float)Math.Cos(angle2) * radius, 0, (float)Math.Sin(angle2) * radius);
            DrawLine(p1XZ, p2XZ, color);
            
            // YZ plane
            var p1YZ = center + new Vector3(0, (float)Math.Cos(angle1) * radius, (float)Math.Sin(angle1) * radius);
            var p2YZ = center + new Vector3(0, (float)Math.Cos(angle2) * radius, (float)Math.Sin(angle2) * radius);
            DrawLine(p1YZ, p2YZ, color);
        }
    }
    
    public static void Render(GraphicsDevice device, Matrix view, Matrix projection)
    {
        if (!_isEnabled || _lines.Count == 0) return;
        
        if (_effect == null)
            Initialize(device);
        
        _effect.World = Matrix.Identity;
        _effect.View = view;
        _effect.Projection = projection;
        
        device.DepthStencilState = DepthStencilState.Default;
        device.RasterizerState = RasterizerState.CullNone;
        device.BlendState = BlendState.AlphaBlend;
        
        foreach (var pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            
            if (_lines.Count >= 2)
            {
                device.DrawUserPrimitives(
                    PrimitiveType.LineList,
                    _lines.ToArray(),
                    0,
                    _lines.Count / 2);
            }
        }
        
        _lines.Clear();
    }
}