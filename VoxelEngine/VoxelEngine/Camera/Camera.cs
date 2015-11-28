using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace VoxelEngine.Camera
{
    public class Camera3D
    {
        public float CameraYRotation;
        
        public void OnRenderFrame(FrameEventArgs e)
        {
            Matrix4 matrixModelview;
            CameraYRotation = (CameraYRotation < 360f) ? (CameraYRotation + 0.1f * (float)e.Time) : 0f;
            Matrix4.CreateRotationY(CameraYRotation, out matrixModelview);
            matrixModelview *= Matrix4.LookAt(0f, 8f, -10f, 0f, 8f, 0f, 0f, 1f, 0f);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref matrixModelview);
        }
    }
}
