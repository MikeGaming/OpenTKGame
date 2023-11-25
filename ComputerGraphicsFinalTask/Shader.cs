using OpenTK.Graphics.OpenGL4;

namespace ComputerGraphicsFinalTask;

public class Shader : IDisposable
{
    public readonly int Handle;
    private bool _disposedValue;
    

    public Shader(string vertPath, string fragPath)
    {
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader,  File.ReadAllText(StaticUtilities.ShaderDirectory+vertPath));
        
        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader,  File.ReadAllText(StaticUtilities.ShaderDirectory+fragPath));
        
        GL.CompileShader(vertexShader);

        int successState;
        
        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out successState);

        if (successState == 0)
        {
            Console.WriteLine("Error in vertex: " + GL.GetShaderInfoLog(vertexShader));
        }
        
        GL.CompileShader(fragmentShader);
        GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out successState);

        if (successState == 0)
        {
            Console.WriteLine("Error in Fragment: " + GL.GetShaderInfoLog(fragmentShader));
        }

        //Bind the shaders
        Handle = GL.CreateProgram();
        
        GL.AttachShader(Handle, vertexShader);
        GL.AttachShader(Handle, fragmentShader);
        
        GL.LinkProgram(Handle);
        
        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out successState);

        if (successState == 0)
        {
            Console.WriteLine("Error in handle: " + GL.GetProgramInfoLog(Handle));
        }
        
        //Clean up GPU
        GL.DetachShader(Handle, vertexShader);
        GL.DetachShader(Handle, fragmentShader);
        
        GL.DeleteShader(fragmentShader);
        GL.DeleteShader(vertexShader);
    }

    ~Shader()
    {
        if (!_disposedValue)
        {
            Console.WriteLine("GPU resource leak... Oops");
        }
    }

    public void Use()
    {
        GL.UseProgram(Handle);
    }

    public int GetAttribLocation(string attributeName)
    {
        return GL.GetAttribLocation(Handle, attributeName);
    }

    public int GetUniformLocation(string attributeName)
    {
        return GL.GetUniformLocation(Handle, attributeName);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            GL.DeleteProgram(Handle);
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
