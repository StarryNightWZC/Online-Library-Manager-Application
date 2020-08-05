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
    public partial class usersignup : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        // sign up button click event
        protected void Button1_Click(object sender, EventArgs e)
        {
            if (checkMemberExists())
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.warning('Member Already Exist with this Member ID, try other ID')", true);
            }
            else
            {
                signUpNewMember();
            }
        }

        // user defined method
        bool checkMemberExists()
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                SqlCommand sqlCmd = new SqlCommand("SELECT * from member_master_tbl where member_id='" + TextBox8.Text.Trim() + "';", connection);
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
        void signUpNewMember()
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO member_master_tbl" +
                    "(full_name,dob,phone,email,province,city,postal_code,full_address,member_id,password,account_status) " +
                    "values(@full_name,@dob,@phone,@email,@province,@city,@postal_code,@full_address,@member_id,@password,@account_status)",
                    connection);
                sqlCmd.Parameters.AddWithValue("@full_name", TextBox1.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@dob", TextBox2.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@phone", TextBox3.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@email", TextBox4.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@province", DropDownList1.SelectedItem.Value);
                sqlCmd.Parameters.AddWithValue("@city", TextBox6.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@postal_code", TextBox7.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@full_address", TextBox5.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@member_id", TextBox8.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@password", TextBox9.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@account_status", "Active");
                sqlCmd.ExecuteNonQuery();
                connection.Close();
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.success('Sign Up Successful. Go to User Login to Login')", true);
            }
            catch (Exception ex)
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Error!!')", true);
            }
        }
    }
}