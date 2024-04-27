using JG_GL;
using SharpGL.SceneGraph;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Texturing_Cloth
{
    internal class ClothGeometry : GLGeometry
    {
        private readonly int my_Width;
        private readonly int my_Height;
        private Particle[,] my_Particles;
        private List<Spring> Springs;
        private double SpringLength;
        private double my_Damping = 0.998;
        private double my_ParticleMass = 5.0;
        private double SpringStiffness = 0.8;
        private bool ShowParticles = false;

        /// <summary>
        /// Create a Cloth Geometry that consists of particles connected with springs.
        /// <para>The Cloth is centered around (0,0,0) and lies in the X,Y plane.</para>
        /// </summary>
        /// <param name="width">The number of particles in one row</param>
        /// <param name="height">The number of rows in the Cloth</param>
        /// <param name="cellSize">The distance between 2 particles</param>
        public ClothGeometry(int width, int height, double cellSize) 
        {
            my_Width = width;
            my_Height = height;
            SpringLength = cellSize;
            my_Particles = new Particle[my_Width, my_Height];
            Springs = new List<Spring>();
            //Step 1: Create the 3D Cloth as a grid of Particles
            double x, y;
            for (int w = 0; w < my_Width; w++)
            {
                for (int h = 0; h < my_Height; h++)
                {
                    x = SpringLength * (w - my_Width / 2);
                    y = SpringLength * (h - my_Height / 2);
                    my_Particles[w, h] = new Particle(my_ParticleMass, new Vector3D(x, y, 0), ShowParticles);
                    my_Particles[w, h].Damping = my_Damping;
                }
            }
            //Step 2: Connect the Particles with Springs
            for (int w = 0; w < my_Width - 1; w++)
            {
                for (int h = 0; h < my_Height - 1; h++)
                {
                    Spring sp1 = new Spring(my_Particles[w, h], my_Particles[w + 1, h]);
                    Spring sp2 = new Spring(my_Particles[w, h], my_Particles[w, h + 1]);
                    sp1.Stiffness = SpringStiffness;
                    sp2.Stiffness = SpringStiffness;
                    Springs.Add(sp1);
                    Springs.Add(sp2);
                }
            }
            //Connect the last row
            for (int w = 0; w < my_Width - 1; w++)
            {
                Spring sp1 = new Spring(my_Particles[w, my_Height - 1], my_Particles[w + 1, my_Height - 1]);
                sp1.Stiffness = SpringStiffness;
                Springs.Add(sp1);
            }
            //Connect the last column
            for (int h = 0; h < my_Height - 1; h++)
            {
                Spring sp2 = new Spring(my_Particles[my_Width - 1, h], my_Particles[my_Width - 1, h + 1]);
                sp2.Stiffness = SpringStiffness;
                Springs.Add(sp2);
            }
            my_VertexCount = width * height;
        }

        public double Stiffness
        {
            get { return SpringStiffness; }
            set { SpringStiffness = value; }
        }

        public double Damping
        {
            get { return my_Damping; }
            set { my_Damping = value; }
        }

        public double ParticleMass
        {
            get { return my_ParticleMass; }
            set { my_ParticleMass = value; }
        }

        public Particle[,] Particles
        {
            get { return my_Particles; }
        }

        public void Update(Vector3D gravity)
        {
            for (int i = 0; i < Springs.Count; i++)
            {
                Springs[i].Update(gravity);
            }
        }

        protected override void CreateVertices()
        {
            Matrix3D rm = CalculateRotationMatrix(InitialRotationAxis.X, InitialRotationAxis.Y, InitialRotationAxis.Z);
            my_vertices = new Vector3D[my_VertexCount];
            double x, y;
            int count = 0;
            for (int w = 0; w < my_Width; w++)
            {
                for (int h = 0; h < my_Height; h++)
                {
                    my_vertices[count] = Particles[w,h].Position;
                    count += 1;
                }
            }
            //Apply the initial rotation
            for (int I = 0; I < my_vertices.Length; I++)
            {
                my_vertices[I] = rm.Transform(my_vertices[I]);
            }
        }

        protected override void CreateNormals()
        {
            //Calculate the normals for each vertex position as the cross product of 2 vectors
            my_normals = new Vector3D[my_VertexCount];
            Matrix3D rm = CalculateRotationMatrix(InitialRotationAxis.X, InitialRotationAxis.Y, InitialRotationAxis.Z);
            int count = 0;
            Vector3D V1;
            Vector3D V2;
            for (int X = 0; X < my_Width; X++)
            {
                for (int Y = 0; Y < my_Height; Y++)
                {
                    if (X < my_Width - 1 & Y < my_Height - 1)
                    {
                        V1 = my_vertices[(X + 1) * my_Height + Y] - my_vertices[X * my_Height + Y];
                        V2 = my_vertices[X * my_Height + Y + 1] - my_vertices[X * my_Height + Y];
                        my_normals[count] = Vector3D.CrossProduct(V2, V1);
                    }
                    else if (X == my_Width - 1)
                    {
                        my_normals[count] = my_normals[count - my_Height + 1];
                    }
                    else if (Y == my_Height - 1)
                    {
                        my_normals[count] = my_normals[count - 1];
                    }
                    count++;
                }
            }
            //Apply the initial rotation
            for (int I = 0; I < my_normals.Length; I++)
            {
                my_normals[I] = rm.Transform(my_normals[I]);
            }
        }

        protected override void CreateIndices()
        {
            int count = 0;
            int K1;
            int K2;
            my_indices = new int[6 * (my_Width - 1) * (my_Height - 1)];
            //Calculate the Indices for each cell
            //Triangles = (K1, K+1, K2) - (K2, K1+1, K2+1)
            for (int Y = 0; Y < my_Width - 1; Y++)
            {
                K1 = Y * my_Height;
                K2 = (Y + 1) * my_Height;
                for (int X = 0; X < my_Height - 1; X++)
                {
                    my_indices[count] = K1;
                    count += 1;
                    my_indices[count] = K2;
                    count += 1;
                    my_indices[count] = K1 + 1;
                    count += 1;
                    my_indices[count] = K1 + 1;
                    count += 1;
                    my_indices[count] = K2;
                    count += 1;
                    my_indices[count] = K2 + 1;
                    count += 1;
                    K1++;
                    K2++;
                }
            }
        }

        protected override void CreateTexCoordinates()
        {
            my_textureCoords = new Vector[my_VertexCount];
            int count = 0;
            //Calculate the Texture Coordinates for each vertex position
            for (int X = 0; X < my_Width; X++)
            {
                for (int Y = 0; Y < my_Height; Y++)
                {
                    my_textureCoords[count] = new Vector(X / (my_Width - 1.0), Y / (my_Height - 1.0));
                    count += 1;
                }
            }
        }

        public override Vector3D GetVertexLayout()
        {
            return new Vector3D(my_Width, my_Height, 0.0);
        }
    }
}
