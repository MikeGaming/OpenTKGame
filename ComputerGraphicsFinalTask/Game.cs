using OpenTK.Graphics.OpenGL4;
using Assimp;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ComputerGraphicsFinalTask;

public class Game : GameWindow
{
    //Car Variables
    private Shader _carLitShader;
    private readonly List<Texture> _carModelTextures = new();
    private readonly List<TextureUnit> _carTextureUnits = new();
    private static readonly List<GameObject> CarModelObjects = new();
    private readonly List<int> _carTextureList = new();

    //Anime Girl Variables
    private Shader _animegirlLitShader;
    private readonly List<Texture> _animeGirlTextures = new();
    private readonly List<TextureUnit> _animeGirlTextureUnits = new();
    private static readonly List<GameObject> AnimeGirlObjects = new();
    private readonly List<int> _animeGirlTextureList = new();

    //Skybox Variables
    private Shader _skyboxShader;
    public static Cubemap _skyboxCubemap;
    public static readonly List<Skybox> Skyboxes = new();
    
    //Gameobject variables
    public static readonly List<GameObject> LitObjects = new();
    //private static readonly List<GameObject> UnlitObjects = new();
    private static readonly List<PointLight> Lights = new();
        
    public static Matrix4 View;
    public static Matrix4 Projection;

    public static Camera GameCam = null!;
    private Vector2 _previousMousePos;

    private const float CameraSpeed = 1.5f; 
    private const float CameraSensitivity = 0.2f;
    
    private readonly string[] _pointLightDefinition =
    {
        "pointLights[",
        "INDEX",
        "]."
    };
    
    
    
    public Game(int width, int height, string title) : 
        base(GameWindowSettings.Default, new NativeWindowSettings { Title = title, Size = (width, height) })
    {
            
    }
    
    protected override void OnLoad()
    {
        base.OnLoad();
            
        GL.ClearColor(0, 0, 0, 0);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Enable(EnableCap.CullFace);
        GL.Enable(EnableCap.DepthTest);
            
        _previousMousePos = new Vector2(MouseState.X, MouseState.Y);
        CursorState = CursorState.Grabbed;
            
        GameCam = new Camera(Vector3.UnitZ * 3, (float)Size.X / Size.Y);

        //CREATE SHADERS
        _carLitShader = new Shader("litShader.vert", "litShader.frag");
        _animegirlLitShader = new Shader("litShader.vert", "litShader.frag");
        _skyboxShader = new Shader("skyboxShader.vert", "skyboxShader.frag");
        
        
        //CREATE TEXTURES
        
        //Car Model Textures
        _carModelTextures.Add(new Texture("truckColour.jpg"));
        _carModelTextures.Add(new Texture("truckColour.jpg"));
        
        //Anime Girl Model Textures
        _animeGirlTextures.Add(new Texture("Higo_Clothes.png"));
        _animeGirlTextures.Add(new Texture("Higo_Clothes.png"));
        _animeGirlTextures.Add(new Texture("Higo_Body.png"));
        _animeGirlTextures.Add(new Texture("Higo_Hair.png"));
        _animeGirlTextures.Add(new Texture("Higo_Fire.png"));
        
        //Skybox Texture
        
        string[] faces =
        {
            "right.jpg",
            "left.jpg",
            "top.jpg",
            "bottom.jpg",
            "front.jpg",
            "back.jpg"
        };
        _skyboxCubemap = new Cubemap(faces);
        
        
        //LOAD OBJECTS
        
        //CAR MODEL STUFF
            _carLitShader.Use();
            
            //Car Texture Loading
            _carTextureUnits.Add(TextureUnit.Texture0);
            _carTextureUnits.Add(TextureUnit.Texture1);
            _carModelTextures[0].Use(_carTextureUnits[0]);
            _carTextureList.Add(0);
            _carModelTextures[1].Use(_carTextureUnits[1]);
            _carTextureList.Add(1);
            int id = _carLitShader.GetUniformLocation("modelTex");
            GL.Uniform1(id, _carTextureList.Count, _carTextureList.ToArray());
            
            //Lit Car Model Loading
            AssimpContext importer = new AssimpContext();
            PostProcessSteps postProcessSteps = PostProcessSteps.Triangulate | PostProcessSteps.CalculateTangentSpace;
            Scene scene = importer.ImportFile(StaticUtilities.ObjectDirectory + "truck.obj", postProcessSteps);
            foreach (Mesh mesh in scene.Meshes)
            {
                CarModelObjects.Add(new GameObject(mesh.ConvertMesh(), mesh.GetUnsignedIndices(), _carLitShader));
                Console.WriteLine("Loaded " + mesh.Name);
            }
            foreach(GameObject modelParts in CarModelObjects)
            {
                modelParts.Transform.Scale = new Vector3(1f, 1f, 1f);
                modelParts.Transform.Position = new Vector3(0, 0, 0);
                modelParts.Transform.Rotation = new Vector3(-MathHelper.PiOver2, 0, 0);
                LitObjects.Add(modelParts);
            }
        //END OF CAR MODEL STUFF
            
        //ANIME GIRL MODEL STUFF
            _animegirlLitShader.Use();
            
            //Anime Girl Texture Loading
            _animeGirlTextureUnits.Add(TextureUnit.Texture2);
            _animeGirlTextureUnits.Add(TextureUnit.Texture3);
            _animeGirlTextureUnits.Add(TextureUnit.Texture4);
            _animeGirlTextureUnits.Add(TextureUnit.Texture5);
            _animeGirlTextureUnits.Add(TextureUnit.Texture6);
            _animeGirlTextures[0].Use(_animeGirlTextureUnits[0]);
            _animeGirlTextureList.Add(2);
            _animeGirlTextures[1].Use(_animeGirlTextureUnits[1]);
            _animeGirlTextureList.Add(3);
            _animeGirlTextures[2].Use(_animeGirlTextureUnits[2]);
            _animeGirlTextureList.Add(4);
            _animeGirlTextures[3].Use(_animeGirlTextureUnits[3]);
            _animeGirlTextureList.Add(5);
            _animeGirlTextures[4].Use(_animeGirlTextureUnits[4]);
            _animeGirlTextureList.Add(6);
            id = _animegirlLitShader.GetUniformLocation("modelTex");
            GL.Uniform1(id, _animeGirlTextureList.Count, _animeGirlTextureList.ToArray());
            
            //Lit Anime Girl Model Loading
            importer = new AssimpContext();
            postProcessSteps = PostProcessSteps.Triangulate | PostProcessSteps.CalculateTangentSpace;
            scene = importer.ImportFile(StaticUtilities.ObjectDirectory + "animegirl.fbx", postProcessSteps);
            foreach (Mesh mesh in scene.Meshes)
            {
                AnimeGirlObjects.Add(new GameObject(mesh.ConvertMesh(), mesh.GetUnsignedIndices(), _animegirlLitShader));
                Console.WriteLine("Loaded " + mesh.Name);
            }
            foreach(GameObject modelParts in AnimeGirlObjects)
            {
                modelParts.Transform.Scale = new Vector3(.1f, .1f, .1f);
                modelParts.Transform.Position = new Vector3(0, 3, 0);
                modelParts.Transform.Rotation = new Vector3(-MathHelper.PiOver2, 0, 0);
                LitObjects.Add(modelParts);
            }
        //END OF ANIME GIRL MODEL STUFF
        
        //SKYBOX STUFF
            _skyboxShader.Use();
            _skyboxCubemap.Use(TextureUnit.Texture7);
            id = _skyboxShader.GetUniformLocation("skybox");
            GL.Uniform1(id, 7);
            Skyboxes.Add(new Skybox(StaticUtilities.SkyboxVertices, _skyboxShader));
        //END OF SKYBOX STUFF
            
        //Lights
        Lights.Add(new PointLight(new Vector3(1,0,0), .25f));
        Lights[0].Transform.Position = new Vector3(2, 2, 2);
        Lights.Add(new PointLight(new Vector3(1,1,1), 5f));
        Lights[1].Transform.Position = new Vector3(1, -6f, 3f);

    }
        
    protected override void OnUnload()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.UseProgram(0);
            
        foreach(GameObject gameObject in LitObjects)
        {
            gameObject.Dispose();
        }
        foreach(GameObject gameObject in CarModelObjects)
        {
            gameObject.Dispose();
        }
        foreach(GameObject gameObject in AnimeGirlObjects)
        {
            gameObject.Dispose();
        }
        foreach(Skybox skybox in Skyboxes)
        {
            skybox.Dispose();
        }
        /*
        foreach(GameObject gameObject in UnlitObjects)
        {
            gameObject.Dispose();
        }
        */
            
        _carLitShader.Dispose();   
        _animegirlLitShader.Dispose();
        _skyboxShader.Dispose();
            
            
        base.OnUnload();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, e.Width, e.Height);
    }
        
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
        View = GameCam.GetViewMatrix();
        Projection = GameCam.GetProjectionMatrix();
            
        for(int j = 0; j < CarModelObjects.Count; j++)
        {
            CarModelObjects[j].MyShader.Use();
            _carModelTextures[j].Use(_carTextureUnits[j]);
            int id = CarModelObjects[j].MyShader.GetUniformLocation("modelIndex");
            GL.Uniform1(id, j);
            for (int i = 0; i < Lights.Count; i++)
            {
                PointLight currentLight = Lights[i];
                _pointLightDefinition[1] = i.ToString();
                string merged = string.Concat(_pointLightDefinition);

                id = CarModelObjects[j].MyShader.GetUniformLocation(merged + "lightColor");
                GL.Uniform3(id, currentLight.Color);
            
                id = CarModelObjects[j].MyShader.GetUniformLocation(merged + "lightPos");
                GL.Uniform3(id, currentLight.Transform.Position);
        
                id = CarModelObjects[j].MyShader.GetUniformLocation(merged + "lightIntensity");
                GL.Uniform1(id, currentLight.Intensity);

            }
                
            id = CarModelObjects[j].MyShader.GetUniformLocation("numPointLights");
            GL.Uniform1(id, Lights.Count);
            CarModelObjects[j].Render();
            
        }
            
        for(int j = 0; j < AnimeGirlObjects.Count; j++)
        {
            AnimeGirlObjects[j].MyShader.Use();
            _animeGirlTextures[j].Use(_animeGirlTextureUnits[j]);
            int id = AnimeGirlObjects[j].MyShader.GetUniformLocation("modelIndex");
            GL.Uniform1(id, j);
            for (int i = 0; i < Lights.Count; i++)
            {
                PointLight currentLight = Lights[i];
                _pointLightDefinition[1] = i.ToString();
                string merged = string.Concat(_pointLightDefinition);

                id = AnimeGirlObjects[j].MyShader.GetUniformLocation(merged + "lightColor");
                GL.Uniform3(id, currentLight.Color);
            
                id = AnimeGirlObjects[j].MyShader.GetUniformLocation(merged + "lightPos");
                GL.Uniform3(id, currentLight.Transform.Position);
        
                id = AnimeGirlObjects[j].MyShader.GetUniformLocation(merged + "lightIntensity");
                GL.Uniform1(id, currentLight.Intensity);

            }
                
            id = AnimeGirlObjects[j].MyShader.GetUniformLocation("numPointLights");
            GL.Uniform1(id, Lights.Count);
            AnimeGirlObjects[j].Render();
            
        }
        
        GL.DepthMask(false);
        _skyboxShader.Use();
        Skyboxes[0].Render();
        GL.DepthMask(true);
            
            
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);
            
        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        if (KeyboardState.IsKeyDown(Keys.W))
        {
            GameCam.Position += GameCam.Forward * CameraSpeed * (float)e.Time; // Forward
        }

        if (KeyboardState.IsKeyDown(Keys.S))
        {
            GameCam.Position -= GameCam.Forward * CameraSpeed * (float)e.Time; // Backwards
        }

        if (KeyboardState.IsKeyDown(Keys.A))
        {
            GameCam.Position -= GameCam.Right * CameraSpeed * (float)e.Time; // Left
        }

        if (KeyboardState.IsKeyDown(Keys.D))
        {
            GameCam.Position += GameCam.Right * CameraSpeed * (float)e.Time; // Right
        }

        if (KeyboardState.IsKeyDown(Keys.Space))
        {
            GameCam.Position += GameCam.Up * CameraSpeed * (float)e.Time; // Up
        }

        if (KeyboardState.IsKeyDown(Keys.LeftShift))
        {
            GameCam.Position -= GameCam.Up * CameraSpeed * (float)e.Time; // Down
        }

        // Get the mouse state

        // Calculate the offset of the mouse position
        var deltaX = MouseState.X - _previousMousePos.X;
        var deltaY = MouseState.Y - _previousMousePos.Y;
        _previousMousePos = new Vector2(MouseState.X, MouseState.Y);

        // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
        GameCam.Yaw += deltaX * CameraSensitivity;
        GameCam.Pitch -= deltaY * CameraSensitivity; // Reversed since y-coordinates range from bottom to top
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);
        GameCam.Fov -= e.OffsetY;
    }

}