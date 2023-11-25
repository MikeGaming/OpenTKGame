using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace ComputerGraphicsFinalTask;

public class Texture
{
    public readonly int Handle;

    public Texture(string filePath)
    {
        //Gen handle ID on blank texture
        Handle = GL.GenTexture();
        
        Use();

        using (Stream stream = File.OpenRead((StaticUtilities.TextureDirectory + filePath)))
        {
            ImageResult img = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, img.Width, img.Height, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte, img.Data);
        }
        
        //filtering
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Filter4Sgis);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Filter4Sgis);
         
        //Wrapping mode
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        
        //mip mapping
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        
    }
    
    public void Use(TextureUnit unit = TextureUnit.Texture0)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, Handle);
    }
    
}