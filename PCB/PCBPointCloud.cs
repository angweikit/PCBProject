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

            string FilePath = @"C:\Users\USER\Desktop\Documentation\PCB.txt"; // set the file path

            FileStream fs1 = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Write); // Create a new text file
            StreamWriter writer = new StreamWriter(fs1);    

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
                    PointCloud_Generate(pcbWay);


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

        public void Draw_Plane(float Length, float Width, Point3f Position, out List<Point3f> PointCloud_Plane)
        {
            PointCloud_Plane = new List<Point3f>();

            float Increment = 0.05f;
            float Mid_Length;
            float Mid_Width;

            float StartingX, EndingX;
            float StartingY, EndingY;

            Mid_Length = Length / 2;
            Mid_Width = Width / 2;

            StartingX = Position.x - Mid_Length;
            EndingX = Position.x - Mid_Length;

            StartingY = Position.y - Mid_Width;
            EndingY = Position.y + Mid_Width;

            for (float IteratorY = StartingY; IteratorY < (EndingY + Increment); IteratorY += Increment)
            {
                for (float IteratorX = StartingX; IteratorX < (EndingX + Increment); IteratorX += Increment)
                {

                }
            }
        }

        public void Draw_Box()
        {

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
