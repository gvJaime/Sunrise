using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform.Windows;

namespace Engine3D
{
    public class GLCamera
    {
        public Matrix3D viewmat;

        public Vector3d m_eye;
        public Vector3d m_lookat;
        public Vector3d m_up;
        public Vector3d m_right;
        public Vector3d m_target;
        public Vector3d m_zaxis;

        enum AnimStep
        {
            StepMove,
            StepDown,
            StepRotate,
            StepNone
        }

        float m_dx, m_dy, m_dz;
        // bulid volume parameters
        float m_bvx, m_bvy, m_bvz;
        float m_bvscalexy, m_bvscaleh;
        float deg2rad;
        float m_dir;

        float m_animmovex, m_animmovey, m_animmovez, m_animzoom;
        float m_animdown, m_animrot;
        int m_animcnt;
        AnimStep m_animstep;

        public GLCamera()
        {
            viewmat = new Matrix3D();
            deg2rad = (float)(2.0 * Math.PI / 360.0);
            m_zaxis = new Vector3d(0, 0, 1);
            m_dx = m_dy = m_dz = 0;
            m_dir = 1.0f;
            m_bvscalexy = 4.0f;
            m_bvscaleh = 1.2f;

        }

        public void UpdateBuildVolume(float bx, float by, float bz)
        {
            m_bvx = bx * m_bvscalexy / 2.0f;
            m_bvy = by * m_bvscalexy / 2.0f;
            m_bvz = bz * m_bvscaleh / 2.0f;
        }

        //This functions sets the camera view to the opengl framework
        public void SetViewGL()
        {
            float[] viewmatrix = new float[16];
            viewmatrix[0] = (float)viewmat.Matrix[0, 0];
            viewmatrix[1] = (float)viewmat.Matrix[1, 0];
            viewmatrix[2] = (float)viewmat.Matrix[2, 0];
            viewmatrix[3] = (float)viewmat.Matrix[3, 0];

            viewmatrix[4] = (float)viewmat.Matrix[0, 1];
            viewmatrix[5] = (float)viewmat.Matrix[1, 1];
            viewmatrix[6] = (float)viewmat.Matrix[2, 1];
            viewmatrix[7] = (float)viewmat.Matrix[3, 1];

            viewmatrix[8] = (float)viewmat.Matrix[0, 2];
            viewmatrix[9] = (float)viewmat.Matrix[1, 2];
            viewmatrix[10] = (float)viewmat.Matrix[2, 2];
            viewmatrix[11] = (float)viewmat.Matrix[3, 2];

            viewmatrix[12] = (float)viewmat.Matrix[0, 3];
            viewmatrix[13] = (float)viewmat.Matrix[1, 3];
            viewmatrix[14] = (float)viewmat.Matrix[2, 3];
            viewmatrix[15] = (float)viewmat.Matrix[3, 3];

            GL.LoadMatrix(viewmatrix);

            float m_offsx = m_up.x * m_dy - m_right.x * m_dx;
            float m_offsy = m_up.y * m_dy - m_right.y * m_dx;
            GL.Translate(-m_eye.x - m_offsx, -m_eye.y - m_offsy, -m_eye.z - m_dz);
        }

        void UpdateView()
        {
            // Create a 4x4 orientation matrix from the right, up, and at vectors
            Matrix3D orientation = new Matrix3D(new float [] {
                m_right.x,  m_right.y,  m_right.z,  0,
                m_up.x,     m_up.y,     m_up.z,     0,
                -m_target.x, -m_target.y, -m_target.z, 0,
                0,          0,          0,          1
            });

            viewmat = orientation;
        }

        protected Vector3d Norma()
        {
            Vector3d norm = m_lookat - m_eye;
            norm.Normalize();
            return norm;
        }

        // rotate eye deg degrees arround rotAxis pivoting at target (m_lookat)
        protected Matrix3D Rotate(Vector3d rotAxis, float deg)
        {
            float rad = deg2rad * deg;
            float c = (float)Math.Cos(rad);
            float s = (float)Math.Sin(rad);
            float t = 1.0f - c;
            float x = rotAxis.x;
            float y = rotAxis.y;
            float z = rotAxis.z;
            Matrix3D rotMat= new Matrix3D(new float [] {
                t*x*x+c, t*x*y-s*z, t*x*z+s*y, 0,
                t*x*y+s*z, t*y*y+c, t*y*z-s*x, 0,
                t*x*z-s*y, t*y*z+s*x, t*z*z+c, 0,
                0,0,0,1
            });
            
            m_eye = rotMat.Transform(m_eye - m_lookat);
            m_eye = m_eye + m_lookat;
            return rotMat;
        }

        public void RotateUp(float deg)
        {
            Rotate(m_right, deg);
            // update target and up
            m_target = m_lookat - m_eye;    // The "look-at" unit vector.
            m_target.Normalize();
            m_up = Vector3d.cross(m_right, m_target);
            UpdateView();
        }

        public void RotateRight(float deg)
        {
            Rotate(m_up, deg);
            // update target and up
            m_target = m_lookat - m_eye;    // The "look-at" unit vector.
            m_target.Normalize();
            m_right = Vector3d.cross(m_target, m_up);
            UpdateView();
        }

        public void RotateRightFlat(float deg)
        {
            RotateRightFlat(deg, m_dir);
        }

        public void RotateRightFlat(float deg, float dir)
        {
            Matrix3D rotMat = Rotate(m_zaxis, deg * dir);
            m_target = rotMat.Transform(m_target);
            m_right = rotMat.Transform(m_right);
            m_up = rotMat.Transform(m_up);
            /*m_target = m_lookat - m_eye;    // The "look-at" unit vector.
            m_target.Normalize();
            m_right = Vector3d.cross(m_target, m_up);*/
            UpdateView();
        }

        // set rotation direction based on mouse location projected to 3D space
        public void UpdateDirection(float ix, float iy)
        {
            Vector3d mouse = new Vector3d(ix, iy, 0);
            Vector3d horizon = new Vector3d(m_right.x, m_right.y, 0);
            Vector3d perp = Vector3d.cross(mouse, horizon);
            m_dir = Math.Sign(perp.z);
        }

        public void MoveForward(float dist)
        {
            float factor = Vector3d.length(m_eye - m_lookat) / 200.0f;
            MoveForward(dist, factor);
        }
   
        public void MoveForward(float dist, float factor)
        {
            if (factor < 0.3)
                factor = 0.3f;
            dist = dist * factor;
            Vector3d diff = (m_eye - m_lookat);
            float len = Vector3d.length(diff) - dist;
            if ((len <= 0) || (len >= 1000))
                return;
            diff.Normalize();
            diff = diff * (float)dist;
            m_eye = m_eye - diff;
            UpdateView();
        }

        public void Move(float dx, float dy)
        {
            // normalize dist
            float factor = Vector3d.length(m_eye - m_lookat) / 500.0f;
            Move(dx, dy, factor);
        }

        public void Move(float dx, float dy, float factor)
        {
            m_dx += dx * factor;
            if (m_dx < -m_bvx) m_dx = -m_bvx;
            else if (m_dx > m_bvx) m_dx = m_bvx;
            if (Math.Abs(m_up.z) > 0.7)
            {
                m_dz += dy * factor * Math.Sign(m_up.z);
                if (m_dz < -m_bvz) m_dz = -m_bvz;
                else if (m_dz > m_bvz) m_dz = m_bvz;
            }
            else
            {
                m_dy += dy * factor;
                if (m_dy < -m_bvy) m_dy = -m_bvy;
                else if (m_dy > m_bvy) m_dy = m_bvy;
            }
        }

        public void ResetView(float x, float y, float z, float updeg, float lookz)
        {
            m_eye = new Vector3d(x, y, z);
            m_lookat = new Vector3d(0, 0, 0);
            m_up = new Vector3d(0, 0, 1);
            m_dz = lookz;
            m_dx = m_dy = 0;
            //m_right = new Vector3d(1, 0, 0);
            m_target = m_lookat - m_eye;    // The "look-at" unit vector.
            m_target.Normalize();
            m_right = Vector3d.cross(m_target, m_up);
            RotateUp(updeg);
            UpdateView();

            //Vector3d xaxis = Vector3d.cross(up, zaxis);// The "right" vector.
            //xaxis.Normalize();
            //Vector3d yaxis = Vector3d.cross(zaxis, xaxis);     // The "up" vector.
        }

        public void ResetViewAnim(float x, float y, float z, float updeg, float lookz)
        {
            m_animcnt = 10;
            // move
            m_animstep = AnimStep.StepMove;
            m_animmovex = -m_dx / m_animcnt;
            m_animmovey = -m_dy / m_animcnt;
            m_animmovez = (lookz - m_dz) / m_animcnt;

            // zoom
            Vector3d diff = (m_eye - m_lookat);
            float len = Vector3d.length(diff) - Vector3d.length(new Vector3d(x, y, z));
            m_animzoom = len / m_animcnt;

            // down
            len = (float)Math.Sqrt(m_eye.x * m_eye.x + m_eye.y * m_eye.y);
            float deg = (float)Math.Atan2(m_eye.z, len) / deg2rad;
            if (m_up.z < 0)
                deg = 180 - deg;
            if (((deg > 85) && (deg < 95)) || ((deg < -85) && (deg > -95)))
            {
                RotateUp(-deg / 10);
                deg -= deg / 10;
            }
            m_animdown = (updeg - deg) / m_animcnt;
            
            // rotate
            deg = (float)Math.Atan2(m_eye.x, -m_eye.y) / deg2rad;
            if (m_up.z < 0)
                deg += 180;
            if (deg > 360)
                deg -= 360;
            m_animrot = deg / m_animcnt;
        }

        public bool AnimTick()
        {
            switch (m_animstep)
            {
                case AnimStep.StepMove:
                    m_dx += m_animmovex;
                    m_dy += m_animmovey;
                    m_dz += m_animmovez;
                    MoveForward(m_animzoom, 1);
                    RotateUp(m_animdown);
                    RotateRightFlat(m_animrot, 1);
                    m_animcnt--;
                    if (m_animcnt == 0)
                    {
                        m_animstep = AnimStep.StepNone;
                        return false;
                    }
                    return true;

                case AnimStep.StepRotate:
                    RotateRightFlat(m_animrot);
                    m_animcnt--;
                    if (m_animcnt == 0)
                    {
                        m_animstep = AnimStep.StepNone;
                        return false;
                    }
                    return true;

            }
            return false;
        }
    }
}
