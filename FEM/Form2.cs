using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FEM
{
    public partial class Form2 : Form
    {
        public double[,] m = new double[51,52];

        public Form2(double [,] a) 
        {
            InitializeComponent();
            a = m;
            var rowCount = a.GetLength(0);
            var rowLength = a.GetLength(1);

            for (int rowIndex = 0; rowIndex < rowCount; ++rowIndex)
            {
                var row = new DataGridViewRow();
                dataGridView1.Columns.Add(" ", " ");
                for (int columnIndex = 0; columnIndex < rowLength; ++columnIndex)
                {
                    row.Cells.Add(new DataGridViewTextBoxCell()
                    {
                        Value = a[rowIndex, columnIndex]
                    });
                }

                dataGridView1.Rows.Add(row);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
