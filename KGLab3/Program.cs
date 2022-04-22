using KGLab3;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

var nativeWindowSettings = new NativeWindowSettings() {
    Size = new Vector2i(1024, 1024),
    Title = "RayTracing"
};

using var window = new Window(GameWindowSettings.Default, nativeWindowSettings);
window.Run();