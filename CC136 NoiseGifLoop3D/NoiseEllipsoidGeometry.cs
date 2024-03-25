using JG_GL;
using JG_Math;
using System.Windows;
using System.Windows.Media.Media3D;

public class NoiseEllipsoidGeometry : GLGeometry
{
    private double my_X_Size;
    private double my_Y_Size;
    private double my_Z_Size;
    private int my_stacks;
    private int my_slices;
    private int my_Percent;

    public NoiseEllipsoidGeometry(double X_Size, double Y_Size, double Z_Size)
    {
        my_X_Size = X_Size;
        my_Y_Size = Y_Size;
        my_Z_Size = Z_Size;
        my_stacks = 16;
        my_slices = 32;
        my_Percent = 0;
        my_VertexCount = my_stacks * (my_slices + 1);
    }

    /// <summary>
    /// Create a new Ellipsoid
    /// </summary>
    /// <param name="stacks">The geometry is divided into horizontal stacks</param>
    /// <param name="slices">Each stack is divides into slices</param>
    public NoiseEllipsoidGeometry(double X_Size, double Y_Size, double Z_Size, int stacks, int slices)
    {
        my_X_Size = X_Size;
        my_Y_Size = Y_Size;
        my_Z_Size = Z_Size;
        my_stacks = stacks;
        my_slices = slices;
        my_Percent = 0;
        my_VertexCount = my_stacks * (my_slices + 1);
    }

    /// <summary>
    /// Create a new Ellipsoid with size 1 along all axes (= sphere)
    /// </summary>
    /// <param name="stacks">The geometry is divided into horizontal stacks</param>
    /// <param name="slices">Each stack is divides into slices</param>
    public NoiseEllipsoidGeometry(int stacks, int slices)
    {
        my_X_Size = 1.0;
        my_Y_Size = 1.0;
        my_Z_Size = 1.0;
        my_stacks = stacks;
        my_slices = slices;
        my_Percent = 0;
        my_VertexCount = my_stacks * (my_slices + 1);
    }

    public double X_Size
    {
        get { return my_X_Size; }
        set { my_X_Size = value; }
    }

    public double Y_Size
    {
        get { return my_Y_Size; }
        set { my_Y_Size = value; }
    }

    public double Z_Size
    {
        get { return my_Z_Size; }
        set { my_Z_Size = value; }
    }

    public int Stacks
    {
        get { return my_stacks; }
        set
        {
            my_stacks = value;
            if (my_stacks < 2) my_stacks = 2;
        }
    }

    public int Slices
    {
        get { return my_slices; }
        set
        {
            my_slices = value;
            if (my_slices < 1) my_slices = 1;
        }
    }

    public int Percent
    {
        get { return my_Percent; }
        set { my_Percent = value; }
    }

    protected override void CreateVertices()
    {
        my_VertexCount = my_stacks * (my_slices + 1);
        Matrix3D rm = CalculateRotationMatrix(InitialRotationAxis.X, InitialRotationAxis.Y, InitialRotationAxis.Z);
        my_vertices = new Vector3D[my_VertexCount];
        int count = 0;
        double ds = 1.0 / (my_stacks - 1);
        double dt = 1.0 / my_slices;
        //Calculate the vertex positions
        double XOff;
        double YOff;
        double ZOff;
        double theta;
        double phi;
        double x;
        double y;
        double z; double Offx;
        double Offy;
        double F = 5.0; //Determines how fast the Noise changes
        double R;
        Offx = Math.Cos(2.0 * my_Percent * Math.PI / 100.0);
        Offy = Math.Sin(2.0 * my_Percent * Math.PI / 100.0);
        R = my_Y_Size * OpenSimplexNoise.Noise3D(Offx, F + Offy, 0);
        R = R / 2 + 0.3;
        for (int I = 0; I <= my_slices; I++)
        {
            my_vertices[count] = new Vector3D(0, R, 0); //Top verteces
            count += 1;
        }
        for (double s = ds; s <= 1 - ds / 2; s += ds)
        {
            for (double t = 0; t <= 1 + dt / 2; t += dt)
            {
                theta = t * 2 * Math.PI; //tracks
                phi = s * Math.PI;  //slices
                XOff = F * Math.Sin(theta) * Math.Sin(phi);
                YOff = F * Math.Cos(phi);
                ZOff = F * Math.Cos(theta) * Math.Sin(phi);
                R = my_Y_Size * OpenSimplexNoise.Noise3D(XOff + Offx, YOff + Offy, ZOff);
                R = R / 2 + 0.3;
                x = R * Math.Sin(theta) * Math.Sin(phi);
                y = R * Math.Cos(phi);
                z = -R * Math.Cos(theta) * Math.Sin(phi);
                my_vertices[count] = new Vector3D(x, y, z);
                count += 1;
            }
        }
        for (int I = 0; I <= my_slices; I++)
        {
            my_vertices[count] = new Vector3D(0, -R, 0); //Bottom verteces
            count += 1;
        }
        //Apply the initial rotation
        for (int I = 0; I < my_vertices.Count(); I++)
        {
            my_vertices[I] = rm.Transform(my_vertices[I]);
        }
    }

    protected override void CreateNormals()
    {
        Matrix3D rm = CalculateRotationMatrix(InitialRotationAxis.X, InitialRotationAxis.Y, InitialRotationAxis.Z);
        my_normals = new Vector3D[my_VertexCount];
        int count = 0;
        double E = 0.01;
        double ds = 1.0 / (my_stacks - 1);
        double dt = 1.0 / my_slices;
        double XOff;
        double YOff;
        double ZOff;
        double theta;
        double phi;
        double x;
        double y;
        double z; double Offx;
        double Offy;
        double F = 5.0; //Determines how fast the Noise changes 
        double R;
        Offx = Math.Cos(2.0 * my_Percent * Math.PI / 100);
        Offy = Math.Sin(2.0 * my_Percent * Math.PI / 100);
        //Calculate the normals for each vertex position
        for (int I = 0; I <= my_slices; I++)
        {
            my_normals[count] = new Vector3D(0, 1, 0); //Normal of the top verteces
            count += 1;
        }
        for (double s = ds; s <= 1 - ds / 2; s += ds)
        {
            for (double t = 0; t <= 1 + dt / 2; t += dt)
            {
                theta = t * 2 * Math.PI; //tracks
                phi = s * Math.PI;  //slices
                XOff = F * Math.Sin(theta) * Math.Sin(phi);
                YOff = F * Math.Cos(phi);
                ZOff = F * Math.Cos(theta) * Math.Sin(phi);
                R = my_Y_Size * OpenSimplexNoise.Noise3D(XOff + Offx, YOff + Offy, ZOff);
                R = R / 2 + 0.3;
                x = R * Math.Sin(theta) * Math.Sin(phi);
                y = R * Math.Cos(phi);
                z = -R * Math.Cos(theta) * Math.Sin(phi);
                Vector3D p = new Vector3D(x, y, z);
                theta = t * 2 * Math.PI; //tracks;
                phi = (s + E) * Math.PI;  //slices;
                XOff = F * Math.Sin(theta) * Math.Sin(phi);
                YOff = F * Math.Cos(phi);
                ZOff = F * Math.Cos(theta) * Math.Sin(phi);
                R = my_Y_Size * OpenSimplexNoise.Noise3D(XOff + Offx, YOff + Offy, ZOff);
                R = R / 2 + 0.3;
                x = R * Math.Sin(theta) * Math.Sin(phi);
                y = R * Math.Cos(phi);
                z = -R * Math.Cos(theta) * Math.Sin(phi);
                Vector3D u = new Vector3D(x, y, z) - p;
                theta = (t + E) * 2 * Math.PI; //tracks
                phi = s * Math.PI;  //slices
                XOff = F * Math.Sin(theta) * Math.Sin(phi);
                YOff = F * Math.Cos(phi);
                ZOff = F * Math.Cos(theta) * Math.Sin(phi);
                R = my_Y_Size * OpenSimplexNoise.Noise3D(XOff + Offx, YOff + Offy, ZOff);
                R = R / 2 + 0.3;
                x = R * Math.Sin(theta) * Math.Sin(phi);
                y = R * Math.Cos(phi);
                z = -R * Math.Cos(theta) * Math.Sin(phi);
                Vector3D v = new Vector3D(x, y, z) - p;
                my_normals[count] = Vector3D.CrossProduct(v, u);
                my_normals[count].Normalize();
                count += 1;
            }
        }
        for (int I = 0; I <= my_slices; I++)
        {
            my_normals[count] = new Vector3D(0, -1, 0); //Normal of the bottom verteces
            count += 1;
        }
        //Apply the initial rotation
        for (int I = 0; I < my_normals.Count(); I++)
        {
            my_normals[I] = rm.Transform(my_normals[I]);
        }
    }

    protected override void CreateIndices()
    {
        int indexCount = 6 * ((my_stacks - 1) * my_slices);
        my_indices = new int[indexCount];
        int count = 0;
        int K1;
        int K2;
        K1 = 0;
        K2 = my_slices + 1;
        for (int I = 0; I < my_stacks - 1; I++)
        {
            for (int J = 1; J <= my_slices; J++)
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
                K1 = K1 + 1;
                K2 = K2 + 1;
            }
            K1 = K1 + 1;
            K2 = K2 + 1;
        }
    }

    protected override void CreateTexCoordinates()
    {
        my_textureCoords = new Vector[my_VertexCount];
        double dist;
        double min = double.MaxValue;
        double max = double.MinValue;
        for (int I = 0; I < my_vertices.Count(); I++)
        {
            dist = Math.Sqrt(my_vertices[I].X * my_vertices[I].X + my_vertices[I].Y * my_vertices[I].Y + my_vertices[I].Z * my_vertices[I].Z);
            if (dist < min) min = dist;
            if (dist > max) max = dist;
        }
        for (int I = 0; I < my_vertices.Count(); I++)
        {
            dist = Math.Sqrt(my_vertices[I].X * my_vertices[I].X + my_vertices[I].Y * my_vertices[I].Y + my_vertices[I].Z * my_vertices[I].Z);
            my_textureCoords[I] = new Vector(0.5, (dist - min) / (max - min));
        }
    }

    /// <summary>
    /// X = number of vertices per stack, Y = number of stacks, Z = 0;
    /// </summary>
    /// <returns></returns>
    public override Vector3D GetVertexLayout()
    {
        return new Vector3D(my_slices + 1, my_stacks, 0);
    }
}
