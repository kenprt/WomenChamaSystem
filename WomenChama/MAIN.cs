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
using MaterialSkin.Controls;
using MaterialSkin;

namespace WomenChama
{
    public partial class MAIN : MaterialForm
    {
        public MAIN()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Green800, Primary.Green900, Primary.Green500, Accent.Green200, TextShade.WHITE);

        }
        SqlConnection con = new SqlConnection("Data Source=DESKTOP-KL8RVKT;Initial Catalog=womenGroup;Integrated Security=True");
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel6_Paint(object sender, PaintEventArgs e)
        {

        }
        public void MonthlySavings()
        {
            
        }
        public void loadSavingChart()
        {
            SqlCommand cmd = new SqlCommand("Select Type, sum(amount) as Total from savings group by Type",con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable table = new DataTable();
            sda.Fill(table);
            chart1.DataSource = table;
            chart1.Series["Series1"].XValueMember = "Type";
            chart1.Series["Series1"].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
            chart1.Series["Series1"].YValueMembers = "Total";
            chart1.Series["Series1"].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
        }
        public void loadIndividualSavingsChart()
        {
            SqlCommand cmd = new SqlCommand("Select date, sum(amount) as Total from savings where type ='"+bunifuLabel6.Text+"'and id = '"+txtfinduser.Text+"' group by date", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable table = new DataTable();
            sda.Fill(table);
            chart2.DataSource = table;
            chart2.Series["Series1"].XValueMember = "date";
            chart2.Series["Series1"].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
            chart2.Series["Series1"].YValueMembers = "Total";
            chart2.Series["Series1"].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
        }
        

        private void materialButton1_Click(object sender, EventArgs e)
        {
            bunifuPages1.SetPage("Dashboard");
        }

        private void materialButton3_Click(object sender, EventArgs e)
        {
            bunifuPages1.SetPage("Members");
        }
        public void liveMemberSearch()
        {
            SqlDataAdapter sda = new SqlDataAdapter("Select * from profile where name like '" + this.txtsearch.Text + "%'or phone LIKe '"+this.txtsearch.Text+"%' or address LIKE '"+this.txtsearch.Text+"%'or date like '"+this.txtsearch.Text+"%' ", con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            bunifuDataGridView1.DataSource = dt;
        }

        private void txtsearch_Click(object sender, EventArgs e)
        {

        }
        public void addToSavings()
        {
            SqlCommand cmd = new SqlCommand("Select * from profile where phone = '"+txtphone.Text+"'",con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                label7.Text = dt.Rows[0][0].ToString();
                SqlCommand addSavings = new SqlCommand("Insert into savings (id, amount,date, type)Values('" + label7.Text + "','" + label6.Text + "','" + currentdate.Text + "','" + bunifuLabel6.Text + "')", con);
                addSavings.ExecuteNonQuery();
                bindSavings();

            }
           
        }
        public void bindMembers()
        {
            SqlCommand cmd = new SqlCommand("Select * from profile", con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            bunifuDataGridView1.DataSource = dt;
        }
        public void clearMembers()
        {
            txtaddress.Text = "";
            txtphone.Text = "";
            txtfullname.Text = "";
            label1.Text = "";
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            if(txtfullname.Text == "" || txtphone.Text == "" || txtaddress.Text == "")
            {
                MessageBox.Show("All fields are required", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if(txtphone.TextLength != 10)
            {
                MessageBox.Show("This is an invalid phone number", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                try
                {
                    con.Open();
                    if (con.State == System.Data.ConnectionState.Open) 
                    {
                        SqlCommand cmd = new SqlCommand("Select * from profile where phone = '" + txtphone.Text + "'", con);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        if(dt.Rows.Count > 0)
                        {
                            MessageBox.Show("The phone number entered already exists in the system", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            SqlCommand checkupdate = new SqlCommand("Select * from profile where id = '"+label1.Text+"'", con);
                            SqlDataAdapter ada = new SqlDataAdapter(checkupdate);
                            DataTable tbl = new DataTable();
                            ada.Fill(tbl);
                            if (tbl.Rows.Count > 0)
                            {
                                SqlCommand update = new SqlCommand("Update profile set name= '" + txtfullname.Text + "', phone='" + txtphone.Text + "', address = '" + txtaddress.Text + "'", con);
                                update.ExecuteNonQuery();
                                MessageBox.Show("Record updated successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                bindMembers();
                                countMembers();
                                clearMembers();
                               
                            }
                            else
                            {
                                SqlCommand command = new SqlCommand("Insert into profile  (name, phone,address, date)Values('" + txtfullname.Text + "', '" + txtphone.Text + "','" + txtaddress.Text + "','" + currentdate.Text + "')", con);
                       
                                command.ExecuteNonQuery();
                                addToSavings();
                                MessageBox.Show("Member added succesfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                bindMembers();
                                countMembers();
                                clearMembers();
                            }
                        

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
            }        }
        public void countMembers()
        {
            SqlCommand cmd = new SqlCommand("Select count(id) as total from profile", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                lblmembers.Text = dt.Rows[0]["Total"].ToString();
            }
        }

        private void txtphone_Click(object sender, EventArgs e)
        {

        }

        private void MAIN_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'womenGroupDataSet1.profile' table. You can move, or remove it, as needed.
            this.profileTableAdapter1.Fill(this.womenGroupDataSet1.profile);
            // TODO: This line of code loads data into the 'womenGroupDataSet.profile' table. You can move, or remove it, as needed.
            this.profileTableAdapter.Fill(this.womenGroupDataSet.profile);
            bindMembers();
            liveMemberSearch();
            countMembers();
            bindSavings();
            countSavings();
            loadSavingChart();
            bindLoans();
            countLoans();
            MonthlySavings();
        }

        private void bunifuDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(bunifuDataGridView1.Columns[e.ColumnIndex].Name == "view")
            {
                DataGridViewRow dr = new DataGridViewRow();
                dr = bunifuDataGridView1.SelectedRows[0];
                label1.Text = dr.Cells[0].Value.ToString();
                txtfullname.Text = dr.Cells[1].Value.ToString();
                txtphone.Text = dr.Cells[2].Value.ToString();
                txtaddress.Text = dr.Cells[3].Value.ToString();
            }else if(bunifuDataGridView1.Columns[e.ColumnIndex].Name == "delete")
            {
                try
                {
                    con.Open();
                    if(con.State == System.Data.ConnectionState.Open)
                    {
                        DataGridViewRow dr = new DataGridViewRow();
                        dr = bunifuDataGridView1.SelectedRows[0];
                        label1.Text = dr.Cells[0].Value.ToString();
                        SqlCommand cmd = new SqlCommand("Delete from profile where id = '" + label1.Text + "'", con);
                        if(MessageBox.Show("Are you sure you want to permently delete this data","Confirm with Yes",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            cmd.ExecuteNonQuery();
                            bindMembers();
                            countMembers();
                            clearMembers();
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
        public void countSavings()
        {
            SqlCommand cmd = new SqlCommand("Select sum(amount) as Total from savings where type ='"+bunifuLabel6.Text+"'",con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable table = new DataTable();
            adapter.Fill(table);
            if (table.Rows.Count > 0)
            {
                lblsavings.Text = "KES" + " " + table.Rows[0]["Total"].ToString();
            }

        }
        public void countLoans()
        {
            SqlCommand cmd = new SqlCommand("Select sum(amount) as Total from savings where type='"+label4.Text+"'", con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable table = new DataTable();
            adapter.Fill(table);
            if (table.Rows.Count > 0)
            {
                lblloans.Text = "KES" + " " + table.Rows[0]["Total"].ToString();
            }

        }
        private void txtsearch_TextChanged(object sender, EventArgs e)
        {
            liveMemberSearch();
        }

        private void materialButton4_Click(object sender, EventArgs e)
        {
            bunifuPages1.SetPage("Savings");
        }
        public void bindSavings()
        {
            SqlCommand cmd = new SqlCommand("Select * from savings where Type = '"+ bunifuLabel6.Text + "'",con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            bunifuDataGridView2.DataSource = dt;
        }
        public void clearSavings()
        {
            txtregnumber.Text = "";
            txtamount.Text = "";
        }

        private void materialButton8_Click(object sender, EventArgs e)
        {
            if (txtregnumber.Text == "" || txtamount.Text == "")
            {
                MessageBox.Show("All fields are required", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                try
                {
                    con.Open();
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        SqlCommand cmd = new SqlCommand("Select * from profile where id = '" + txtregnumber.Text + "'", con);
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        DataTable table = new DataTable();
                        sda.Fill(table);
                        if (table.Rows.Count>0)
                        {
                            SqlCommand command = new SqlCommand("Insert into savings(id, amount,date,Type)Values('"+txtregnumber.Text+"','"+txtamount.Text+"', '"+currentdate.Text+"','"+ bunifuLabel6 .Text+ "')", con);
                            command.ExecuteNonQuery();
                            bindSavings();
                            countSavings();
                            loadSavingChart();
                            clearSavings();
                        }
                        else
                        {
                            MessageBox.Show("The Reg Number is not in the list","",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        }
                    }
                    {

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

        private void materialButton7_Click(object sender, EventArgs e)
        {
            bunifuPages1.SetPage("individual");
        }

        private void materialButton5_Click(object sender, EventArgs e)
        {
            bunifuPages1.SetPage("Loans");
        }

        private void materialButton9_Click(object sender, EventArgs e)
        {
            if (txtcheck.Text == "")
            {
                MessageBox.Show("The loan limit field cannot be empty", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                try
                {
                    con.Open();
                    if(con.State == System.Data.ConnectionState.Open)
                    {
                        SqlCommand cmd = new SqlCommand("Select * from profile where id = '"+txtcheck.Text+"'",con);
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            SqlCommand command = new SqlCommand("Select sum(amount)as total from savings where type = '" + bunifuLabel6.Text + "' and id = '"+txtcheck.Text+"'", con);
                            SqlDataAdapter adapter = new SqlDataAdapter(command);
                            DataTable table = new DataTable();
                            adapter.Fill(table);
                            if (table.Rows.Count>0)
                            {
                                label2.Text = table.Rows[0]["total"].ToString();
                                lbllimit.Text = "KES" +" "+ (float.Parse(label2.Text) *float.Parse(label3.Text) + (float.Parse(label2.Text))).ToString();
                            }
                            else
                            {
                                lbllimit.Text = "KES" + " " + "0.00";
                            }
                        }
                        else
                        {
                            MessageBox.Show("This is not a registered Number","",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
        }
        public void bindLoans()
        {
            SqlCommand cmd = new SqlCommand("Select * from savings where Type = '"+label4.Text+"'",con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            bunifuDataGridView3.DataSource = dt;
        }
        public void clearLoan()
        {
            regno.Text = "";
            loan.Text = "";
        }

        private void materialButton10_Click(object sender, EventArgs e)
        {
            if (regno.Text=="" || loan.Text == "")
            {
                MessageBox.Show("All fields are required", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                try
                {
                    con.Open();
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        SqlCommand cmd = new SqlCommand("Select * from profile where id = '" + regno.Text + "'", con);
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            SqlCommand checkifPresent = new SqlCommand("Select * from savings where Type= '" + label4.Text + "' and id='" + regno.Text + "'", con);
                            SqlDataAdapter ada = new SqlDataAdapter(checkifPresent);
                            DataTable tbl = new DataTable();
                            ada.Fill(tbl);
                            if (tbl.Rows.Count > 0)
                            {
                                MessageBox.Show("The Reg Number has an existing loan already", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            { 
                                SqlCommand command = new SqlCommand("Select sum(amount)as total from savings where type = '" + bunifuLabel6.Text + "' and id = '" + regno.Text + "'", con);
                                SqlDataAdapter adapter = new SqlDataAdapter(command);
                                DataTable table = new DataTable();
                                adapter.Fill(table);
                                if (table.Rows.Count > 0)
                                {
                                    label2.Text = table.Rows[0]["total"].ToString();
                                    label5.Text = (float.Parse(label2.Text) * float.Parse(label3.Text) + (float.Parse(label2.Text))).ToString();
                                    if (float.Parse(loan.Text) > float.Parse(label5.Text))
                                    {
                                        MessageBox.Show("Failed! The amount entered exceds the loan limit", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    else
                                    {
                                        SqlCommand loans = new SqlCommand("Insert into savings (id, amount, date, Type)Values('" + regno.Text + "', '" + loan.Text + "','" + currentdate.Text + "','" + label4.Text + "')", con);
                                        loans.ExecuteNonQuery();
                                        MessageBox.Show("Loan granted succesfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        bindLoans();
                                        loadSavingChart();
                                        countLoans();
                                        clearLoan();
                                    }
                                }
                            
                             
                            } 
                        }
                        else
                        {
                            MessageBox.Show("This is not a registered Number", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
        }
        public void calculataIntrest()
        {
            SqlCommand cmd = new SqlCommand("Select * from savings where id = '" + txtfinduser.Text + "' and Type ='" + label4.Text + "'", con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                bunifuDatePicker2.Text = dt.Rows[0][2].ToString();
                DateTime sdt = bunifuDatePicker2.Value.Date;
                DateTime edt = currentdate.Value.Date;
                TimeSpan ts = edt - sdt;
                label10.Text= ts.Days.ToString();
                label11.Text = ((float.Parse(label10.Text) * float.Parse(label9.Text))).ToString();
                pay.Text ="KES " + (float.Parse(label8.Text) + float.Parse(label11.Text)).ToString();
            }
        }

        private void materialButton11_Click(object sender, EventArgs e)
        {
            if(txtfinduser.Text == "")
            {
                MessageBox.Show("Enter the user Registration Number", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                try
                {
                    con.Open();
                    if(con.State == System.Data.ConnectionState.Open)
                    {
                        SqlCommand cmd = new SqlCommand("Select * from profile where id='" + txtfinduser.Text + "'", con);
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        if(dt.Rows.Count > 0)
                        {
                            lblusername.Text = dt.Rows[0][1].ToString();
                            SqlCommand command = new SqlCommand("Select sum(amount) as total from Savings where Type = '" + bunifuLabel6.Text + "'and id = '" + txtfinduser.Text + "'", con);
                            SqlDataAdapter adapter = new SqlDataAdapter(command);
                            DataTable table = new DataTable();
                            adapter.Fill(table);
                            if (table.Rows.Count > 0)
                            {
                                ttsavings.Text = "KES"+ " "+ table.Rows[0]["total"].ToString();
                                loadIndividualSavingsChart();
                                SqlCommand loancheck = new SqlCommand("Select sum(amount) as total from Savings where Type = '" + label4.Text + "'and id = '" + txtfinduser.Text + "'", con);
                                SqlDataAdapter ada = new SqlDataAdapter(loancheck);
                                DataTable tbl = new DataTable();
                                ada.Fill(tbl);
                                if (tbl.Rows.Count > 0)
                                {
                                    ttloans.Text = "KES" + " " + tbl.Rows[0]["total"].ToString();
                                    label8.Text = tbl.Rows[0]["total"].ToString();
                                    calculataIntrest();
                                    bindLoans();
                                    countLoans();
                                }

                            }
                         
                        }

                    }
                    else
                    {
                        MessageBox.Show("The Number Entered Does Not Exists","",MessageBoxButtons.OK,MessageBoxIcon.Error);
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

        private void materialButton13_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("Select date, sum(amount) as Total from savings where type ='" + bunifuLabel6.Text + "'and id = '" + txtfinduser.Text + "' and date between '"+bg.Text+"'and'"+end.Text+"'group by date", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable table = new DataTable();
            sda.Fill(table);
            chart2.DataSource = table;
            chart2.Series["Series1"].XValueMember = "date";
            chart2.Series["Series1"].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
            chart2.Series["Series1"].YValueMembers = "Total";
            chart2.Series["Series1"].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
        }

        private void materialButton12_Click(object sender, EventArgs e)
        {
            Form1 frm = new Form1();
            frm.Show();
        }

        private void materialButton6_Click(object sender, EventArgs e)
        {
            bunifuPages1.SetPage("reports");
        }

        private void materialButton20_Click(object sender, EventArgs e)
        {
            bunifuPages1.SetPage("Settings");
        }
    }
}
