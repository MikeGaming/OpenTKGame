using System.Diagnostics.CodeAnalysis;
using OpenTK.Graphics.OpenGL4;
using Assimp;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

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
    
    //Billboard Tree Variables
    private Shader _billboardTreeShader;
    public static Texture _billboardTreeTexture;
    public static readonly List<GameObject> BillboardTreeObjects = new();
    
    //Flipbook Variables
    private Shader _flipbookShader;
    public static Texture _flipbookTexture;
    public static readonly List<GameObject> FlipbookObjects = new();
    
    //Water Variables
    private Shader _waterShader;
    public static readonly List<GameObject> WaterObjects = new();
    
    //Terrain Variables
    private Shader _terrainShader;
    private Texture _waterTexture;
    private Texture _grassTexture;
    private Texture _stoneTexture;
    public static readonly List<GameObject> TerrainObjects = new();

    //Skybox Variables
    private Shader _skyboxShader;
    public static Cubemap _skyboxCubemap;
    public static readonly List<Skybox> Skyboxes = new();
    
    
    //Screenspace Variables
    private Shader _postProcessShader;
    private Shader _colorCorrectShader;
    private Shader _filmgrainShader;
    private Shader _vignetteShader;
    private Shader _pixelShader;
    private Shader _kuwaharaShader;
    private Shader _sketchShader;
    private Shader _toonShader;
    private Shader _chromaticShader;
    private Shader _gausShader;
    private Shader _testingShader;
    private Shader _rainShader;
    private static readonly List<ScreenSpaceObject> ScreenSpaceObjects = new();
    
    //Gameobject variables
    public static readonly List<GameObject> LitObjects = new();
    //private static readonly List<GameObject> UnlitObjects = new();
    private static readonly List<PointLight> Lights = new();
        
    public static Matrix4 View;
    public static Matrix4 Projection;

    public static Camera GameCam = null!;
    private Vector2 _previousMousePos;

    private float x; //time

    private int _width, _height;

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
        _width = width;
        _height = height;
    }
    
    [SuppressMessage("ReSharper.DPA", "DPA0000: DPA issues")]
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
            
        GameCam = new Camera(Vector3.UnitZ * 3, (float)_width / _height);

        //CREATE SHADERS
        _carLitShader = new Shader("litShader.vert", "litShader.frag");
        _animegirlLitShader = new Shader("goochShader.vert", "goochShader.frag");
        _skyboxShader = new Shader("skyboxShader.vert", "skyboxShader.frag");
        _billboardTreeShader = new Shader("treeShader.vert", "treeShader.frag");
        _flipbookShader = new Shader("flipbookShader.vert", "flipbookShader.frag");
        _waterShader = new Shader("waterShader.vert", "waterShader.frag");
        _terrainShader = new Shader("terrainShader.vert", "terrainShader.frag");
        _rainShader = new Shader("PostProcess/rainShader.vert", "PostProcess/rainShader.frag");
        _postProcessShader = new Shader("PostProcess/postProcess.vert", "PostProcess/postProcess.frag");
        _colorCorrectShader = new Shader("PostProcess/colorCorrection.vert", "PostProcess/colorCorrection.frag");
        _filmgrainShader = new Shader("PostProcess/filmgrainShader.vert", "PostProcess/filmgrainShader.frag");
        _vignetteShader = new Shader("PostProcess/vignetteShader.vert", "PostProcess/vignetteShader.frag");
        _pixelShader = new Shader("PostProcess/pixelShader.vert", "PostProcess/pixelShader.frag");
        _kuwaharaShader = new Shader("PostProcess/kuwaharaShader.vert", "PostProcess/kuwaharaShader.frag");
        _sketchShader = new Shader("PostProcess/sketchShader.vert", "PostProcess/sketchShader.frag");
        _toonShader = new Shader("PostProcess/toonShader.vert", "PostProcess/toonShader.frag");
        _chromaticShader = new Shader("PostProcess/chromaticShader.vert", "PostProcess/chromaticShader.frag");
        _gausShader = new Shader("PostProcess/gausShader.vert", "PostProcess/gausShader.frag");
        _testingShader = new Shader("PostProcess/testingShader.vert", "PostProcess/testingShader.frag");
        
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
        
        //Tree Textures
        _billboardTreeTexture = new Texture("Tree.png");
        
        //Flipbook Textures
        _flipbookTexture = new Texture("flipbook.png");
        
        //Terrain Textures
        _waterTexture = new Texture("water.jpg");
        _grassTexture = new Texture("grass.jpg");
        _stoneTexture = new Texture("stone.jpg");
        
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
                modelParts.Transform.Position = new Vector3(4, 2, 0);
                //modelParts.Transform.Rotation = new Vector3(0, 0, 0);
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
        
        //BILLBOARD TREE STUFF
            _billboardTreeShader.Use();
            _billboardTreeTexture.Use(TextureUnit.Texture8);
            id = _billboardTreeShader.GetUniformLocation("modelTex");
            GL.Uniform1(id, 8);
            BillboardTreeObjects.Add(new GameObject(StaticUtilities.QuadVertices, StaticUtilities.QuadIndices,
                _billboardTreeShader));
            //BillboardTreeObjects[BillboardTreeObjects.Count - 1].Transform.Scale = new Vector3(100f, 100f, 100f);
            BillboardTreeObjects[BillboardTreeObjects.Count - 1].Transform.Position = new Vector3(6f, 2.25f, -1f);
        //END OF BILLBOARD TREE STUFF
        
        //FLIPBOOK EXPLOSION STUFF
            _flipbookShader.Use();
            _flipbookTexture.Use(TextureUnit.Texture9);
            id = _flipbookShader.GetUniformLocation("modelTex");
            GL.Uniform1(id, 9);
            FlipbookObjects.Add(new GameObject(StaticUtilities.QuadVertices, StaticUtilities.QuadIndices,
                _flipbookShader));
            FlipbookObjects[FlipbookObjects.Count - 1].Transform.Position = new Vector3(2, 2, 0);
        //END OF FLIPBOOK EXPLOSION STUFF
        
        //WATER STUFF
            _waterShader.Use();
            _waterTexture.Use(TextureUnit.Texture10);
            id = _waterShader.GetUniformLocation("modelTex");
            GL.Uniform1(id, 10);
            importer = new AssimpContext();
            postProcessSteps = PostProcessSteps.Triangulate | PostProcessSteps.CalculateTangentSpace;
            scene = importer.ImportFile(StaticUtilities.ObjectDirectory + "water.fbx", postProcessSteps);
            foreach (Mesh mesh in scene.Meshes)
            {
                WaterObjects.Add(new GameObject(mesh.ConvertMesh(), mesh.GetUnsignedIndices(), _waterShader));
                Console.WriteLine("Loaded " + mesh.Name);
            }
            foreach(GameObject modelParts in WaterObjects)
            {
                modelParts.Transform.Scale = new Vector3(100f, 100f, 1f);
                modelParts.Transform.Position = new Vector3(0, 1f, 0);
                modelParts.Transform.Rotation = new Vector3(-MathHelper.PiOver2, 0, 0);
                LitObjects.Add(modelParts);
            }
        //END OF WATER STUFF
        
        //TERRAIN STUFF
            _terrainShader.Use();
            _waterTexture.Use(TextureUnit.Texture10);
            _grassTexture.Use(TextureUnit.Texture11);
            _stoneTexture.Use(TextureUnit.Texture12);
            id = _terrainShader.GetUniformLocation("rockTexture");
            GL.Uniform1(id, 10);
            id = _terrainShader.GetUniformLocation("grassTexture");
            GL.Uniform1(id, 11);
            id = _terrainShader.GetUniformLocation("snowTexture");
            GL.Uniform1(id, 12);
            importer = new AssimpContext();
            postProcessSteps = PostProcessSteps.Triangulate | PostProcessSteps.CalculateTangentSpace;
            scene = importer.ImportFile(StaticUtilities.ObjectDirectory + "water.fbx", postProcessSteps);
            foreach (Mesh mesh in scene.Meshes)
            {
                TerrainObjects.Add(new GameObject(mesh.ConvertMesh(), mesh.GetUnsignedIndices(), _terrainShader));
                Console.WriteLine("Loaded " + mesh.Name);
            }
            foreach(GameObject modelParts in TerrainObjects)
            {
                modelParts.Transform.Scale = new Vector3(100f, 100f, 1f);
                modelParts.Transform.Position = new Vector3(0, 0f, 0);
                modelParts.Transform.Rotation = new Vector3(-MathHelper.PiOver2, 0, 0);
                LitObjects.Add(modelParts);
            }
        //END OF TERRAIN STUFF
        
        //SKYBOX STUFF
            _skyboxShader.Use();
            _skyboxCubemap.Use(TextureUnit.Texture7);
            id = _skyboxShader.GetUniformLocation("skybox");
            GL.Uniform1(id, 7);
            Skyboxes.Add(new Skybox(StaticUtilities.SkyboxVertices, _skyboxShader));
        //END OF SKYBOX STUFF
        
        //SCREENSPACE STUFF
            _postProcessShader.Use();
            ScreenSpaceObjects.Add(new ScreenSpaceObject(StaticUtilities.screenSpaceVerts, StaticUtilities.QuadIndices,
                _postProcessShader, _width, _height));
            id = _postProcessShader.GetUniformLocation("resolution");
            GL.Uniform2(id, new Vector2(_width, _height));
            
            _colorCorrectShader.Use();
            id = _colorCorrectShader.GetUniformLocation("contrast");
            GL.Uniform1(id, 1f);
            id = _colorCorrectShader.GetUniformLocation("brightness");
            GL.Uniform1(id, 0f);
            id = _colorCorrectShader.GetUniformLocation("saturation");
            GL.Uniform1(id, 1f);
            id = _colorCorrectShader.GetUniformLocation("gamma");
            GL.Uniform1(id, 1f);
            ScreenSpaceObjects.Add(new ScreenSpaceObject(StaticUtilities.screenSpaceVerts, StaticUtilities.QuadIndices,
                _colorCorrectShader, _width, _height));
            
            _filmgrainShader.Use();
            id = _filmgrainShader.GetUniformLocation("grainAmount");
            GL.Uniform1(id, 0.075f);
            ScreenSpaceObjects.Add(new ScreenSpaceObject(StaticUtilities.screenSpaceVerts, StaticUtilities.QuadIndices,
                _filmgrainShader, _width, _height));
            
            _vignetteShader.Use();
            id = _vignetteShader.GetUniformLocation("vignetteStrength");
            GL.Uniform1(id, 0.5f);
            id = _vignetteShader.GetUniformLocation("falloff");
            GL.Uniform1(id, 0.5f);
            ScreenSpaceObjects.Add(new ScreenSpaceObject(StaticUtilities.screenSpaceVerts, StaticUtilities.QuadIndices,
                _vignetteShader, _width, _height));
            
            _pixelShader.Use();
            ScreenSpaceObjects.Add(new ScreenSpaceObject(StaticUtilities.screenSpaceVerts, StaticUtilities.QuadIndices,
                _pixelShader, _width, _height));
            
            _kuwaharaShader.Use();
            ScreenSpaceObjects.Add(new ScreenSpaceObject(StaticUtilities.screenSpaceVerts, StaticUtilities.QuadIndices,
                _kuwaharaShader, _width, _height));
            
            _sketchShader.Use();
            id = _sketchShader.GetUniformLocation("intensity");
            GL.Uniform1(id, 0.9f);
            ScreenSpaceObjects.Add(new ScreenSpaceObject(StaticUtilities.screenSpaceVerts, StaticUtilities.QuadIndices,
                _sketchShader, _width, _height));
            
            _toonShader.Use();
            id = _toonShader.GetUniformLocation("resolution");
            GL.Uniform2(id, new Vector2(_width, _height));
            ScreenSpaceObjects.Add(new ScreenSpaceObject(StaticUtilities.screenSpaceVerts, StaticUtilities.QuadIndices,
                _toonShader, _width, _height));
            
            _chromaticShader.Use();
            id = _chromaticShader.GetUniformLocation("resolution");
            GL.Uniform2(id, new Vector2(_width, _height));
            ScreenSpaceObjects.Add(new ScreenSpaceObject(StaticUtilities.screenSpaceVerts, StaticUtilities.QuadIndices,
                _chromaticShader, _width, _height));
            
            _gausShader.Use();
            id = _gausShader.GetUniformLocation("resolution");
            GL.Uniform2(id, new Vector2(_width, _height));
            ScreenSpaceObjects.Add(new ScreenSpaceObject(StaticUtilities.screenSpaceVerts, StaticUtilities.QuadIndices,
                _gausShader, _width, _height));
            
            _testingShader.Use();
            id = _testingShader.GetUniformLocation("resolution");
            GL.Uniform2(id, new Vector2(_width, _height));
            ScreenSpaceObjects.Add(new ScreenSpaceObject(StaticUtilities.screenSpaceVerts, StaticUtilities.QuadIndices,
                _testingShader, _width, _height));
            
            _rainShader.Use();
            id = _rainShader.GetUniformLocation("resolution");
            ScreenSpaceObjects.Add(new ScreenSpaceObject(StaticUtilities.screenSpaceVerts, StaticUtilities.QuadIndices,
                _rainShader, _width, _height));
            
        //Lights
        Lights.Add(new PointLight(new Vector3(1,1,1), 1f));
        Lights[0].Transform.Position = new Vector3(5f, 10f, 1f);
        FlipbookObjects[FlipbookObjects.Count - 1].Transform.Position = Lights[0].Transform.Position;
        FlipbookObjects[FlipbookObjects.Count - 1].Transform.Scale = new Vector3(0.1f);
        //Lights.Add(new PointLight(new Vector3(0.9922f,0.9843f,0.8275f), 1f));
        //Lights[1].Transform.Position = new Vector3(0, 50f, 0);
        
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
        foreach(GameObject gameObject in BillboardTreeObjects)
        {
            gameObject.Dispose();
        }
        foreach(GameObject gameObject in FlipbookObjects)
        {
            gameObject.Dispose();
        }
        foreach(GameObject gameObject in WaterObjects)
        {
            gameObject.Dispose();
        }
        foreach(GameObject gameObject in TerrainObjects)
        {
            gameObject.Dispose();
        }
        foreach(Skybox skybox in Skyboxes)
        {
            skybox.Dispose();
        }
        foreach(ScreenSpaceObject screenSpaceObject in ScreenSpaceObjects)
        {
            screenSpaceObject.Dispose();
        }
        /*
        foreach(GameObject gameObject in UnlitObjects)
        {
            gameObject.Dispose();
        }
        */
            
        _carLitShader.Dispose();   
        _animegirlLitShader.Dispose();
        _billboardTreeShader.Dispose();
        _flipbookShader.Dispose();
        _skyboxShader.Dispose();
        _waterShader.Dispose();
        _terrainShader.Dispose();
        _rainShader.Dispose();
        _postProcessShader.Dispose();
        _colorCorrectShader.Dispose();
        _filmgrainShader.Dispose();
        _vignetteShader.Dispose();
        _pixelShader.Dispose();
        _kuwaharaShader.Dispose();
        _sketchShader.Dispose();
        _toonShader.Dispose();
        _chromaticShader.Dispose();
        _gausShader.Dispose();
        _testingShader.Dispose();
            
        base.OnUnload();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        _width = e.Width;
        _height = e.Height;
        GL.Viewport(0, 0, e.Width, e.Height);
    }
    
    int cycle = 0;
    private bool bloom, exposure, whiteBalance, colorCorrect;
    
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        
        _width = Size.X;
        _height = Size.Y;
        
        base.OnRenderFrame(e);

        ScreenSpaceObjects[cycle].BindFBO();
        
        GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
        x += (float)e.Time;
        
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
            id = AnimeGirlObjects[j].MyShader.GetUniformLocation("resolution");
            GL.Uniform2(id, new Vector2(_width, _height));
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
        
        for(int j = 0; j < TerrainObjects.Count; j++)
        {
            TerrainObjects[j].MyShader.Use();
            _waterTexture.Use(TextureUnit.Texture10);
            _grassTexture.Use(TextureUnit.Texture11);
            _stoneTexture.Use(TextureUnit.Texture12);
            int id;
            for (int i = 0; i < Lights.Count; i++)
            {
                PointLight currentLight = Lights[i];
                _pointLightDefinition[1] = i.ToString();
                string merged = string.Concat(_pointLightDefinition);

                id = TerrainObjects[j].MyShader.GetUniformLocation(merged + "lightColor");
                GL.Uniform3(id, currentLight.Color);
            
                id = TerrainObjects[j].MyShader.GetUniformLocation(merged + "lightPos");
                GL.Uniform3(id, currentLight.Transform.Position);
        
                id = TerrainObjects[j].MyShader.GetUniformLocation(merged + "lightIntensity");
                GL.Uniform1(id, currentLight.Intensity);

            }
                
            id = TerrainObjects[j].MyShader.GetUniformLocation("numPointLights");
            GL.Uniform1(id, Lights.Count);
            TerrainObjects[j].Render();
            
        }
        
        GL.DepthFunc(DepthFunction.Lequal);
        Skyboxes[0].Render();
        GL.DepthFunc(DepthFunction.Less);
        
        for(int j = 0; j < WaterObjects.Count; j++)
        {
            WaterObjects[j].MyShader.Use();
            _waterTexture.Use(TextureUnit.Texture10);
            int id = WaterObjects[j].MyShader.GetUniformLocation("time");
            GL.Uniform1(id, x);
            for (int i = 0; i < Lights.Count; i++)
            {
                PointLight currentLight = Lights[i];
                _pointLightDefinition[1] = i.ToString();
                string merged = string.Concat(_pointLightDefinition);

                id = WaterObjects[j].MyShader.GetUniformLocation(merged + "lightColor");
                GL.Uniform3(id, currentLight.Color);
            
                id = WaterObjects[j].MyShader.GetUniformLocation(merged + "lightPos");
                GL.Uniform3(id, currentLight.Transform.Position);
        
                id = WaterObjects[j].MyShader.GetUniformLocation(merged + "lightIntensity");
                GL.Uniform1(id, currentLight.Intensity);

            }
                
            id = WaterObjects[j].MyShader.GetUniformLocation("numPointLights");
            GL.Uniform1(id, Lights.Count);
            WaterObjects[j].Render();
            
        }
        
        for(int i = 0; i < BillboardTreeObjects.Count; i++)
        {
            BillboardTreeObjects[i].MyShader.Use();
            _billboardTreeTexture.Use(TextureUnit.Texture8);
            BillboardTreeObjects[i].Render();
        }
        
        for(int i = 0; i < FlipbookObjects.Count; i++)
        {
            FlipbookObjects[i].MyShader.Use();
            _flipbookTexture.Use(TextureUnit.Texture9);
            GL.Uniform1(FlipbookObjects[i].MyShader.GetUniformLocation("time"), x);
            FlipbookObjects[i].Render();
        }

        if (cycle == 0)
        {
            _postProcessShader.Use();
            int id = _postProcessShader.GetUniformLocation("resolution");
            GL.Uniform2(id, new Vector2(_width, _height));
            id = _postProcessShader.GetUniformLocation("bloomEnabled");
            GL.Uniform1(id, bloom ? 1 : 0);
            id = _postProcessShader.GetUniformLocation("exposureEnabled");
            GL.Uniform1(id, exposure ? 1 : 0);
            id = _postProcessShader.GetUniformLocation("whiteBalanceEnabled");
            GL.Uniform1(id, whiteBalance ? 1 : 0);
            id = _postProcessShader.GetUniformLocation("colorCorrectionEnabled");
            GL.Uniform1(id, colorCorrect ? 1 : 0);
            ScreenSpaceObjects[0].Render();
        }
        if (cycle == 1)
        {
            _colorCorrectShader.Use();
            ScreenSpaceObjects[1].Render();
        }
        if (cycle == 2)
        {
            _filmgrainShader.Use();
            int id = _filmgrainShader.GetUniformLocation("time");
            GL.Uniform1(id, x);
            ScreenSpaceObjects[2].Render();
        }
        if (cycle == 3)
        {
            _vignetteShader.Use();
            ScreenSpaceObjects[3].Render();
        }
        if (cycle == 4)
        {
            _pixelShader.Use();
            int id = _pixelShader.GetUniformLocation("resolution");
            GL.Uniform2(id, new Vector2(_width, _height));
            ScreenSpaceObjects[4].Render();
        }
        if (cycle == 5)
        {
            _kuwaharaShader.Use();
            int id = _kuwaharaShader.GetUniformLocation("resolution");
            GL.Uniform2(id, new Vector2(_width, _height));
            ScreenSpaceObjects[5].Render();
        }
        if (cycle == 6)
        {
            _sketchShader.Use();
            int id = _sketchShader.GetUniformLocation("resolution");
            GL.Uniform2(id, new Vector2(_width, _height));
            ScreenSpaceObjects[6].Render();
        }
        if (cycle == 7)
        {
            _toonShader.Use();
            int id = _toonShader.GetUniformLocation("resolution");
            GL.Uniform2(id, new Vector2(_width, _height));
            ScreenSpaceObjects[7].Render();
        }
        if (cycle == 8)
        {
            _chromaticShader.Use();
            int id = _chromaticShader.GetUniformLocation("resolution");
            GL.Uniform2(id, new Vector2(_width, _height));
            ScreenSpaceObjects[8].Render();
        }
        if (cycle == 9)
        {
            _gausShader.Use();
            int id = _gausShader.GetUniformLocation("resolution");
            GL.Uniform2(id, new Vector2(_width, _height));
            ScreenSpaceObjects[9].Render();
        }
        if (cycle == 10)
        {
            _testingShader.Use();
            int id = _testingShader.GetUniformLocation("resolution");
            GL.Uniform2(id, new Vector2(_width, _height));
            id = _testingShader.GetUniformLocation("iTime");
            GL.Uniform1(id, x);
            ScreenSpaceObjects[10].Render();
        }

        if (cycle == 11)
        {
            _rainShader.Use();
            int id = _rainShader.GetUniformLocation("resolution");
            GL.Uniform2(id, new Vector2(_width, _height));
            id = _rainShader.GetUniformLocation("time");
            GL.Uniform1(id, x);
            ScreenSpaceObjects[11].Render();
        }
        
        SwapBuffers();
    }

    
    float cooldown = 0;
    private float tempCamSpeed;
    
    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);
            
        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }
        cooldown += (float)e.Time;
        
        if(KeyboardState.IsKeyDown(Keys.Tab) && cooldown > .2f)
        {
            cycle++;
            if (cycle > ScreenSpaceObjects.Count - 1)
            {
                cycle = 0;
            }
            cooldown = 0;
        }

        if (KeyboardState.IsKeyDown(Keys.D0) && cooldown > .5f)
        {
            bloom = !bloom;
            cooldown = 0;
        }
        if (KeyboardState.IsKeyDown(Keys.D1)&& cooldown > .5f)
        {
            exposure = !exposure;
            cooldown = 0;
        }
        if (KeyboardState.IsKeyDown(Keys.D2)&& cooldown > .5f)
        {
            whiteBalance = !whiteBalance;
            cooldown = 0;
        }
        if (KeyboardState.IsKeyDown(Keys.D3)&& cooldown > .5f)
        {
            colorCorrect = !colorCorrect;
            cooldown = 0;
        }
        
        
        if (KeyboardState.IsKeyDown(Keys.LeftShift))
        {
            tempCamSpeed = CameraSpeed * 2.0f;
        }
        else
        {
            tempCamSpeed = CameraSpeed;
        }
        
        if (KeyboardState.IsKeyDown(Keys.W))
        {
            GameCam.Position += GameCam.Forward * tempCamSpeed * (float)e.Time; // Forward
        }

        if (KeyboardState.IsKeyDown(Keys.S))
        {
            GameCam.Position -= GameCam.Forward * tempCamSpeed * (float)e.Time; // Backwards
        }

        if (KeyboardState.IsKeyDown(Keys.A))
        {
            GameCam.Position -= GameCam.Right * tempCamSpeed * (float)e.Time; // Left
        }

        if (KeyboardState.IsKeyDown(Keys.D))
        {
            GameCam.Position += GameCam.Right * tempCamSpeed * (float)e.Time; // Right
        }

        if (KeyboardState.IsKeyDown(Keys.Space))
        {
            GameCam.Position += GameCam.Up * tempCamSpeed * (float)e.Time; // Up
        }

        if (KeyboardState.IsKeyDown(Keys.LeftControl))
        {
            GameCam.Position -= GameCam.Up * tempCamSpeed * (float)e.Time; // Down
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