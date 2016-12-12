using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Compile;

namespace ZeonCompiler
{

    public partial class Form1 : Form
    {
       

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Compiler comp = new Compiler();
            comp.otherMain(tbInput.Text);
            tbRice.Text = comp.getRice();
            tbError.Text = comp.getErrorLog();
        }
    }




    class Tokens
    {
        public int index { get; set; }
        public String valor { get; set; }
        public Tokens()
        {
            index = 0;
            valor = "";
        }
        public Tokens(int ind, String val)
        {
            index = ind;
            valor = val;
        }
    }
    class Variables
    {
        public string name { get; set; }
        public string type { get; set; }
        public string direction { get; set; }
        public bool isArray { get; set; }
        public bool initialized { get; set; }
        public string length { get; set; }
        public int size { get; set; }
        public Variables()
        {
            name = "noname";
            type = "null";
            isArray = false;
            initialized = false;
            direction = "-1";
            length = "-1";
            size = -1;
        }

    }
}
