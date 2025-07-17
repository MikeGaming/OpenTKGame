using OpenTK.Mathematics;

namespace ComputerGraphicsFinalTask;

public class DirectionalLight
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

    public DirectionalLight(Vector3 col, float intensity)
    {
        Color = col;
        Intensity = intensity;
        _intensitySqr = intensity * intensity;
    }
}
