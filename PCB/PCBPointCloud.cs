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

        public void Draw_Plane(float Length, float Width, float Height, List<Point3f> Plane_MidPoint, out List<Point3f> PointCloud_Plane)
        {
            PointCloud_Plane = new List<Point3f>();

            float Increment = 0.05f;
            float Ending_Increment = 0.01f;

            float StartingX, EndingX;
            float StartingY, EndingY;
            float StartingZ, EndingZ;

            for (int Iterator = 0; Iterator < Plane_MidPoint.Count; Iterator ++)
            {
                StartingX = Plane_MidPoint[Iterator].x - Length;
                EndingX = Plane_MidPoint[Iterator].x + Length;

                StartingY = Plane_MidPoint[Iterator].y - Width;
                EndingY = Plane_MidPoint[Iterator].y + Width;

                StartingZ = Plane_MidPoint[Iterator].z - Height;
                EndingZ = Plane_MidPoint[Iterator].z + Height;

                if (Iterator == 0 || Iterator == 1) // Top and Bottom Plane
                {
                    StartingZ = Plane_MidPoint[Iterator].z;
                    EndingZ = Plane_MidPoint[Iterator].z;
                }
                else if (Iterator == 2 || Iterator == 3)  // Front and Back Plane
                {
                    StartingY = Plane_MidPoint[Iterator].y;
                    EndingY = Plane_MidPoint[Iterator].y;
                }
                else if (Iterator == 4 || Iterator == 5) // Left and Right Plane
                {
                    StartingX = Plane_MidPoint[Iterator].x;
                    EndingX = Plane_MidPoint[Iterator].x;
                }

                for (float IteratorY = StartingY; IteratorY < (EndingY + Ending_Increment); IteratorY += Increment)
                {
                    for (float IteratorX = StartingX; IteratorX < (EndingX + Ending_Increment); IteratorX += Increment)
                    {
                        for (float IteratorZ = StartingZ; IteratorZ < (EndingZ + Ending_Increment); IteratorZ += Increment)
                        {
                            Point3f Generate_Plane_Point = new Point3f();

                            Generate_Plane_Point.x = IteratorX;
                            Generate_Plane_Point.y = IteratorY;
                            Generate_Plane_Point.z = IteratorZ;

                            PointCloud_Plane.Add(Generate_Plane_Point);
                        }
                    }
                }
            }
        }

        public void Draw_Box(List<Component> PCBWay_Input)
        {
            string FilePath = @"C:\Users\USER\Desktop\test123.txt"; // set the file path

            FileStream fs1 = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Write); // Create a new text file
            StreamWriter writer = new StreamWriter(fs1);

            float i = 200;
            float z = 60; // coordinate z
            float SizeX, SizeY, SizeZ;
            float Mid_Length, Mid_Width, Mid_Height;

            List<Point3f> Generate_Plane_Point = new List<Point3f>();
            List<Point3f> Store_BoxPlane_Point = new List<Point3f>();

            for (int Iterator = 0; Iterator < PCBWay_Input.Count; Iterator++)
            {
                List<Point3f> Plane_MidPoint = new List<Point3f>();
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

                Draw_Plane(Mid_Length, Mid_Width, Mid_Height, Plane_MidPoint, out Generate_Plane_Point); // call draw plane function

                for (int IteratorTest = 0; IteratorTest < Generate_Plane_Point.Count; IteratorTest++)
                {                    
                    writer.Write(Generate_Plane_Point[IteratorTest].x.ToString("0.0000") + "    " + Generate_Plane_Point[IteratorTest].y.ToString("0.0000") + "    " + Generate_Plane_Point[IteratorTest].z.ToString("0.0000") + "    " + i.ToString("0000") + "\n");
                    Store_BoxPlane_Point.Add(Generate_Plane_Point[IteratorTest]);
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
