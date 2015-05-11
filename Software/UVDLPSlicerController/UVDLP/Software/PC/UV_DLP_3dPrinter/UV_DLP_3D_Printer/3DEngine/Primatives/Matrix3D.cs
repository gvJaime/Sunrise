using System;
using System.Collections.Generic;
using System.Text;
using UV_DLP_3D_Printer;
namespace Engine3D
{
    public class Matrix3D
    {
        public float [,] Matrix;

        public Matrix3D() 
        {
            Matrix = new float[4, 4];
            Identity();
        }

        public Matrix3D(float [] vals)
        {
            Matrix = new float[4, 4];
            Matrix[0, 0] = vals[0];  Matrix[0, 1] = vals[1];  Matrix[0, 2] = vals[2];  Matrix[0, 3] = vals[3];
            Matrix[1, 0] = vals[4];  Matrix[1, 1] = vals[5];  Matrix[1, 2] = vals[6];  Matrix[1, 3] = vals[7];
            Matrix[2, 0] = vals[8];  Matrix[2, 1] = vals[9];  Matrix[2, 2] = vals[10]; Matrix[2, 3] = vals[11];
            Matrix[3, 0] = vals[12]; Matrix[3, 1] = vals[13]; Matrix[3, 2] = vals[14]; Matrix[3, 3] = vals[15];
        }

        public void Identity ()
        {
            Matrix[0,0] = 1;  Matrix[0,1] = 0;  Matrix[0,2] = 0;  Matrix[0,3] = 0;
            Matrix[1,0] = 0;  Matrix[1,1] = 1;  Matrix[1,2] = 0;  Matrix[1,3] = 0;
            Matrix[2,0] = 0;  Matrix[2,1] = 0;  Matrix[2,2] = 1;  Matrix[2,3] = 0;
            Matrix[3,0] = 0;  Matrix[3,1] = 0;  Matrix[3,2] = 0;  Matrix[3,3] = 1;
        }
        public static void MergeMatrices (ref Matrix3D Dest , Matrix3D Source  )
        {
           // Multiply Source by Dest; store result in Temp:
           float [,] Temp  = new float [ 4 , 4 ];
           for (int i = 0; i < 4; i++)
           {
               for (int j = 0; j < 4; j++)
               {
                   Temp[i, j] = (Source.Matrix[i, 0] * Dest.Matrix[0, j])
                                 + (Source.Matrix[i, 1] * Dest.Matrix[1, j])
                                 + (Source.Matrix[i, 2] * Dest.Matrix[2, j])
                                 + (Source.Matrix[i, 3] * Dest.Matrix[3, j]);
               }
           }
           // Copy Temp to Dest.Matrix:
            for (int i = 0; i < 4; i++)
            {
                Dest.Matrix [ i , 0 ] = Temp [ i , 0 ];
                Dest.Matrix [ i , 1 ] = Temp [ i , 1 ];
                Dest.Matrix [ i , 2 ] = Temp [ i , 2 ];
                Dest.Matrix [ i , 3 ] = Temp [ i , 3 ];
            }

        }
        void MergeMatrix (Matrix3D NewMatrix )
        {
            // Multiply NewMatirx by Matrix; store result in TempMatrix
            float [,] TempMatrix  = new float [ 4 , 4 ];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    TempMatrix[i, j] = (Matrix[i, 0] * NewMatrix.Matrix[0, j])
                    + (Matrix[i, 1] * NewMatrix.Matrix[1, j])
                    + (Matrix[i, 2] * NewMatrix.Matrix[2, j])
                    + (Matrix[i, 3] * NewMatrix.Matrix[3, j]);
                }
            }

            // Copy TempMatrix to Matrix:
            for (int i = 0; i < 4; i++)
            {
                Matrix[i,0] = TempMatrix[i,0];
                Matrix[i,1] = TempMatrix[i,1];
                Matrix[i,2] = TempMatrix[i,2];
                Matrix[i,3] = TempMatrix[i,3];
            }   
        }
        public void  Rotate ( float Xa, float Ya, float Za )
        {
            Matrix3D Rmat = new Matrix3D();
            Matrix3D RMatrix = new Matrix3D();
            float sinxa = (float)Math.Sin(Xa);
            float cosxa = (float)Math.Cos(Xa);
            float sinza = (float)Math.Sin(Za);
            float cosza = (float)Math.Cos(Za);
            float sinya = (float)Math.Sin(Ya);
            float cosya = (float)Math.Cos(Ya);
            Rmat.Identity();
            RMatrix.Identity();

            // Initialize Z rotation matrix - Note: we perform Z
            // rotation first to align the 3D Z axis with the 2D Z axis.
            Rmat.Matrix[0,0]=cosza;    Rmat.Matrix[0,1]=sinza;    Rmat.Matrix[0,2]=0;    Rmat.Matrix[0,3]=0;
            Rmat.Matrix[1,0]=-sinza;   Rmat.Matrix[1,1]=cosza;    Rmat.Matrix[1,2]=0;    Rmat.Matrix[1,3]=0;
            Rmat.Matrix[2,0]=0;        Rmat.Matrix[2,1]=0;        Rmat.Matrix[2,2]=1;    Rmat.Matrix[2,3]=0;
            Rmat.Matrix[3,0]=0;        Rmat.Matrix[3,1]=0;        Rmat.Matrix[3,2]=0;    Rmat.Matrix[3,3]=1;

            // Merge matrix with master matrix:
            MergeMatrices (ref RMatrix, Rmat );

            // Initialize X rotation matrix:
            Rmat.Matrix[0,0]=1;  Rmat.Matrix[0,1]=0;        Rmat.Matrix[0,2]=0;       Rmat.Matrix[0,3]=0;
            Rmat.Matrix[1,0]=0;  Rmat.Matrix[1,1]=cosxa;    Rmat.Matrix[1,2]=sinxa;   Rmat.Matrix[1,3]=0;
            Rmat.Matrix[2,0]=0;  Rmat.Matrix[2,1]=-sinxa;   Rmat.Matrix[2,2]=cosxa;   Rmat.Matrix[2,3]=0;
            Rmat.Matrix[3,0]=0;  Rmat.Matrix[3,1]=0;        Rmat.Matrix[3,2]=0;       Rmat.Matrix[3,3]=1;

            // Merge matrix with master matrix:
            MergeMatrices(ref RMatrix, Rmat);

            // Initialize Y rotation matrix:
            Rmat.Matrix[0,0]=cosya;   Rmat.Matrix[0,1]=0;   Rmat.Matrix[0,2]=-sinya;   Rmat.Matrix[0,3]=0;
            Rmat.Matrix[1,0]=0;       Rmat.Matrix[1,1]=1;   Rmat.Matrix[1,2]=0;        Rmat.Matrix[1,3]=0;
            Rmat.Matrix[2,0]=sinya;   Rmat.Matrix[2,1]=0;   Rmat.Matrix[2,2]=cosya;    Rmat.Matrix[2,3]=0;
            Rmat.Matrix[3,0]=0;       Rmat.Matrix[3,1]=0;   Rmat.Matrix[3,2]=0;        Rmat.Matrix[3,3]=1;

            // Merge matrix with master matrix:
            MergeMatrices (ref RMatrix, Rmat );

            MergeMatrix ( RMatrix ); // now merge with this one.

        }
        public void Translate ( float Xt, float Yt, float Zt )
        {
            // Create 3D translation matrix:

            // Declare translation matrix:
            Matrix3D  Tmat  = new Matrix3D ();

            // Initialize translation matrix:
            Tmat.Matrix[0,0]=1;  Tmat.Matrix[0,1]=0;  Tmat.Matrix[0,2]=0;  Tmat.Matrix[0,3]=0;
            Tmat.Matrix[1,0]=0;  Tmat.Matrix[1,1]=1;  Tmat.Matrix[1,2]=0;  Tmat.Matrix[1,3]=0;
            Tmat.Matrix[2,0]=0;  Tmat.Matrix[2,1]=0;  Tmat.Matrix[2,2]=1;  Tmat.Matrix[2,3]=0;
            Tmat.Matrix[3,0]=Xt; Tmat.Matrix[3,1]=Yt; Tmat.Matrix[3,2]=Zt; Tmat.Matrix[3,3]=1;

            // Merge matrix with master matrix:
            MergeMatrix ( Tmat );
        }
        public Point3d Transform(Point3d V)
        {
            Point3d pnt = new Point3d();
	        pnt.x = ( ( V.x * Matrix [ 0 , 0 ]) )
                  + ( ( V.y * Matrix [ 1 , 0 ]) )
                  + ( ( V.z * Matrix [ 2 , 0 ]) )
                  + Matrix [ 3 , 0 ];
            
            pnt.y = ( ( V.x * Matrix [ 0 , 1 ]) )
                  + ( ( V.y * Matrix [ 1 , 1 ]) )
                  + ( ( V.z * Matrix [ 2 , 1 ]) )
                  + Matrix [ 3 , 1 ];
            pnt.z = ( ( V.x * Matrix [ 0 , 2] ) )
                  + ( ( V.y * Matrix [ 1 , 2 ]) )
                  + ( ( V.z * Matrix [ 2 , 2 ]) )
                  + Matrix [ 3 , 2 ];

            return pnt;
        }

        // create a look-at rotation matrix
        public void LookAt(Vector3d dir, Vector3d up)
        {
            //Vector3d dir = new Vector3d(direction.x, direction.y, direction.z);
            //dir.Normalize();
            Vector3d vx = up.Cross(dir);
            vx.Normalize();
            Vector3d vy = dir.Cross(vx);
            vy.Normalize();
            Matrix[0, 0] = vx.x;  Matrix[0, 1] = vx.y;  Matrix[0, 2] = vx.z;  Matrix[0, 3] = 0;
            Matrix[1, 0] = vy.x;  Matrix[1, 1] = vy.y;  Matrix[1, 2] = vy.z;  Matrix[1, 3] = 0;
            Matrix[2, 0] = dir.x; Matrix[2, 1] = dir.y; Matrix[2, 2] = dir.z; Matrix[2, 3] = 0;
            Matrix[3, 0] = 0;     Matrix[3, 1] = 0;     Matrix[3, 2] = 0;     Matrix[3, 3] = 1;
        }

        public Vector3d Transform(Vector3d V)
        {
            Point3d pnt = new Point3d(V.x, V.y, V.z);
            pnt = Transform(pnt);
            return new Vector3d(pnt.x, pnt.y, pnt.z);
        }
    }
}
