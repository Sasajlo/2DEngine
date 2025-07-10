using System.Runtime.InteropServices;

namespace Engine
{
    public static class EngineCore
    {
        private const string DLL_NAME = "EngineCore.dll";

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool InitializeEngine(int width, int height, [MarshalAs(UnmanagedType.LPUTF8Str)] string title);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ShutdownEngine();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool ShouldClose();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PollEvents();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Render();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetWindowTitle([MarshalAs(UnmanagedType.LPUTF8Str)] string title);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetWindowSize(out int width, out int height);
        
        // Input handling
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool IsKeyPressed(int keyCode);
        
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool IsKeyHeld(int keyCode);
        
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool IsKeyReleased(int keyCode);
        
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern float GetMouseScrollDelta();
        
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ResetMouseScrollDelta();
        
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetMousePosition(out float x, out float y);
        
        // Time management
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void UpdateDeltaTime();
        
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern float GetDeltaTime();
        
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern double GetTime();
        
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ResetTime();
        
        // Sprite rendering
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void RenderSprite(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string texturePath,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 16)] float[] worldMatrix,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] float[] color,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)] float[] size,
            int sortingOrder,
            [MarshalAs(UnmanagedType.I1)] bool flipX,
            [MarshalAs(UnmanagedType.I1)] bool flipY);
            
        // Camera
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCameraMatrices(
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 16)] float[] viewMatrix,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 16)] float[] projectionMatrix);
    }
} 