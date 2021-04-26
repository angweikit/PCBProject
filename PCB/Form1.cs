using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCB
{
    public partial class Form1 : Form
    {
        PCBPointCloud pcbPointCloud = new PCBPointCloud();
        public Form1()
        {
            InitializeComponent();
        }

        private float[] _vertices;
        int VertexAttrib_StrideSize = 11;
        int PointCloudSize = new int();
        private void btnLoad_Click(object sender, EventArgs e)
        {
            pcbPointCloud.Load_Model(ref _vertices, VertexAttrib_StrideSize, out PointCloudSize);
        }
    }
}
