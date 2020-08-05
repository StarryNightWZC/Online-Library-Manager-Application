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
    public partial class userbooks : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //if (Session["username"].ToString() == "" || Session["username"] == null)
                if (string.IsNullOrEmpty((string)Session["username"]))
                {
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.warning('Session Expired Login Again')", true);
                    Response.Redirect("userlogin.aspx");
                }
                else
                {
                    getUserBookData();
                }
            }
            catch (Exception ex)
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.warning('Session Expired Login Again')", true);
                Response.Redirect("userlogin.aspx");
            }
        }

        void getUserBookData()
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                SqlCommand sqlCmd = new SqlCommand("SELECT * FROM book_issue_tbl WHERE member_id='" + Session["username"].ToString() + "';", connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                GridView1.DataSource = dataTable;
                GridView1.DataBind();
            }
            catch (Exception ex)
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Error!!')", true);
            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //Check your condition here
                    DateTime dateTime = Convert.ToDateTime(e.Row.Cells[5].Text);
                    DateTime today = DateTime.Today;
                    if (today > dateTime)
                    {
                        e.Row.BackColor = System.Drawing.Color.PaleVioletRed;
                    }
                }
            }
            catch (Exception ex)
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Error!!')", true);
            }
        }
    }
}