﻿using OpenTK.Graphics.OpenGL4;

namespace ComputerGraphicsFinalTask;

public class GameObject
{
    public Transform Transform; // Every gameobject has a transform

    private readonly int _vertexBufferObject;
    private readonly int _vertexArrayObject;
    private readonly int _elementBufferObject;

    private readonly uint[] Indices;
    public readonly Shader MyShader;

    public GameObject(float[] vertices, uint[] indices, Shader shader)
    {
        
        Transform = new Transform();
        
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
        GL.VertexAttribPointer(id, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
        GL.EnableVertexAttribArray(id);
        
        id = MyShader.GetAttribLocation("vertexNormals");
        GL.VertexAttribPointer(id, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(id);
            
        id = MyShader.GetAttribLocation("UVs");
        GL.VertexAttribPointer(id, 2, VertexAttribPointerType.Float,
            false, 8 * sizeof(float), 6 * sizeof(float));
        GL.EnableVertexAttribArray(id);
            
        //EBO
            
        _elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
    }

    public void Render()
    {
        MyShader.Use();
        
        int id = MyShader.GetUniformLocation("model");
        GL.UniformMatrix4(id, true, ref Transform.GetMatrix);
        id = MyShader.GetUniformLocation("view");
        GL.UniformMatrix4(id, true, ref Game.View);
        id = MyShader.GetUniformLocation("projection");
        GL.UniformMatrix4(id, true, ref Game.Projection);
        
     
        id = MyShader.GetUniformLocation("viewPos");
        GL.Uniform3(id, Game.GameCam.Position);
        
        StaticUtilities.CheckError("render");
        
        
        

        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        
        GL.BindVertexArray(0);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(_elementBufferObject);
        GL.DeleteBuffer(_vertexBufferObject);
        GL.DeleteVertexArray(_vertexArrayObject);
    }



}