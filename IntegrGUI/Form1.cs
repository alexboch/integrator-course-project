using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Expressions;

namespace IntegrGUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            
            InitializeComponent();
            double x = "x+2*x+22".Eval(0.5);
        }

        RPNExpression rpn_ex;
        double a=0, b=0;//пределы интегрирования
        double dx = 0;
        int n,index=0;

        
       
       
       
        private void button1_Click(object sender, EventArgs e)
        {
            statusLabel.Text = "Выполняется вычисление"; 
             
               try{
                    a = Convert.ToDouble(textBox3.Text);
                    b = Convert.ToDouble(textBox4.Text);
              
                    n = Convert.ToInt32(textBox1.Text);
                    dx = (b - a) / n;
                
                rpn_ex = function.Text.ToRPN();
               
                index = listBox1.SelectedIndex;
                backgroundWorker1.RunWorkerAsync();// запуск вычисления во втором потоке
           
           }
            catch (Exception exc)
            {
                MessageBox.Show("Ошибка:" + exc.Message);
            }
        }

        

        private void Form1_Load(object sender, EventArgs e)
        {
            listBox1.SelectedIndex = 0;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                BackgroundWorker bw = sender as BackgroundWorker;
                double percent = (b - a) / 100;
                double Sum = 0;
                double p = 0;
                int progress = 0, prevprogress = 0; ;
                switch (index)
                {
                    case 0://Метод прямоугольников
                        for (double x = a; x < b; x += dx,p+=dx)
                        {
                            Sum += rpn_ex.Evaluate(x - dx / 2) * dx;
                            prevprogress = progress;
                            progress = Convert.ToInt32(p / percent);
                            if (progress <= 100 && progress > prevprogress)
                            bw.ReportProgress(progress);
                        }
                        e.Result = Sum.ToString();
                        
                        break;
                    case 1: //Метод трапеций

                        for (double x = a; x + dx <= b; x += dx,p+=dx)
                        {
                            double x2 = x + dx;
                            Sum += (rpn_ex.Evaluate(x) + rpn_ex.Evaluate(x2)) / 2.0 * dx;
                            prevprogress = progress;
                            progress = Convert.ToInt32(p / percent);
                            if (progress <= 100 && progress > prevprogress)
                            bw.ReportProgress(progress);
                        }
                        e.Result = Sum.ToString();
                        break;
                    default: e.Result = "";
                        break;
                    case 2://Метод Симпсона
                        double SumOdd = 0, SumEven = 0, X= a;
                        for (int i = 0; X <= b; X += dx, p+=dx,i++)
                        {
                            if (i % 2 == 0)
                                SumEven += rpn_ex.Evaluate(X);
                            else
                                SumOdd += rpn_ex.Evaluate(X);
                            prevprogress = progress;
                            progress = Convert.ToInt32(p / percent);
                            if (progress <=100&&progress> prevprogress)
                            bw.ReportProgress(progress);
                        }
                        double x_1 = rpn_ex.Evaluate(a);
                        double x_2 = rpn_ex.Evaluate(b);
                        double Result = dx / 3 * (x_1 + SumEven * 2 + SumOdd * 4 + x_2);
                        e.Result = Result.ToString();
                        break;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            label3.Text = e.Result.ToString(); ;
                statusLabel.Text = "Вычисление завершено";
               
            }

        private void Cancel_Btn_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();//прервать вычисление
       
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

       
       
    }
}
