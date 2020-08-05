using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LibraryManager
{
    public partial class adminmembermanagement : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            GridView1.DataBind();
        }

        // Go button
        protected void LinkButton4_Click(object sender, EventArgs e)
        {
            getMemberByID();
        }
        // Active button
        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            updateMemberStatusByID("Active");
        }
        // pending button
        protected void LinkButton2_Click(object sender, EventArgs e)
        {
            updateMemberStatusByID("Pending");
        }
        // deactive button
        protected void LinkButton3_Click(object sender, EventArgs e)
        {
            updateMemberStatusByID("Suspended");
        }
        // delete button
        protected void Button2_Click(object sender, EventArgs e)
        {
            deleteMemberByID();
        }

        // user defined function
        bool checkIfMemberExists()
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                SqlCommand sqlCmd = new SqlCommand("SELECT * FROM member_master_tbl WHERE member_id='" + TextBox1.Text.Trim() + "';", connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                if (dataTable.Rows.Count >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Error!!')", true);
                return false;
            }
        }

        void deleteMemberByID()
        {
            if (checkIfMemberExists())
            {
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }

                    SqlCommand sqlCmd = new SqlCommand("DELETE FROM member_master_tbl WHERE member_id='" + TextBox1.Text.Trim() + "'", connection);

                    sqlCmd.ExecuteNonQuery();
                    connection.Close();
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.success('Member Deleted Successfully')", true);
                    clearForm();
                    GridView1.DataBind();
                }
                catch (Exception ex)
                {
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Error!!')", true);
                }
            }
            else
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Invalid Member ID')", true);
            }
        }

        void getMemberByID()
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                SqlCommand sqlCmd = new SqlCommand("SELECT * FROM member_master_tbl WHERE member_id='" + TextBox1.Text.Trim() + "'", connection);
                SqlDataReader dataReader = sqlCmd.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        TextBox2.Text = dataReader.GetValue(0).ToString();
                        TextBox7.Text = dataReader.GetValue(10).ToString();
                        TextBox8.Text = dataReader.GetValue(1).ToString();
                        TextBox3.Text = dataReader.GetValue(2).ToString();
                        TextBox4.Text = dataReader.GetValue(3).ToString();
                        TextBox9.Text = dataReader.GetValue(4).ToString();
                        TextBox10.Text = dataReader.GetValue(5).ToString();
                        TextBox11.Text = dataReader.GetValue(6).ToString();
                        TextBox6.Text = dataReader.GetValue(7).ToString();
                    }
                }
                else
                {
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Invalid Member ID')", true);
                }

            }
            catch (Exception ex)
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Error!!')", true);
            }
        }

        void updateMemberStatusByID(string status)
        {
            if (checkIfMemberExists())
            {
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    SqlCommand sqlCmd = new SqlCommand("UPDATE member_master_tbl SET account_status='"
                        + status + "' WHERE member_id='" + TextBox1.Text.Trim() + "'", connection);
                    sqlCmd.ExecuteNonQuery();
                    connection.Close();
                    GridView1.DataBind();
                    TextBox7.Text = status;
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.success('Member Status Updated')", true);
                }
                catch (Exception ex)
                {
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Error!!')", true);
                }
            }
            else
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Invalid Member ID')", true);
            }

        }

        void clearForm()
        {
            TextBox1.Text = "";
            TextBox2.Text = "";
            TextBox7.Text = "";
            TextBox8.Text = "";
            TextBox3.Text = "";
            TextBox4.Text = "";
            TextBox9.Text = "";
            TextBox10.Text = "";
            TextBox11.Text = "";
            TextBox6.Text = "";
        }
    }
}