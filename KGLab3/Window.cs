using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace KGLab3; 

public class Window : GameWindow {
    private int _basicProgramId;
    private int _basicVertexShader;
    private int _basicFragmentShader;

    private int _vboPosition;
    private int _vertexArrayObject;
    private int _elementBufferObject;
    private readonly Vector3[] _vertexData = new Vector3[] {
        new Vector3(1f, 1f, 0f),
        new Vector3( 1f, -1f, 0f),
        new Vector3( -1f, -1f, 0f),
        new Vector3(-1f, 1f, 0f) 
    };
    
    private readonly uint[] _indices =
    {
        // Note that indices start at 0!
        0, 1, 3, // The first triangle will be the bottom-right half of the triangle
        1, 2, 3  // Then the second will be the top-right half of the triangle
    };

    private readonly string _title;

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings) {
        _title = nativeWindowSettings.Title;
    }

    protected override void OnLoad() {
        base.OnLoad();

        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
        
        GL.GenBuffers(1, out _vboPosition);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboPosition);
        GL.BufferData<Vector3>(
            BufferTarget.ArrayBuffer, 
            (IntPtr)(_vertexData.Length * Vector3.SizeInBytes), 
            _vertexData,
            BufferUsageHint.StaticDraw
        );
        
        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);
        
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3*sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        _elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);
        
        InitShaders();
        GL.UseProgram(_basicProgramId);
    }

    protected override void OnUnload() {
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
        GL.UseProgram(0);

        // Delete all the resources.
        GL.DeleteBuffer(_vboPosition);
        GL.DeleteVertexArray(_vertexArrayObject);

        GL.DeleteProgram(_basicProgramId);
        
        base.OnUnload();
    }

    protected override void OnUpdateFrame(FrameEventArgs args) {
        base.OnUpdateFrame(args);
        
        #if DEBUG
        Title = _title + $" | FPS: {1.0 / args.Time}";
#endif
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, Size.X, Size.Y);
    }

    protected override void OnRenderFrame(FrameEventArgs args) {
        base.OnRenderFrame(args);
        
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.UseProgram(_basicProgramId);
        
        GL.BindVertexArray(_vertexArrayObject);

        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        SwapBuffers();
    }

    private void InitShaders() {
        _basicProgramId = GL.CreateProgram();
        LoadShader("..\\..\\..\\shader.vert", ShaderType.VertexShader, _basicProgramId,
            out _basicVertexShader);
        LoadShader("..\\..\\..\\shader.frag", ShaderType.FragmentShader, _basicProgramId,
            out _basicFragmentShader);
        GL.LinkProgram(_basicProgramId);

        int status = 0;
        GL.GetProgram(_basicProgramId, GetProgramParameterName.LinkStatus, out status);
        var programInfoLog = GL.GetProgramInfoLog(_basicProgramId);
        Console.WriteLine(programInfoLog);
    }
    
    private void LoadShader(string filename, ShaderType type, int program, out int address) {
        address = GL.CreateShader(type);
        using (var sr = new StreamReader(filename))
        {
            GL.ShaderSource(address, sr.ReadToEnd());
        }
        GL.CompileShader(address);
        GL.AttachShader(program, address);
        Console.WriteLine(GL.GetShaderInfoLog(address));
    }
}