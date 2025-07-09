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
    }
} 