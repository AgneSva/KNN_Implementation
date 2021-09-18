using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace KNNDT
{
    public partial class Form1 : Form
    {
        string conString = @"Data Source=LAPTOP-2DBCAKJA\SQLEXPRESS;Initial Catalog=knn;Integrated Security=True";
        //  List<double> x = new List<double>();
        int k;
        int lastNumber;
        string result;
        string result2;

        public Form1()
        {
            InitializeComponent();
        }

        public void Form1_Load(object sender, EventArgs e)
        {

            SqlConnection con = new SqlConnection(conString);
            con.Open();
            //  using (SqlConnection sqlCon = new SqlConnection(conString))
            //{
            //filling tables
            dgv1.ReadOnly = true;
            // dgv2.ReadOnly = true;

            SqlDataAdapter adapter = new SqlDataAdapter("Select * from KLASIFIKUOTI", conString);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dgv1.DataSource = dt;

            SqlDataAdapter adapter2 = new SqlDataAdapter("Select * from NEKLASIFIKUOTI", conString);
            DataTable dt2 = new DataTable();
            adapter2.Fill(dt2);
            dgv2.DataSource = dt2;
            dgv3.DataSource = dt2;

        }
        //funkcija skaiciuoti distan.
        private double distanceToCenterFunction(double x1Test, double x2Test, double x1Data, double x2Data)
        {
            var result = Math.Sqrt(Math.Pow(x1Test - x1Data, 2) + Math.Pow(x2Test - x2Data, 2));
            return result;
        }


        //issaiskinti kuriu daugiau=pliusu ar minusu=rezultatas graz
        public string countingResult(int p, int m)
        {
            string r;
            if (p > m)
            {
                r = "PLUS";

            }
            else if (m > p)
            {
                r = "MINUS";

            }
            else
            {
                r = "NEAISKU";
            }
            return r;
        }
        public void start_Click(object sender, EventArgs e)
        {

            double[] x1Data = new double[dgv1.RowCount - 1];
            double[] x2Data = new double[dgv1.RowCount - 1];
            string[] classData = new string[dgv1.RowCount - 1];
            //distance for first point:
            double[] distance = new double[dgv1.RowCount - 1];
            //Distance for second point:
            double[] distance2 = new double[dgv1.RowCount - 1];
            //xtest1 X
            //xtest2 Y
            double[] xTest1 = new double[dgv2.RowCount - 1];
            double[] xTest2 = new double[dgv2.RowCount - 1];

            for (int i = 0; i < dgv2.RowCount - 1; i++)
            {
                xTest1[i] = Convert.ToDouble(dgv2.Rows[i].Cells[0].Value);
                xTest2[i] = Convert.ToDouble(dgv2.Rows[i].Cells[1].Value);

            }
           
            for (int i = 0; i < dgv1.RowCount - 1; i++)
            {
                x1Data[i] = Convert.ToDouble(dgv1.Rows[i].Cells[0].Value);
                x2Data[i] = Convert.ToDouble(dgv1.Rows[i].Cells[1].Value);

                classData[i] = Convert.ToString(dgv1.Rows[i].Cells[2].Value);


                distance[i] = distanceToCenterFunction(xTest1[0], xTest2[0], x1Data[i], x2Data[i]);
                distance2[i] = distanceToCenterFunction(xTest1[1], xTest2[1], x1Data[i], x2Data[i]);

            }
           

            //find k
            k = Convert.ToInt32(comboBox1.SelectedItem);

            double[] distanceCopy = new double[dgv1.RowCount - 1];
            double[] distanceAscending = new double[dgv1.RowCount - 1];
            double[] distanceAscendingIndexNumber = new double[dgv1.RowCount - 1];
            //for second point
            double[] distance2Copy = new double[dgv1.RowCount - 1];
            double[] distance2Ascending = new double[dgv1.RowCount - 1];
            double[] distance2AscendingIndexNumber = new double[dgv1.RowCount - 1];
            double smallest = 9999;

            foreach (var item in distance)
            {
                distanceCopy = distance;
            }

            foreach (var item in distance2)
            {
                distance2Copy = distance2;
            }

            for (int j = 0; j < dgv1.RowCount - 1; j++)
            {
                for (int i = 0; i < dgv1.RowCount - 1; i++)
                {
                    if (smallest > distanceCopy[i])
                    {
                        smallest = distanceCopy[i];
                        distanceAscending[j] = smallest;
                        distanceAscendingIndexNumber[j] = i;
                        lastNumber = i;
                    }
                }
                distanceCopy[lastNumber] = 9999;
                smallest = 9999;
            }


            for (int j = 0; j < dgv1.RowCount - 1; j++)
            {
                for (int i = 0; i < dgv1.RowCount - 1; i++)
                {
                    if (smallest > distance2Copy[i])
                    {
                        smallest = distance2Copy[i];
                        distance2Ascending[j] = smallest;
                        distance2AscendingIndexNumber[j] = i;
                        lastNumber = i;
                    }
                }
                distance2Copy[lastNumber] = 9999;
                smallest = 9999;
            }

            rtbDistance.Text = null;
            rbtIndex.Text = null;
            rbtClass.Text = null;
            List<string> classList = new List<string>();
            List<string> class2List = new List<string>();

            for (int i = 0; i < k; i++)
            {
          //      rtbDistance.AppendText(distance2Ascending[i].ToString() + Environment.NewLine);
          //      rbtIndex.AppendText(distance2AscendingIndexNumber[i].ToString() + Environment.NewLine);
           //     rbtClass.AppendText(classData[Convert.ToInt32(distance2AscendingIndexNumber[i])] + Environment.NewLine);
                //artimiausiu k klase- konkuruoj.
                classList.Add(classData[Convert.ToInt32(distanceAscendingIndexNumber[i])]);
                class2List.Add(classData[Convert.ToInt32(distance2AscendingIndexNumber[i])]);
            }
            //classdata- egzistuojancios klases pirmoj lentelej
      
            var plusc = classList.Where(s => s.Contains("PLUS")).Count();
            var minusc = classList.Where(s => s.Contains("MINUS")).Count();

            var plusc2 = class2List.Where(s => s.Contains("PLUS")).Count();
            var minusc2 = class2List.Where(s => s.Contains("MINUS")).Count();

            result = countingResult(plusc, minusc);
            result2 = countingResult(plusc2, minusc2);

            //filling table with results
            dgv3.Rows[0].Cells[2].Value = result;
            dgv3.Rows[1].Cells[2].Value = result2;

        }
    }
}
