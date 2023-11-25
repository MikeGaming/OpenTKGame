using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace ComputerGraphicsFinalTask;

public class Cubemap
{
    public readonly int Handle;

    public Cubemap(string[] filePaths)
    {
        Handle = GL.GenTexture();
        GL.BindTexture(TextureTarget.TextureCubeMap, Handle);

        for (int i = 0; i < filePaths.Length; i++)
        {
            using (Stream stream = File.OpenRead((StaticUtilities.TextureDirectory + filePaths[i])))
            {
                ImageResult img = ImageResult.FromStream(stream, ColorComponents.RedGreenBlue);
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgb, img.Width, img.Height, 0,
                    PixelFormat.Rgb, PixelType.UnsignedByte, img.Data);
            }
        }

        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
    }

    public void Use(TextureUnit unit = TextureUnit.Texture0)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.TextureCubeMap, Handle);
    }
    
}