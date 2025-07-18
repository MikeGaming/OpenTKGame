using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace ComputerGraphicsFinalTask;

public class ScreenSpaceObject
{
    public Transform Transform; // Every gameobject has a transform

    private readonly int _vertexBufferObject;
    private readonly int _vertexArrayObject;
    private readonly int _elementBufferObject;
    private readonly int _frameBufferObject;

    private int Width, Height;

    private int texture;
    private int depthTexture;

    private readonly uint[] Indices;
    public readonly Shader MyShader;

    public ScreenSpaceObject(float[] vertices, uint[] indices, Shader shader, int width, int height)
    {

        Transform = new Transform();

        Width = width;
        Height = height;

        Indices = indices;
        MyShader = shader;
        StaticUtilities.CheckError("1");

        //VBO
        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        //Static draw means not moving
        //DYNAMIC meaning data changes often
        //STREAM data is ALWAYS changing

        //VAO
        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);

        int id = MyShader.GetAttribLocation("vertexPosition");
        GL.VertexAttribPointer(id, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
        GL.EnableVertexAttribArray(id);

        id = MyShader.GetAttribLocation("UVs");
        GL.VertexAttribPointer(id, 2, VertexAttribPointerType.Float,
            false, 4 * sizeof(float), 2 * sizeof(float));
        GL.EnableVertexAttribArray(id);

        //EBO

        _elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        //FBO
        _frameBufferObject = GL.GenFramebuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _frameBufferObject);

        GL.GenTextures(1, out texture);
        GL.ActiveTexture(TextureUnit.Texture31);
        GL.BindTexture(TextureTarget.Texture2D, texture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, Width, Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.BindTexture(TextureTarget.Texture2D, 0);

        GL.GenTextures(1, out depthTexture);
        GL.ActiveTexture(TextureUnit.Texture30);
        GL.BindTexture(TextureTarget.Texture2D, depthTexture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, Width, Height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest); 
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
        float[] borderColor = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);
        GL.BindTexture(TextureTarget.Texture2D, 0);

        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, texture, 0);
        // Attach depth texture as FBO's depth buffer
        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, depthTexture, 0);

        if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferComplete)
            Console.WriteLine("Framebuffer is complete");
        else
            Console.WriteLine("Framebuffer is not complete");

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public void Render()
    {
        
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        GL.ClearColor(1f, 1f, 1f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        MyShader.Use();
        
        StaticUtilities.CheckError("render");
        
        GL.Disable(EnableCap.DepthTest);
        GL.ActiveTexture(TextureUnit.Texture31);
        GL.BindTexture(TextureTarget.Texture2D, texture);
        GL.Uniform1(MyShader.GetUniformLocation("screenTexture"), 31);

        GL.ActiveTexture(TextureUnit.Texture30);
        GL.BindTexture(TextureTarget.Texture2D, depthTexture);
        GL.Uniform1(MyShader.GetUniformLocation("depthTexture"), 30);

        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);

        GL.BindVertexArray(0);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public void BindFBO()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _frameBufferObject);
        GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.Enable(EnableCap.DepthTest);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(_elementBufferObject);
        GL.DeleteBuffer(_vertexBufferObject);
        GL.DeleteVertexArray(_vertexArrayObject);
        GL.DeleteFramebuffer(_frameBufferObject);
    }



}