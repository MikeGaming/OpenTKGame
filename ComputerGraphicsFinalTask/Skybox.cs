
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace ComputerGraphicsFinalTask;

public class Skybox
{
    private readonly int _vertexArrayObject;
    private readonly int _vertexBufferObject;
    
    public readonly Shader MyShader;

    private Matrix4 _view;

    public Skybox(float[] vertices, Shader shader)
    {
        MyShader = shader;
        
        //VBO
        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
        StaticUtilities.CheckError("2");
        
        //VAO
        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);
        StaticUtilities.CheckError("3");
        
        int id = 0;
        GL.VertexAttribPointer(id, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(id);
    }

    public void Render()
    {
        
        MyShader.Use();
        
        _view = new Matrix4(new Matrix3(Game.GameCam.GetViewMatrix()));
        
        int id = MyShader.GetUniformLocation("view");
        GL.UniformMatrix4(id, true, ref _view);
        id = MyShader.GetUniformLocation("projection");
        GL.UniformMatrix4(id, true, ref Game.Projection);
        
        GL.BindVertexArray(_vertexArrayObject);
        GL.ActiveTexture(TextureUnit.Texture7);
        GL.BindTexture(TextureTarget.TextureCubeMap, Game._skyboxCubemap.Handle);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        
        GL.BindVertexArray(0);
        
    }
    
    public void Dispose()
    {
        GL.DeleteVertexArray(_vertexArrayObject);
        GL.DeleteBuffer(_vertexBufferObject);
    }
}
