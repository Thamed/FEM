using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEM
{
    
    class CalcData
    {
        static int nh;
        public double dR;
        public double dtau;
        public double ntime;
        public double x;
        public double alfa;
        public double Rp;
        public double tptau;
        public double dT;
        public double[] crood = new double[nh];
        public double[] temp = new double[nh];
        public double[] temptau = new double[2];
        public double dtmax;
        public double tau;
        public double[,] globalH = new double[nh,nh];
        public double[] globalP = new double[nh];
        public double[,] matrix = new double[nh,nh + 1];

        CalcData()
        {
            double nh = Form1.nh;
        }

        public static double[,] clearglobalh(double[,] globalH)
        {

            for (int i = 0; i < nh; i++)
            {
                for (int j = 0; j < nh; j++)
                    globalH[i,j] = 0;
            }

            return globalH;
        }

        public static double[] clearglobalp(double[] globalP)
        {

            for (int i = 0; i < nh; i++)
                globalP[i] = 0;

            return globalP;
        }

        public static Element buildlocals(Element[] e, CalcData cd, Data d, int ip, int ie, double[] W, double[] N1, double[] N2)
        {

            if (ip != 1)
            {
                e[ie].localH[0,0] += d.K * cd.Rp * W[ip] / cd.dR + d.c * d.ro * cd.dR * cd.Rp * W[ip] * N1[ip] * N1[ip] / cd.dtau;
                e[ie].localH[0,1] += (-1) * d.K * cd.Rp * W[ip] / cd.dR + d.c * d.ro * cd.dR * cd.Rp * W[ip] * N1[ip] * N2[ip] / cd.dtau;
                e[ie].localH[1,0] = e[ie].localH[0,1];
                e[ie].localH[1,1] += d.K * cd.Rp * W[ip] / cd.dR + d.c * d.ro * cd.dR * cd.Rp * W[ip] * N2[ip] * N2[ip] / cd.dtau;

                e[ie].localP[0] += d.c * d.ro * cd.dR * cd.tptau * cd.Rp * W[ip] * N1[ip] / cd.dtau;
                e[ie].localP[1] += d.c * d.ro * cd.dR * cd.tptau * cd.Rp * W[ip] * N2[ip] / cd.dtau;
            }
            else {
                e[ie].localH[0,0] += d.K * cd.Rp * W[ip] / cd.dR + d.c * d.ro * cd.dR * cd.Rp * W[ip] * N1[ip] * N1[ip] / cd.dtau;
                e[ie].localH[0,1] += (-1) * d.K * cd.Rp * W[ip] / cd.dR + d.c * d.ro * cd.dR * cd.Rp * W[ip] * N1[ip] * N2[ip] / cd.dtau;
                e[ie].localH[1,0] = e[ie].localH[0,1];
                e[ie].localH[1,1] += d.K * cd.Rp * W[ip] / cd.dR + d.c * d.ro * cd.dR * cd.Rp * W[ip] * N2[ip] * N2[ip] / cd.dtau + 2 * cd.alfa * d.rmax;

                e[ie].localP[0] += d.c * d.ro * cd.dR * cd.tptau * cd.Rp * W[ip] * N1[ip] / cd.dtau;
                e[ie].localP[1] += d.c * d.ro * cd.dR * cd.tptau * cd.Rp * W[ip] * N2[ip] / cd.dtau + 2 * cd.alfa * d.rmax * d.tempair;
            }

            return e[ie];
        }

        public static double[,] buildglobalh(int ie, Element[] el, CalcData cd)
        {

            cd.globalH[ie,ie] += +el[ie].localH[0,0];
            cd.globalH[ie,ie + 1] += el[ie].localH[0,1];
            cd.globalH[ie + 1,ie] += el[ie].localH[1,0];
            cd.globalH[ie + 1,ie + 1] += el[ie].localH[1,1];


            return cd.globalH;
        }


        public static double[] buildglobalp(int ie, Element[] el, CalcData cd)
        {

            cd.globalP[ie] += el[ie].localP[0];
            cd.globalP[ie + 1] += el[ie].localP[1];


            return cd.globalP;
        }


        public static double[,] mergematrix(CalcData cd)
        {

            for (int i = 0; i < nh; i++)
            {
                for (int j = 0; j < nh; j++)
                    cd.matrix[i,j] = cd.globalH[i,j];
            }

            for (int i = 0; i < nh; i++)
                cd.matrix[i,nh] = cd.globalP[i];


            return cd.matrix;
        }
    }
}
