using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace UV_DLP_3D_Printer._3DEngine
{
    public class ArcBall
    {
        private Vector3 _start = Vector3.Zero;
        private Vector3 _end = Vector3.Zero;
        private float _adjustWidth = 0.0f;
        private float _adjustHeight = 0.0f;

        public void Resize(float width, float height)
        {
            _adjustWidth = 1.0f / ((width - 1.0f) * 0.5f);
            _adjustHeight = 1.0f / ((height - 1.0f) * 0.5f);
        }

        public void Click(Vector2 point)
        {
            MapToSphere(point, ref _start);
        }

        public Quaternion Drag(Vector2 point)
        {
            MapToSphere(point, ref _end);

            Quaternion rotate = Quaternion.Identity;
            Vector3 perpendicular = Vector3.Cross(_start, _end);
            if (perpendicular.Length > float.Epsilon)
            {
                rotate.X = perpendicular.X;
                rotate.Y = perpendicular.Y;
                rotate.Z = perpendicular.Z;
                rotate.W = Vector3.Dot(_start, _end);
            }

            return rotate;
        }

        private void MapToSphere(Vector2 point, ref Vector3 vector)
        {
            point.X = (point.X * _adjustWidth) - 1.0f;
            point.Y = 1.0f - (point.Y * _adjustHeight);

            float length = (point.X * point.X) + (point.Y * point.Y);

            if (length > 1.0f)
            {
                float norm = (float)(1.0 / Math.Sqrt(length));

                vector.X = point.X * norm;
                vector.Y = point.Y * norm;
                vector.Z = 0.0f;
            }
            else
            {
                vector.X = point.X;
                vector.Y = point.Y;
                //vector.Z = (float)Math.Sqrt(1.0f - point.Length);
                vector.Z = (float)Math.Sqrt(1.0f - length);
            }
        }
    }
}


