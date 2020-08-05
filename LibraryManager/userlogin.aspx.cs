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
    public partial class userlogin : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        // user login
        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();

                }
                SqlCommand sqlCmd = new SqlCommand("SELECT * FROM member_master_tbl WHERE member_id='" +
                    TextBox1.Text.Trim() + "' AND password='" + TextBox2.Text.Trim() + "'", connection);
                SqlDataReader dataReader = sqlCmd.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.success('Login Successful')", true);
                        Session["username"] = dataReader.GetValue(8).ToString();
                        Session["fullname"] = dataReader.GetValue(0).ToString();
                        Session["role"] = "user";
                        //Session["status"] = dr.GetValue(10).ToString();
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

            }
        }
        protected void Button2_Click(object sender, EventArgs e)
        {
            Response.Redirect("usersignup.aspx");
        }
    }
}