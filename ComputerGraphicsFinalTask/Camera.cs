using OpenTK.Mathematics;

namespace ComputerGraphicsFinalTask;

public class Camera
{
    public Vector3 Position;
    public readonly float AspectRatio;

    public Vector3 Forward;
    public Vector3 Up;
    public Vector3 Right;

    
    
    // Rotation around the X axis (radians)
    private float _pitch = 0;

    // Rotation around the Y axis (radians)
    private float _yaw = -MathHelper.PiOver2; // Without this, you would be started rotated 90 degrees right.
    
    private float _fov = MathHelper.PiOver2;

    public float Fov
    {
        get => MathHelper.RadiansToDegrees(_fov);
        set => _fov = MathHelper.DegreesToRadians(MathHelper.Clamp(value, 1f, 90));
    }

    public float Pitch
    {
        get => MathHelper.RadiansToDegrees(_pitch);
        set
        {
            //Beware gimbal lock
            float angle = MathHelper.Clamp(value, -89f, 89f);
            _pitch = MathHelper.DegreesToRadians(angle);
            UpdateVectors();
        }
    }
    
    public float Yaw
    {
        get => MathHelper.RadiansToDegrees(_yaw);
        set
        {
            _yaw = MathHelper.DegreesToRadians(value);
            UpdateVectors();
        }
    }

    public Camera(Vector3 position, float aspectRatio)
    {
        Position = position;
        AspectRatio = aspectRatio;
    }

    public Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(Position, Position + Forward, Up);
    }

    public Matrix4 GetProjectionMatrix()
    {
        return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f);
    }

    private void UpdateVectors()
    {
        Forward.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
        Forward.Y = MathF.Sin(_pitch);
        Forward.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);

        Forward = Vector3.Normalize(Forward);
        
        Right = Vector3.Normalize(Vector3.Cross(Forward, Vector3.UnitY));
        Up = Vector3.Normalize(Vector3.Cross(Right, Forward));
    }
}