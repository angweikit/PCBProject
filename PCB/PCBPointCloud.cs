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

        const int Number_Plane = 6;

        const int TOP_PLANE = 0;
        const int BOTTOM_PLANE = 1;
        const int RIGHT_PLANE = 2;
        const int LEFT_PLANE = 3;
        const int BACK_PLANE = 4;
        const int FRONT_PLANE = 5;

        const float RESOLUTION = 0.01f; //100 points for every mm

        StreamWriter writer;

        public void Load_Model(ref float[] tempPoints, int VertexAttribSize, out int PointCloudSize)
        {

            string fileName;
            int i = 0;

            PointCloudSize = 0;

            string FilePath = @"C:\Users\USER\Desktop\Documentation\PCB\test123.txt"; // set the file path

            FileStream fs1 = new FileStream(FilePath, FileMode.Create, FileAccess.Write); // Create a new text file
            writer = new StreamWriter(fs1);

            this.openModel = new OpenFileDialog();
            if (this.openModel.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            else
            {
                fileName = this.openModel.FileName;
            }

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
                    List<Coordinate> Store_BoxPlane_Point = new List<Coordinate>();

                    PCBWayList(lines, out pcbWay);
                    Draw_Box(pcbWay);
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

        public void Draw_Box(List<Component> PCBWay_Input)
        {         
            float Length_X, Length_Y, Length_Z;

            //Coordinate [] Generate_Plane_Point = new Coordinate [3000000];

            for (int Iterator = 0; Iterator < PCBWay_Input.Count; Iterator++)
            {
                Coordinate Position = new Coordinate();
                Coordinate Mid = new Coordinate();

                float z = 600; // coordinate z
                float Convert_Value = 10;

                Length_X = int.Parse(PCBWay_Input[Iterator].footprint.Substring(0, 2));  // Record is 0603R  (This variable just get 06);
                Length_Y = int.Parse(PCBWay_Input[Iterator].footprint.Substring(2, 2));  // Record is 0603R  (This variable just get 03);
                Length_Z = 1;

                Position.x = PCBWay_Input[Iterator].midX / Convert_Value;  // Record Mid X is 660.236mil, change it to 66.0236 
                Position.y = PCBWay_Input[Iterator].midY / Convert_Value;  // Record Mid Y is 1603.937mil, change it to 160.3937
                Position.z = z / Convert_Value;

                for (int InteratorMid = 0; InteratorMid < Number_Plane; InteratorMid++) 
                {               
                    if (InteratorMid == TOP_PLANE)
                    {
                        Mid.x = Position.x;
                        Mid.y = Position.y;
                        Mid.z = (float)(Position.z + Length_Z * 0.5);
                        Draw_Plane(Length_X, Length_Y, Length_Z, 0, 0, 0, Mid); // call draw plane function
                    }
                    else if(InteratorMid == BOTTOM_PLANE)
                    {
                        Mid.x = Position.x;
                        Mid.y = Position.y;
                        Mid.z = (float)(Position.z - Length_Z * 0.5);
                        Draw_Plane(Length_X, Length_Y, Length_Z, 0, 0, 0, Mid); // call draw plane function
                    }
                    else if (InteratorMid == RIGHT_PLANE)
                    {
                        Mid.x = (float)(Position.x + Length_X * 0.5);
                        Mid.y = Position.y;
                        Mid.z = Position.z;
                        Draw_Plane(Length_Y, Length_Z, Length_X, 90, 0, 90, Mid); // call draw plane function
                    }
                    else if (InteratorMid == LEFT_PLANE)
                    {
                        Mid.x = (float)(Position.x - Length_X * 0.5);
                        Mid.y = Position.y;
                        Mid.z = Position.z;
                        Draw_Plane(Length_Y, Length_Z, Length_X, 90, 0, 90, Mid); // call draw plane function
                    }
                    else if (InteratorMid == BACK_PLANE)
                    {
                        Mid.x = Position.x;
                        Mid.y = (float)(Position.y + Length_Y * 0.5);
                        Mid.z = Position.z;
                        Draw_Plane(Length_X, Length_Z, Length_Y, 90, 0, 0, Mid); // call draw plane function
                    }
                    else if (InteratorMid == FRONT_PLANE)
                    {
                        Mid.x = Position.x;
                        Mid.y = (float)(Position.y - Length_Y * 0.5);
                        Mid.z = Position.z;
                        Draw_Plane(Length_X, Length_Z, Length_Y, 90, 0, 0, Mid); // call draw plane function
                    }
                }    
            }
            //writer.Close();
        }

        public void Draw_Plane(float Length_X, float Length_Y, float Length_Z, double Rotation_X, double Rotation_Y, double Rotation_Z, Coordinate Plane_MidPoint)
        {
            int PointCloudLength = (int)(((Length_X + 1) / RESOLUTION) * ((Length_Y + 1) / RESOLUTION) + 50);

            Coordinate[] PointCloud_Plane = new Coordinate[PointCloudLength];

            int PointIndex = 0;

            float StartingX;
            float StartingY;
            float StartingZ;

            StartingX = (float)(Plane_MidPoint.x - (Length_X * 0.5));
            StartingY = (float)(Plane_MidPoint.y - (Length_Y * 0.5));
            StartingZ = (float)(Plane_MidPoint.z - (Length_Z * 0.5));

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
            //Origin of rotation to be coordinate from Plane_MidPoint            

            if (Rotation_X != 0 || (Rotation_X == 0 && Rotation_Y == 0 && Rotation_Z == 0))
            {
                for (int Iterator = 0; Iterator < PointIndex; Iterator++)
                {
                    Coordinate Origin = Plane_MidPoint;
                    float Hypotenuse = PointCloud_Plane[Iterator].y - Origin.y;

                    PointCloud_Plane[Iterator].y = Origin.y + (float)(Hypotenuse * Math.Cos(Rotation_X));
                    PointCloud_Plane[Iterator].z = Origin.z + (float)(Hypotenuse * Math.Sin(Rotation_X));

                    if (Rotation_Z == 0)
                    {
                        writer.Write(PointCloud_Plane[Iterator].x.ToString("0.0000") + "    " + PointCloud_Plane[Iterator].y.ToString("0.0000") + "    " + PointCloud_Plane[Iterator].z.ToString("0.0000") + "    200\n");
                    }
                }
            }

            if (Rotation_Y != 0)
            {
                for (int Iterator = 0; Iterator < PointIndex; Iterator++)
                {
                    Coordinate Origin = Plane_MidPoint;
                    float Hypotenuse = PointCloud_Plane[Iterator].x - Origin.x;

                    PointCloud_Plane[Iterator].x = Origin.x + (float)(Hypotenuse * Math.Cos(Rotation_Y));
                    PointCloud_Plane[Iterator].z = Origin.z + (float)(Hypotenuse * Math.Sin(Rotation_Y));

                    writer.Write(PointCloud_Plane[Iterator].x.ToString("0.0000") + "    " + PointCloud_Plane[Iterator].y.ToString("0.0000") + "    " + PointCloud_Plane[Iterator].z.ToString("0.0000") + "    200\n");
                }
            }

            if (Rotation_Z != 0)
            {
                for (int Iterator = 0; Iterator < PointIndex; Iterator++)
                {
                    Coordinate Origin = Plane_MidPoint;
                    float Hypotenuse = PointCloud_Plane[Iterator].x - Origin.x;

                    PointCloud_Plane[Iterator].x = Origin.x + (float)(Hypotenuse * Math.Cos(Rotation_Z));
                    PointCloud_Plane[Iterator].y = Origin.y + (float)(Hypotenuse * Math.Sin(Rotation_Z));

                    writer.Write(PointCloud_Plane[Iterator].x.ToString("0.0000") + "    " + PointCloud_Plane[Iterator].y.ToString("0.0000") + "    " + PointCloud_Plane[Iterator].z.ToString("0.0000") + "    200\n");
                }
            }
        }

        //public void Converter_Unit()
        //{
        //    string Convert_Unit= "mil";
        //    float Convert_Value = 0;

        //    if (Convert_Unit == "mil")
        //    {
        //        Convert_Value = 10;
        //    }

        //    
        //}
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
        public float x, y, z;
    }
    public class Component
    {
        public string designator, footprint, topbottom, comment;
        public float midX, midY, refX, refY, padX, padY, rotation;
    }
}
