using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Threading;

namespace FEM
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Thread w1;
        Thread w2;

        public static String filename;
        public static ArrayList datalist = new ArrayList();
        public static int nh = 51;
        public static int ne = nh - 1;
        static double[,] matrix = new double[nh, nh + 1];
        public void initdata(Data d, Element[] el, Node[] n)
        {


            double r = 0;
            double deltar = (d.rmax - d.rmin)/ne;

            for (int i = 0; i < nh; i++)
            {
                double alfa = 0;
                if (i == ne) alfa = d.alfaair;

                n[i].temp = d.tempbegin;
                n[i].bcond = 2 * alfa * d.rmax * d.tempair;
                n[i].r = r;
                n[i].alfa = alfa;
                //textArea.append(n[i].bcond + "\n");
                r += deltar;
            }

            for (int i = 0; i < ne; i++)
            {
                el[i].node1 = n[i];
                el[i].node2 = n[i + 1];
                el[i].c = d.c;
                el[i].ro = d.ro;
                el[i].k = d.K;
                //textArea.append(el[i].c + "\n");
            }
        }

        public void initlocals(Element[] el, Node[] n, Data d)
        {
            int np = 2;
            double[] N1 = { 0.5 * (1 + 0.57735027), 0.5 * (1 - 0.57735027) };
            double[] N2 = { 0.5 * (1 - 0.57735027), 0.5 * (1 + 0.57735027) };

            for (int i = 0; i < ne; i++)
            {
                double deltar = el[i].node2.r - el[i].node1.r;
                double dr = d.rmax / ne;
                double dtau = (Math.Pow(dr, 2)) / (0.5 * d.a);
                double ntime = (d.taumax / dtau) + 1;
                dtau = d.taumax / ntime;
                //textArea.append(el[i].node2.r + "\n");

                el[i].localH[0,0] = 0;
                el[i].localH[0,1] = 0;
                el[i].localH[1,0] = 0;
                el[i].localH[1,1] = 0;

                el[i].localP[0] = 0;
                el[i].localP[1] = 0;

                for (int ip = 0; ip < np; ip++)
                {
                    double rp = N1[ip] * el[i].node1.r + N2[ip] * el[i].node2.r;
                    double q = el[i].ro * el[i].c * deltar / dtau;

                    el[i].localH[0,0] += el[i].k / deltar * rp + q * (N1[ip] * N1[ip]) * rp;
                    el[i].localH[0,1] += -el[i].k / deltar * rp + q * (N1[ip] * N2[ip]) * rp;
                    el[i].localH[1,0] = el[i].localH[0,1];
                    el[i].localH[1,1] += el[i].k / deltar * rp + q * (N2[ip] * N2[ip]) * rp;

                    el[i].localP[0] += -q * (N1[ip] * el[i].node1.temp + N2[ip] * el[i].node2.temp) * N1[ip] * rp;
                    el[i].localP[1] += -q * (N1[ip] * el[i].node1.temp + N2[ip] * el[i].node2.temp) * N2[ip] * rp;
                    //textArea.append(q +"");
                }
                el[i].localH[1,1] += 2 * el[i].node2.alfa * el[i].node2.r;
                el[i].localP[1] -= el[i].node2.bcond;

                //textArea.append(el[i].localH[0,0] + "\t");
                //textArea.append(el[i].localH[0,1] + "\n");
                //textArea.append(el[i].localH[1,0] + "\t");
                //textArea.append(el[i].localH[1,1] + "\n\n");

            }
        }

        public void initglobals(Element[] el, Node[] n, double[,] globalH, double[] globalP, Data d)
        {
            for (int i = 0; i < nh; i++)
            {
                for (int j = 0; j < nh; j++)
                {
                    globalH[i,j] = 0;
                }
            }

            for (int i = 0; i < nh; i++)
            {
                globalP[i] = 0;
            }


            for (int i = 0; i < ne; i++)
            {
                globalH[i,i] += el[i].localH[0,0];
                globalH[i,i + 1] += el[i].localH[0,1];
                globalH[i + 1,i] += el[i].localH[1,0];
                globalH[i + 1,i + 1] += el[i].localH[1,1];

            }
            for (int i = 0; i < ne; i++)
            {
                globalP[i] += el[i].localP[0];
                globalP[i + 1] += el[i].localP[1];
            }

            /*
            for (int i = 0; i < nh; i++) {
            for (int j = 0; j < nh; j++) {
                textArea.append(globalH[i,j]+"\t");   
            }
                    textArea.append("\n");
        }
            */
        }

        public void merge(double[,] matrix, double[,] globalH, double[] globalP)
        {
            for (int i = 0; i < nh; i++)
            {
                for (int j = 0; j < nh; j++)
                    matrix[i,j] = globalH[i,j];
            }

            for (int i = 0; i < nh; i++)
                matrix[i,nh] = globalP[i];
        }
        /*wykresy*/
        public void inputDataSet1(double d1, double d2)
        {
            chart1.Series["Na powierzchni wsadu"].Points.AddXY(d1, d2);


        }
        public void inputDataSet2(double d1, double d2)
        {
            chart1.Series["W osi wsadu"].Points.AddXY(d1, d2);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Data d = new Data();
            d.a = (Double.Parse(textBox7.Text)) / (Double.Parse(textBox5.Text)) / (Double.Parse(textBox6.Text));
            d.rmax = Double.Parse(textBox1.Text);
            d.taumax = Double.Parse(textBox8.Text);
            d.K = Double.Parse(textBox7.Text);
            d.tempbegin = Double.Parse(textBox3.Text);
            d.tempair = Double.Parse(textBox4.Text);
            d.alfaair = Double.Parse(textBox2.Text);
            d.c = Double.Parse(textBox5.Text);
            d.ro = Double.Parse(textBox6.Text);
            d.rmin = Double.Parse(textBox9.Text);
            Element[] el = new Element[ne];
            Node[] n = new Node[nh];
            for (int i = 0; i < ne; i++) el[i] = new Element();
            for (int i = 0; i < nh; i++) n[i] = new Node();
            double[,] globalH = new double[nh, nh];
            double[] globalP = new double[nh];
            double[] globalT = new double[nh];
            double[] globalPmin = new double[nh];
            
            double dr = d.rmax / ne;
            double dtau = (Math.Pow(dr, 2)) / (0.5 * d.a);
            double ntime = (d.taumax / dtau) + 1;
            dtau = d.taumax / ntime;


            initdata(d, el, n);

            for (double time = 0; time <= d.taumax; time += dtau)
            {
                initlocals(el, n, d);
                initglobals(el, n, globalH, globalP, d);

                for (int i = 0; i < nh; i++)
                {
                    globalPmin[i] = globalP[i] * (-1);
                }

                merge(matrix, globalH, globalPmin);
                Gauss.gauss(nh, matrix, globalT);

                for (int i = 0; i < nh; i++)
                    n[i].temp = globalT[i];
                richTextBox1.AppendText(time + "\t");
                richTextBox1.AppendText(globalT[0] + "\t");
                richTextBox1.AppendText(globalT[ne] + "\n");
                //textArea.append("\n");

                inputDataSet2(time, globalT[0]);
                inputDataSet1(time, globalT[ne]);

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;


        }

        private void button5_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            chart1.Series.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                    if (control is TextBox)
                        (control as TextBox).Clear();
                    else
                        func(control.Controls);
            };

            func(Controls);
            richTextBox1.Clear();
            richTextBox1.Focus();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            panel1.Visible = false;
            chart1.Series.Clear();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(matrix);
            form2.ShowDialog();
        }
    }
}
