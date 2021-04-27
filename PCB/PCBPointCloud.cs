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

        public void Load_Model(ref float[] tempPoints, int VertexAttribSize, out int PointCloudSize)
        {

            string fileName;
            int i = 0;

            PointCloudSize = 0;

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
                    List<Point3f> Plane_MidPoint = new List<Point3f>();

                    float Mid_Length = 0;
                    float Mid_Width = 0;
                    float Mid_Height = 0;

                    PCBWayList(lines, out pcbWay);
                    //PointCloud_Generate(pcbWay);
                    Draw_Box(pcbWay, out Mid_Length, out Mid_Width, out Mid_Height, out Plane_MidPoint);
                    Draw_Plane(Mid_Length, Mid_Width, Mid_Height, Plane_MidPoint);
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

        public void Draw_Plane(float Length, float Width, float Height, List<Point3f> Plane_MidPoint)
        {
            List<Point3f> PointCloud_Plane = new List<Point3f>();

            string FilePath = @"C:\Users\USER\Desktop\test123.txt"; // set the file path

            FileStream fs1 = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Write); // Create a new text file
            StreamWriter writer = new StreamWriter(fs1);

            float Increment = 0.05f;

            float StartingX, EndingX;
            float StartingY, EndingY;
            float StartingZ, EndingZ;
            float i = 200;

            for (int Iterator = 0; Iterator < Plane_MidPoint.Count; Iterator++)
            {
                StartingX = Plane_MidPoint[Iterator].x - Length;
                EndingX = Plane_MidPoint[Iterator].x + Length;

                StartingY = Plane_MidPoint[Iterator].y - Width;
                EndingY = Plane_MidPoint[Iterator].y + Width;

                StartingZ = Plane_MidPoint[Iterator].z - Height;
                EndingZ = Plane_MidPoint[Iterator].z + Height;

                if (Iterator == 0 || Iterator == 1)
                {                
                    for (float IteratorY = StartingY; IteratorY < (EndingY + Increment); IteratorY += Increment)
                    {
                        for (float IteratorX = StartingX; IteratorX < (EndingX + Increment); IteratorX += Increment)
                        {
                            Point3f Generate_Plane_Point = new Point3f();

                            Generate_Plane_Point.x = IteratorX;
                            Generate_Plane_Point.y = IteratorY;
                            Generate_Plane_Point.z = Plane_MidPoint[Iterator].z;

                            PointCloud_Plane.Add(Generate_Plane_Point);
                            writer.Write(Generate_Plane_Point.x.ToString("0.0000") + "    " + Generate_Plane_Point.y.ToString("0.0000") + "    " + Generate_Plane_Point.z.ToString("0.0000") + "    " + i.ToString("0000") + "\n");
                        }
                    }
                }
                else if (Iterator == 2 || Iterator == 3)
                {
                    for (float IteratorZ = StartingZ; IteratorZ < (EndingZ); IteratorZ += Increment)
                    {
                        for (float IteratorX = StartingX; IteratorX < (EndingX + Increment); IteratorX += Increment)
                        {
                            Point3f Generate_Plane_Point = new Point3f();

                            Generate_Plane_Point.x = IteratorX;
                            Generate_Plane_Point.y = Plane_MidPoint[Iterator].y;
                            Generate_Plane_Point.z = IteratorZ;

                            PointCloud_Plane.Add(Generate_Plane_Point);
                            writer.Write(Generate_Plane_Point.x.ToString("0.0000") + "    " + Generate_Plane_Point.y.ToString("0.0000") + "    " + Generate_Plane_Point.z.ToString("0.0000") + "    " + i.ToString("0000") + "\n");
                        }
                    }
                }
                else if (Iterator == 4 || Iterator == 5)
                {
                    for (float IteratorZ = StartingZ; IteratorZ < (EndingZ); IteratorZ += Increment)
                    {
                        for (float IteratorY = StartingY; IteratorY < (EndingY + Increment); IteratorY += Increment)
                        {
                            Point3f Generate_Plane_Point = new Point3f();

                            Generate_Plane_Point.x = Plane_MidPoint[Iterator].x;
                            Generate_Plane_Point.y = IteratorY;
                            Generate_Plane_Point.z = IteratorZ;

                            PointCloud_Plane.Add(Generate_Plane_Point);
                            writer.Write(Generate_Plane_Point.x.ToString("0.0000") + "    " + Generate_Plane_Point.y.ToString("0.0000") + "    " + Generate_Plane_Point.z.ToString("0.0000") + "    " + i.ToString("0000") + "\n");
                        }
                    }
                }
            }
            writer.Close();
        }

        public void Draw_Box(List<Component> PCBWay_Input, out float Mid_Length, out float Mid_Width, out float Mid_Height, out List<Point3f> Plane_MidPoint)
        {
            float z = 60;
            float SizeX, SizeY, SizeZ;
            Mid_Length = 0; Mid_Width = 0; Mid_Height = 0;

            Plane_MidPoint = new List<Point3f>();

            for (int Iterator = 0; Iterator < PCBWay_Input.Count; Iterator++)
            {
                Point3f Position = new Point3f();
                SizeX = int.Parse(PCBWay_Input[Iterator].footprint.Substring(0, 2));  // Record is 0603R  (This variable just get 06)
                SizeY = int.Parse(PCBWay_Input[Iterator].footprint.Substring(2, 2));  // Record is 0603R  (This variable just get 03)
                SizeZ = 1;

                Mid_Length = SizeX / 2;
                Mid_Width = SizeY / 2;
                Mid_Height = SizeZ / 2;

                Position.x = PCBWay_Input[Iterator].midX / 10;  // Record Mid X is 660.236mil, change it to 66.0236 
                Position.y = PCBWay_Input[Iterator].midY / 10;  // Record Mid Y is 1603.937mil, change it to 160.3937
                Position.z = z;

                for (int InteratorMid = 0; InteratorMid < 6; InteratorMid++) 
                {
                    Point3f Mid = new Point3f();
                    if (InteratorMid == 0)
                    {
                        Mid.x = Position.x;
                        Mid.y = Position.y;
                        Mid.z = Position.z + Mid_Height;
                    }
                    else if(InteratorMid == 1)
                    {
                        Mid.x = Position.x;
                        Mid.y = Position.y;
                        Mid.z = Position.z - Mid_Height;
                    }
                    else if (InteratorMid == 2)
                    {
                        Mid.x = Position.x;
                        Mid.y = Position.y + Mid_Width;
                        Mid.z = Position.z;
                    }
                    else if (InteratorMid == 3)
                    {
                        Mid.x = Position.x;
                        Mid.y = Position.y - Mid_Width;
                        Mid.z = Position.z;
                    }
                    else if (InteratorMid == 4)
                    {
                        Mid.x = Position.x + Mid_Length;
                        Mid.y = Position.y;
                        Mid.z = Position.z;
                    }
                    else if (InteratorMid == 5)
                    {
                        Mid.x = Position.x - Mid_Length;
                        Mid.y = Position.y;
                        Mid.z = Position.z;
                    }
                    Plane_MidPoint.Add(Mid);
                }             
            }
        }

        public void PointCloud_Generate(List<Component> PCBWay_Input)
        {
            float SizeX, SizeY;

            float MidSizeX;
            float MidSizeY;

            float Z_Top = 61;
            float Z_Bottom = 60;
            float i = 200;

            float PositionX, PositionY;

            float StartingX, EndingX;
            float StartingY, EndingY;
            float Increment = 0.05f;

            string FilePath = @"C:\Users\USER\Desktop\Documentation\PCB\test123.txt"; // set the file path

            FileStream fs1 = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Write); // Create a new text file
            StreamWriter writer = new StreamWriter(fs1);

            for (int Iterator = 0; Iterator < PCBWay_Input.Count; Iterator++)
            {
                Component Processing = new Component();

                Processing = PCBWay_Input[Iterator];

                SizeX = int.Parse(Processing.footprint.Substring(0, 2));  // Record is 0603R  (This variable just get 06)
                SizeY = int.Parse(Processing.footprint.Substring(2, 2));  // Record is 0603R  (This variable just get 03)

                PositionX = Processing.midX / 10;  // Record Mid X is 660.236mil, change it to 66.0236 
                PositionY = Processing.midY / 10;  // Record Mid Y is 1603.937mil, change it to 160.3937

                MidSizeX = SizeX / 2;  // Find the mid of size X (The middle of 6 is 3)
                MidSizeY = SizeY / 2;  // Find the mid of size Y (The middle of 3 is 1.5)

                // Find the start and end point
                StartingX = PositionX - MidSizeX; 
                EndingX = PositionX + MidSizeX;

                StartingY = PositionY - MidSizeY;
                EndingY = PositionY + MidSizeY;

                for (float IteratorY = StartingY; IteratorY < (EndingY + Increment); IteratorY += Increment) 
                {
                    for (float IteratorX = StartingX; IteratorX < (EndingX + Increment); IteratorX += Increment)
                    {
                        // First plane on top
                        writer.Write(IteratorX.ToString("0.0000") + "    " + IteratorY.ToString("0.0000") + "    " + Z_Top.ToString("0.0000") + "    " + i.ToString("0000") + "\n");
                        // Second plane on bottom
                        writer.Write(IteratorX.ToString("0.0000") + "    " + IteratorY.ToString("0.0000") + "    " + Z_Bottom.ToString("0.0000") + "    " + i.ToString("0000") + "\n");

                        if (IteratorY == StartingY || IteratorY == EndingY || IteratorX == StartingX || IteratorX == EndingX)
                        {
                            for (float IteratorZ = Z_Bottom; IteratorZ < Z_Top; IteratorZ += Increment)
                            {
                                writer.Write(IteratorX.ToString("0.0000") + "    " + IteratorY.ToString("0.0000") + "    " + IteratorZ.ToString("0.0000") + "    " + i.ToString("0000") + "\n");
                            }
                        }
                        
                    }
                }

            }
            writer.Close();
        }
    }

    public class Point3f
    {
        public float x, y, z;
    }   
    public class Component
    {
        public string designator, footprint, topbottom, comment;
        public float midX, midY, refX, refY, padX, padY, rotation;
    }
}
