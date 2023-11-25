using OpenTK.Mathematics;

namespace ComputerGraphicsFinalTask;

public class PointLight
{
    public Transform Transform = new();
    public Vector3 Color;
    private float _intensity;
    private float _intensitySqr;
    public float Intensity
    {
        get => _intensity;
        set
        {
            _intensity = value;
            _intensitySqr = value * value;
        }
    }

    public PointLight(Vector3 col, float intensity)
    {
        Color = col;
        Intensity = intensity;
        _intensitySqr = intensity * intensity;
    }

    public void Update()
    {
        foreach (GameObject litObjects in Game.LitObjects)
        {
            float distance = Vector3.Distance(litObjects.Transform.Position, Transform.Position);
            
            //X = distance
            double brightnessFactor = Math.Pow(distance, -2) * _intensitySqr;
            
        }
    }
}