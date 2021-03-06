using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCB
{
    class PCBPointCloud
    {
        public OpenFileDialog openModel;

        const int NUMBER_PLANE = 6;

        const int TOP_PLANE = 0;
        const int BOTTOM_PLANE = 1;
        const int RIGHT_PLANE = 2;
        const int LEFT_PLANE = 3;
        const int BACK_PLANE = 4;
        const int FRONT_PLANE = 5;

        const int ROTATE_X = 0;
        const int ROTATE_Y = 1;
        const int ROTATE_Z = 2;

        const float RESOLUTION = 0.03f; //100 points for every mm

        StreamWriter writer;

        public void Load_Model(ref float[] tempPoints, int VertexAttribSize, out int PointCloudSize)
        {

            string fileName;
            int i = 0;

            PointCloudSize = 0;

            string FilePath = @"D:\PCB\test123.txt"; // set the file path

            FileStream fs1 = new FileStream(FilePath, FileMode.Create, FileAccess.Write); // Create a new text file
            writer = new StreamWriter(fs1);

            //this.openModel = new OpenFileDialog();
            //if (this.openModel.ShowDialog() != DialogResult.OK)
            //{
            //    return;
            //}
            //else
            //{
            //    fileName = this.openModel.FileName;
            //}

            fileName = @"D:\PCB\PCB Centroid.txt";
            string str = System.IO.Path.GetExtension(fileName).ToLower();
            if (str == ".obj")
            {
                //this.PointCloud = PointCloud.FromObjFile(fileName);
            }
            if (str == ".xyz" || str == ".txt")
            {

                try
                {
                    string[] lines = System.IO.File.ReadAllLines(fileName);
                    //PointCloudSize = lines.GetLength(0) * VertexAttribSize;

                    List<Component> pcbWay = new List<Component>();

                    PCBWayList(lines, out pcbWay);
                    Component_Data_Control(pcbWay);
                }
                catch (Exception err)
                {
                    System.Windows.Forms.MessageBox.Show("Read_XYZ: read error in file :  " + fileName + " : at line: " + i.ToString() + " ; " + err.Message);
                }
            }
        }

        public void PCBWayList(string[] lines, out List<Component> PCBWay_Output)
        {
            PCBWay_Output = new List<Component>();

            for (int Iterator = 0; Iterator < lines.GetLength(0); Iterator++)
            {
                string[] arrStr1 = lines[Iterator].Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Component pcb = new Component();
                pcb.designator = (Convert.ToString(arrStr1[0]));
                pcb.footprint = (Convert.ToString(arrStr1[1]));
                pcb.midX = (Convert.ToSingle(arrStr1[2]));
                pcb.midY = (Convert.ToSingle(arrStr1[3]));
                pcb.refX = (Convert.ToSingle(arrStr1[4]));
                pcb.refY = (Convert.ToSingle(arrStr1[5]));
                pcb.padX = (Convert.ToSingle(arrStr1[6]));
                pcb.padY = (Convert.ToSingle(arrStr1[7]));
                pcb.topbottom = (Convert.ToString(arrStr1[8]));
                pcb.rotation = (Convert.ToSingle(arrStr1[9]));
                pcb.comment = (Convert.ToString(arrStr1[10]));
                PCBWay_Output.Add(pcb);
            }
        }

        public void Component_Data_Control(List<Component> PCBWay_Input)
        {
            for (int Iterator = 0; Iterator < PCBWay_Input.Count; Iterator++)
            {
                Coordinate Position = new Coordinate();
                Angle Rotation = new Angle();

                Length_Size Component_Length = new Length_Size();

                float z = 600; // coordinate z
                float Convert_Value = 0.1f;

                Component_Length.x = int.Parse(PCBWay_Input[Iterator].footprint.Substring(0, 2));  // Record is 0603R  (This variable just get 06);
                Component_Length.y = int.Parse(PCBWay_Input[Iterator].footprint.Substring(2, 2));  // Record is 0603R  (This variable just get 03);
                Component_Length.z = 1;

                Position.x = PCBWay_Input[Iterator].midX * Convert_Value;  // Record Mid X is 660.236mil, change it to 66.0236 
                Position.y = PCBWay_Input[Iterator].midY * Convert_Value;  // Record Mid Y is 1603.937mil, change it to 160.3937
                Position.z = z * Convert_Value;

                Rotation.x = 0;
                Rotation.y = 0;
                Rotation.z = 0;

                Draw_Box(Component_Length, Position, Rotation);
            }
        }
        public void Draw_Box(Length_Size Component_Length, Coordinate Position, Angle Rotation)
        {
            Coordinate MidPoint = new Coordinate();
            Coordinate RotationMidPoint = new Coordinate();      

            for (int InteratorMid = 0; InteratorMid < NUMBER_PLANE; InteratorMid++)
            {
                if (InteratorMid == TOP_PLANE)
                {
                    MidPoint.x = Position.x;
                    MidPoint.y = Position.y;
                    MidPoint.z = (float)(Position.z + Component_Length.z * 0.5);

                    // Top plane
                    Draw_Plane(Rotate_Angle(RotationMidPoint, MidPoint, Position, Rotation),
                        Component_Length.x, Component_Length.y, Component_Length.z, 0 + Rotation.x, 0 + Rotation.y, 0 + Rotation.z);
                }
                else if (InteratorMid == BOTTOM_PLANE)
                {
                    MidPoint.x = Position.x;
                    MidPoint.y = Position.y;
                    MidPoint.z = (float)(Position.z - Component_Length.z * 0.5);

                    // Bottom plane
                    Draw_Plane(Rotate_Angle(RotationMidPoint, MidPoint, Position, Rotation), 
                        Component_Length.x, Component_Length.y, Component_Length.z, 0 + Rotation.x, 0 + Rotation.y, 0 + Rotation.z);
                }
                else if (InteratorMid == RIGHT_PLANE)
                {
                    MidPoint.x = (float)(Position.x + Component_Length.x * 0.5);
                    MidPoint.y = Position.y;
                    MidPoint.z = Position.z;

                    // Right plane
                    Draw_Plane(Rotate_Angle(RotationMidPoint, MidPoint, Position, Rotation),
                        Component_Length.z, Component_Length.y, Component_Length.x, 0 + Rotation.x, 90 + Rotation.y, 0 + Rotation.z);
                }
                else if (InteratorMid == LEFT_PLANE)
                {
                    MidPoint.x = (float)(Position.x - Component_Length.x * 0.5);
                    MidPoint.y = Position.y;
                    MidPoint.z = Position.z;

                    // Left plane
                    Draw_Plane(Rotate_Angle(RotationMidPoint, MidPoint, Position, Rotation),
                        Component_Length.z, Component_Length.y, Component_Length.x, 0 + Rotation.x, 90 + Rotation.y, 0 + Rotation.z);
                }
                else if (InteratorMid == BACK_PLANE)
                {
                    MidPoint.x = Position.x;
                    MidPoint.y = (float)(Position.y + Component_Length.y * 0.5);
                    MidPoint.z = Position.z;

                    // Back plane
                    Draw_Plane(Rotate_Angle(RotationMidPoint, MidPoint, Position, Rotation), 
                        Component_Length.x, Component_Length.z, Component_Length.y, 90 + Rotation.x, 0 + Rotation.y, 0 + Rotation.z); 
                }
                else if (InteratorMid == FRONT_PLANE)
                {
                    MidPoint.x = Position.x;
                    MidPoint.y = (float)(Position.y - Component_Length.y * 0.5);
                    MidPoint.z = Position.z;

                    // Front plane
                    Draw_Plane(Rotate_Angle(RotationMidPoint, MidPoint, Position, Rotation), 
                        Component_Length.x, Component_Length.z, Component_Length.y, 90 + Rotation.x, 0 + Rotation.y, 0 + Rotation.z);
                }
            }

        }
        public void Draw_Plane(Coordinate MidPoint_Coordinate, float Length_X, float Length_Y, float Length_Z, double Rotation_X, double Rotation_Y, double Rotation_Z)
        {
            int PointCloudLength = (int)(((Length_X + 1) / RESOLUTION) * ((Length_Y + 1) / RESOLUTION) + 50);

            Coordinate[] PointCloud_Plane = new Coordinate[PointCloudLength];

            int PointIndex = 0;
            int intensity = 150;

            float StartingX;
            float StartingY;
            float StartingZ;

            StartingX = (float)(MidPoint_Coordinate.x - (Length_X * 0.5));
            StartingY = (float)(MidPoint_Coordinate.y - (Length_Y * 0.5));
            StartingZ = (float)(MidPoint_Coordinate.z);

            //generate flat plane according to coordinate and size specified
            for (int Iterator_Y = 0; Iterator_Y < (Length_Y / RESOLUTION); Iterator_Y++)
            {
                for (int Iterator_X = 0; Iterator_X < (Length_X / RESOLUTION); Iterator_X++)
                {
                    PointCloud_Plane[PointIndex] = new Coordinate();
                    PointCloud_Plane[PointIndex].x = StartingX + (Iterator_X * RESOLUTION);
                    PointCloud_Plane[PointIndex].y = StartingY + (Iterator_Y * RESOLUTION); ;
                    PointCloud_Plane[PointIndex].z = StartingZ;

                    PointIndex++;
                }
            }

            Rotation_X *= (Math.PI / 180);
            Rotation_Y *= (Math.PI / 180);
            Rotation_Z *= (Math.PI / 180);
            //Origin of rotation to be coordinate from Mid_Coordinate            

            for (int Iterator = 0; Iterator < PointIndex; Iterator++)
            {
                PointCloud_Plane[Iterator] = Rotate_X(PointCloud_Plane[Iterator], MidPoint_Coordinate, Rotation_X);
                PointCloud_Plane[Iterator] = Rotate_Y(PointCloud_Plane[Iterator], MidPoint_Coordinate, Rotation_Y);
                PointCloud_Plane[Iterator] = Rotate_Z(PointCloud_Plane[Iterator], MidPoint_Coordinate, Rotation_Z);

                PointCloud_Plane[Iterator] = Rotate_X(PointCloud_Plane[Iterator], MidPoint_Coordinate, 30 * (Math.PI / 180));
                PointCloud_Plane[Iterator] = Rotate_Y(PointCloud_Plane[Iterator], MidPoint_Coordinate, 0);
                PointCloud_Plane[Iterator] = Rotate_Z(PointCloud_Plane[Iterator], MidPoint_Coordinate, 0);
            }

            

            for (int Iterator = 0; Iterator < PointIndex; Iterator++)
            {
                writer.Write(PointCloud_Plane[Iterator].x.ToString("0.0000") + "    " + PointCloud_Plane[Iterator].y.ToString("0.0000") + "    " + PointCloud_Plane[Iterator].z.ToString("0.0000") + "    " + intensity.ToString("0000") + "\n");
            }
        }

        //Rotate X
        public Coordinate Rotate_X(Coordinate Input_Coordinate, Coordinate Origin_Coordinate, double Rotation_Angle)
        {
            return Rotate3DAroundOrigin(ROTATE_X, Input_Coordinate, Origin_Coordinate, Rotation_Angle);
        }

        //Rotate Y
        public Coordinate Rotate_Y(Coordinate Input_Coordinate, Coordinate Origin_Coordinate, double Rotation_Angle)
        {
            return Rotate3DAroundOrigin(ROTATE_Y, Input_Coordinate, Origin_Coordinate, Rotation_Angle);
        }

        //Rotate Z
        public Coordinate Rotate_Z(Coordinate Input_Coordinate, Coordinate Origin_Coordinate, double Rotation_Angle)
        {
            return Rotate3DAroundOrigin(ROTATE_Z, Input_Coordinate, Origin_Coordinate, Rotation_Angle);
        }

        private Coordinate Rotate3DAroundOrigin(int RotationType, Coordinate Input_Coordinate, Coordinate Origin, double Rotation_Angle)
        {
            float X = 0, Y = 0, Z = 0;

            Input_Coordinate.x -= Origin.x;
            Input_Coordinate.y -= Origin.y;
            Input_Coordinate.z -= Origin.z;

            switch(RotationType)
            {
                case ROTATE_X:
                    X = Input_Coordinate.x;
                    Y = Input_Coordinate.y;
                    Z = Input_Coordinate.z;

                    Input_Coordinate.y = (float)((Y * Math.Cos(Rotation_Angle) - (Z * Math.Sin(Rotation_Angle))));
                    Input_Coordinate.z = (float)((Y * Math.Sin(Rotation_Angle) + (Z * Math.Cos(Rotation_Angle))));
                    break;
                case ROTATE_Y:
                    X = Input_Coordinate.x;
                    Y = Input_Coordinate.y;
                    Z = Input_Coordinate.z;

                    Input_Coordinate.x = (float)((Z * Math.Sin(Rotation_Angle) + (X * Math.Cos(Rotation_Angle))));
                    Input_Coordinate.z = (float)((Z * Math.Cos(Rotation_Angle) - (X * Math.Sin(Rotation_Angle))));
                    break;
                case ROTATE_Z:
                    X = Input_Coordinate.x;
                    Y = Input_Coordinate.y;
                    Z = Input_Coordinate.z;

                    Input_Coordinate.x = (float)((X * Math.Cos(Rotation_Angle) - (Y * Math.Sin(Rotation_Angle))));
                    Input_Coordinate.y = (float)((X * Math.Sin(Rotation_Angle) + (Y * Math.Cos(Rotation_Angle))));
                    break;
            }

            Input_Coordinate.x += Origin.x;
            Input_Coordinate.y += Origin.y;
            Input_Coordinate.z += Origin.z;

            return Input_Coordinate;
        }

        //RotationMidPoint = The midpoint after rotate
        //MidPoint = The origin midpoint of a plane / point to rotate
        //Position = The box midpoint and center point of rotation
        //Rotation = The rotation angle in degrees

        public Coordinate Rotate_Angle(Coordinate RotationMidPoint, Coordinate MidPoint, Coordinate Position, Angle Rotation)
        {
            double angleInRadians;
            double cosTheta;
            double sinTheta;

            if (Rotation.x != 0)
            {
                angleInRadians = Rotation.x * (Math.PI / 180);
                cosTheta = Math.Cos(angleInRadians);
                sinTheta = Math.Sin(angleInRadians);

                RotationMidPoint.x = MidPoint.x;
                RotationMidPoint.y = (float)(cosTheta * (MidPoint.y - Position.y) - sinTheta * (MidPoint.z - Position.z) + Position.y);
                RotationMidPoint.z = (float)(sinTheta * (MidPoint.y - Position.y) + cosTheta * (MidPoint.z - Position.z) + Position.z);
                return RotationMidPoint;
            }

            if(Rotation.y != 0)
            {
                angleInRadians = Rotation.y * (Math.PI / 180);
                cosTheta = Math.Cos(angleInRadians);
                sinTheta = Math.Sin(angleInRadians);

                RotationMidPoint.x = (float)(cosTheta * (MidPoint.x - Position.x) + sinTheta * (MidPoint.z - Position.z) + Position.x);
                RotationMidPoint.y = MidPoint.y;
                RotationMidPoint.z = (float)(cosTheta * (MidPoint.z - Position.z) - sinTheta * (MidPoint.x - Position.x) + Position.z);
              
                return RotationMidPoint;
            }

            if (Rotation.z != 0)
            {
                angleInRadians = Rotation.z * (Math.PI / 180);
                cosTheta = Math.Cos(angleInRadians);
                sinTheta = Math.Sin(angleInRadians);

                RotationMidPoint.x = (float)(cosTheta * (MidPoint.x - Position.x) - sinTheta * (MidPoint.y - Position.y) + Position.x);
                RotationMidPoint.y = (float)(sinTheta * (MidPoint.x - Position.x) + cosTheta * (MidPoint.y - Position.y) + Position.y);
                RotationMidPoint.z = MidPoint.z;
                return RotationMidPoint;
            }

            return MidPoint; // not rotate return origin midpoint
        }

    }

    public class Coordinate
    {
        public float x, y, z;
    }

    public class Length_Size
    {
        public float x, y, z;
    }

    public class Angle
    {
        public double  x, y, z;
    }
    public class Component
    {
        public string designator, footprint, topbottom, comment;
        public float midX, midY, refX, refY, padX, padY, rotation;
    }
}
