using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WomenChama
{
    public partial class Form1 : Form
    {
        SqlConnection con = new SqlConnection("Data Source=DESKTOP-KL8RVKT;Initial Catalog=womenGroup;Integrated Security=True");
        public Form1()
        {
            InitializeComponent();
        }
        public void calculataIntrest()
        {
            SqlCommand cmd = new SqlCommand("Select * from savings where id = '" + txtregno.Text + "' and Type ='" + label2.Text + "'", con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                bunifuDatePicker1.Text = dt.Rows[0][2].ToString();
                DateTime sdt = bunifuDatePicker1.Value.Date;
                DateTime edt = currentdate.Value.Date;
                TimeSpan ts = edt - sdt;
                label3.Text = ts.Days.ToString();
                label4.Text = ((float.Parse(label3.Text) * float.Parse(label5.Text))).ToString();
                pay.Text = (float.Parse(label6.Text) + float.Parse(label4.Text)).ToString();
                label7.Text = (float.Parse(pay.Text) - float.Parse(txtamount.Text)).ToString();
               
                if(float.Parse(txtamount.Text)> float.Parse(pay.Text))
                {
                    MessageBox.Show("The Payment Exceeds the loan to be payed", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (float.Parse(label7.Text) == 0)
                {
                    SqlCommand command = new SqlCommand("Delete from savings where id = '" + txtregno.Text + "' and Type = '" + label2.Text + "'", con);
                    command.ExecuteNonQuery();
                    SqlCommand savePay = new SqlCommand("Insert into payment(id, amount, intrest, date)Values('" + txtregno.Text + "','" + txtamount.Text + "','" + label4.Text + "','" + currentdate.Text + "')", con);
                    savePay.ExecuteNonQuery();

                }
                else
                {
                    SqlCommand updateLoan = new SqlCommand("Update savings set amount= '" + label7.Text + "' where id = '" + txtregno.Text + "'and Type = '" + label2.Text + "'", con);
                    updateLoan.ExecuteNonQuery();
                    SqlCommand savePay = new SqlCommand("Insert into payment(id, amount, intrest, date)Values('" + txtregno.Text + "','" + txtamount.Text + "','" + label4.Text + "','" + currentdate.Text + "')", con);
                    savePay.ExecuteNonQuery();
                }

            }
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            if(txtamount.Text == "" || txtregno.Text == "")
            {
                MessageBox.Show("All fields are required", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                try
                {
                    con.Open();
                    if(con.State == System.Data.ConnectionState.Open)
                    {
                        SqlCommand cmd = new SqlCommand("Select * from profile where id = '"+txtregno.Text+"'", con);
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            SqlCommand command = new SqlCommand("Select * from savings where id ='"+txtregno.Text+"' and Type = '"+label2.Text+"'",con);
                            SqlDataAdapter adapter = new SqlDataAdapter(command);
                            DataTable table = new DataTable();
                            adapter.Fill(table);
                            if (table.Rows.Count > 0)
                            {
                                label6.Text = table.Rows[0][1].ToString();
                                calculataIntrest();
                               
                                MAIN frm = new MAIN();
                                frm.bindLoans();
                                txtamount.Text = "";
                                txtregno.Text = "";
                       

                            }

                        }
                        else
                        {
                            MessageBox.Show("The Registration Number does not exist", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    
                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    con.Close();
                }

            }
        }
    }
}
