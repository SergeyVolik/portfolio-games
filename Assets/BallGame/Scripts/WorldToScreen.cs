using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SV.BallGame
{

    public static class WorldToScreenUtils
    {
        /// <summary>
        /// Convert point from world space to screen space
        /// </summary>
        public static float2 ConvertWorldToScreenCoordinates(this WorldToScreen worldToScreen, float3 point)
        {
            return ConvertWorldToScreenCoordinates(
                point,
                worldToScreen.Position,
                worldToScreen.ProjectionMatrix,
                worldToScreen.Up,
                worldToScreen.Right,
                worldToScreen.Forward,
                worldToScreen.PixelWidth,
                worldToScreen.PixelHeight,
                worldToScreen.ScaleFactor);
        }

        public static bool IsPositionInsideScreen(this WorldToScreen worldToScreen, float3 point)
        {
            var pos = worldToScreen.ConvertWorldToScreenCoordinates(point);

            if (pos.x > 0 && pos.x < worldToScreen.PixelWidth && pos.y < worldToScreen.PixelHeight && pos.y > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Convert point from world space to screen space
        /// </summary>
        /// <param name="point">Point in world space for conversion</param>
        /// <param name="cameraPos">Camera position in world space</param>
        /// <param name="camProjMatrix">Camera projection matrix</param>
        /// <param name="camUp">Camera Up vector in world space</param>
        /// <param name="camRight">Camera Right vector in world space</param>
        /// <param name="camForward">Camera Forward vector in world space</param>
        /// <param name="pixelWidth">Screen pixel width</param>
        /// <param name="pixelHeight">Screen pixel height</param>
        /// <param name="scaleFactor">Canvas scale factor (for position in canvas screen space)</param>
        /// <returns>Screen space coordinates</returns>
        public static float2 ConvertWorldToScreenCoordinates(this float3 point, in float3 cameraPos,
                                                             in float4x4 camProjMatrix,
                                                             in float3 camUp, in float3 camRight, in float3 camForward,
                                                             in float pixelWidth, in float pixelHeight,
                                                             in float scaleFactor)
        {
            /*
            * 1 convert P_world to P_camera
            */
            float4 pointInCameraCoodinates =
                ConvertWorldToCameraCoordinates(point, cameraPos, camUp, camRight, camForward);


            /*
            * 2 convert P_camera to P_clipped
            */
            float4 pointInClipCoordinates = math.mul(camProjMatrix, pointInCameraCoodinates);

            /*
            * 3 convert P_clipped to P_ndc
            * Normalized Device Coordinates
            */
            float4 pointInNdc = pointInClipCoordinates / pointInClipCoordinates.w;


            /*
            * 4 convert P_ndc to P_screen
            */
            float2 pointInScreenCoordinates;
            pointInScreenCoordinates.x = pixelWidth / 2.0f * (pointInNdc.x + 1);
            pointInScreenCoordinates.y = pixelHeight / 2.0f * (pointInNdc.y + 1);


            // return screencoordinates
            return pointInScreenCoordinates / scaleFactor;
        }

        private static float4 ConvertWorldToCameraCoordinates(in float3 point, in float3 cameraPos, in float3 camUp,
                                                              in float3 camRight, in float3 camForward)
        {
            // translate the point by the negative camera-offset
            //and convert to Vector4
            float4 translatedPoint = new float4(point - cameraPos, 1f);

            // create transformation matrix
            float4x4 transformationMatrix = float4x4.identity;
            transformationMatrix.c0 = new float4(camRight.x, camUp.x, -camForward.x, 0);
            transformationMatrix.c1 = new float4(camRight.y, camUp.y, -camForward.y, 0);
            transformationMatrix.c2 = new float4(camRight.z, camUp.z, -camForward.z, 0);

            float4 transformedPoint = math.mul(transformationMatrix, translatedPoint);

            return transformedPoint;
        }
    }

    /// <summary>
    /// Data for convert world to screen space.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WorldToScreen
    {
        /// <summary>
        /// Camera projection matrix.
        /// </summary>
        public float4x4 ProjectionMatrix;

        /// <summary>
        /// Camera position.
        /// </summary>
        public float3 Position;

        /// <summary>
        /// Camera up vector.
        /// </summary>
        public float3 Up;

        /// <summary>
        /// Camera right vector.
        /// </summary>
        public float3 Right;

        /// <summary>
        /// Camera forward.
        /// </summary>
        public float3 Forward;

        /// <summary>
        /// Camera view pixel width.
        /// </summary>
        public float PixelWidth;

        /// <summary>
        /// Camera view pixel height.
        /// </summary>
        public float PixelHeight;

        /// <summary>
        /// View scale factor.
        /// </summary>
        public float ScaleFactor;

        /// <summary>
        /// Get world to screen convert data from camera.
        /// </summary>
        public static WorldToScreen Create(Camera camera, float scaleFactor = 1)
        {
            var camTr = camera.transform;
            WorldToScreen worldToScreen = new WorldToScreen
            {
                ProjectionMatrix = camera.projectionMatrix,
                Position = camTr.position,
                Up = camTr.up,
                Right = camTr.right,
                Forward = camTr.forward,
                PixelWidth = camera.pixelWidth,
                PixelHeight = camera.pixelHeight,
                ScaleFactor = scaleFactor,
            };

            return worldToScreen;
        }

    }


    public struct WorldToScreenSingleton : IComponentData
    {
        public WorldToScreen Value;
    }


    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class WorldToScreenStructSystem : SystemBase
    {
        private Camera cam;

        protected override void OnCreate()
        {
            base.OnCreate();
            
            EntityManager.AddComponent<WorldToScreenSingleton>(this.SystemHandle);

            cam = Camera.main;
        }
        protected override void OnUpdate()
        {
            if(cam == null)
                cam = Camera.main;

            if (cam == null)
                return;

            var data = SystemAPI.GetSingletonRW<WorldToScreenSingleton>();

            data.ValueRW.Value = WorldToScreen.Create(cam);
        }
    }
}
