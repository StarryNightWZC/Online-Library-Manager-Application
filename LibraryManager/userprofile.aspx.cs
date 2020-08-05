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
    public partial class userprofile : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //if (Session["username"].ToString() == "" || Session["username"] == null)
                if (string.IsNullOrEmpty((string)Session["username"]))
                {
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Session Expired, Please Login Again')", true);
                    Response.Redirect("userlogin.aspx");
                }
                else
                {
                    if (!Page.IsPostBack)
                    {
                        getUserPersonalDetails();
                    }
                }
            }
            catch (Exception ex)
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Session Expired, Please Login Again')", true);
                Response.Redirect("userlogin.aspx");
            }
        }

        // update button click
        protected void Button1_Click(object sender, EventArgs e)
        {
            if (Session["username"].ToString() == "" || Session["username"] == null)
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Session Expired, Please Login Again')", true);
                Response.Redirect("userlogin.aspx");
            }
            else
            {
                updateUserPersonalDetails();
            }
        }

        // user defined function
        void updateUserPersonalDetails()
        {
            string password = "";
            if (TextBox10.Text.Trim() == "")
            {
                password = TextBox9.Text.Trim();
            }
            else
            {
                password = TextBox10.Text.Trim();
            }
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                SqlCommand sqlCmd = new SqlCommand("UPDATE member_master_tbl SET full_name=@full_name, dob=@dob, phone=@phone, email=@email, " +
                    "province=@province, city=@city, postal_code=@postal_code, full_address=@full_address, password=@password, " +
                    "account_status=@account_status WHERE member_id='" + Session["username"].ToString().Trim() + "'", connection);
                sqlCmd.Parameters.AddWithValue("@full_name", TextBox1.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@dob", TextBox2.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@phone", TextBox3.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@email", TextBox4.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@province", DropDownList1.SelectedItem.Value);
                sqlCmd.Parameters.AddWithValue("@city", TextBox6.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@postal_code", TextBox7.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@full_address", TextBox5.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@password", password);
                sqlCmd.Parameters.AddWithValue("@account_status", "Active");
                int result = sqlCmd.ExecuteNonQuery();
                connection.Close();
                if (result > 0)
                {
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.success('Your Details Updated Successfully')", true);
                    getUserPersonalDetails();
                }
                else
                {
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Invaid Entry')", true);
                }
            }
            catch (Exception ex)
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Error!!')", true);
            }
        }


        void getUserPersonalDetails()
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                SqlCommand sqlCmd = new SqlCommand("SELECT * FROM member_master_tbl WHERE member_id='" + Session["username"].ToString() + "';", connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                TextBox1.Text = dataTable.Rows[0]["full_name"].ToString();
                TextBox2.Text = dataTable.Rows[0]["dob"].ToString();
                TextBox3.Text = dataTable.Rows[0]["phone"].ToString();
                TextBox4.Text = dataTable.Rows[0]["email"].ToString();
                DropDownList1.SelectedValue = dataTable.Rows[0]["province"].ToString().Trim();
                TextBox6.Text = dataTable.Rows[0]["city"].ToString();
                TextBox7.Text = dataTable.Rows[0]["postal_code"].ToString();
                TextBox5.Text = dataTable.Rows[0]["full_address"].ToString();
                TextBox8.Text = dataTable.Rows[0]["member_id"].ToString();
                TextBox9.Text = dataTable.Rows[0]["password"].ToString();
                Label1.Text = dataTable.Rows[0]["account_status"].ToString().Trim();
                if (dataTable.Rows[0]["account_status"].ToString().Trim() == "active")
                {
                    Label1.Attributes.Add("class", "badge badge-pill badge-success");
                }
                else if (dataTable.Rows[0]["account_status"].ToString().Trim() == "pending")
                {
                    Label1.Attributes.Add("class", "badge badge-pill badge-warning");
                }
                else if (dataTable.Rows[0]["account_status"].ToString().Trim() == "deactive")
                {
                    Label1.Attributes.Add("class", "badge badge-pill badge-danger");
                }
                else
                {
                    Label1.Attributes.Add("class", "badge badge-pill badge-info");
                }
            }
            catch (Exception ex)
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Error!!')", true);
            }
        }

    }
}