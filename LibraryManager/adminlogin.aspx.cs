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
    public partial class adminlogin : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        // login button click event
        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();

                }
                SqlCommand sqlCmd = new SqlCommand("SELECT * FROM admin_login_tbl WHERE username='" +
                    TextBox1.Text.Trim() + "' AND password='" + TextBox2.Text.Trim() + "'", connection);
                SqlDataReader dataReader = sqlCmd.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.success('Successful login')", true);
                        Session["username"] = dataReader.GetValue(0).ToString();
                        Session["fullname"] = dataReader.GetValue(2).ToString();
                        Session["role"] = "admin";
                        //Session["status"] = dataReader.GetValue(10).ToString();
                    }
                    Response.Redirect("homepage.aspx");
                }
                else
                {
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Invalid credentials')", true);
                }

            }
            catch (Exception ex)
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Error!!')", true);
            }
        }
    }
}