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
    public partial class adminbookissuing : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            GridView1.DataBind();
        }

        // issue book
        protected void Button2_Click(object sender, EventArgs e)
        {
            if (checkIfBookExist() && checkIfMemberExist())
            {
                if (checkIfIssueEntryExist())
                {
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.warning('This Member already has this book')", true);
                }
                else
                {
                    issueBook();
                }
            }
            else
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Wrong Book ID or Member ID')", true);
            }
        }
        // return book
        protected void Button4_Click(object sender, EventArgs e)
        {
            if (checkIfBookExist() && checkIfMemberExist())
            {

                if (checkIfIssueEntryExist())
                {
                    returnBook();
                }
                else
                {
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.warning('This Entry does not exist')", true);
                }

            }
            else
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Wrong Book ID or Member ID')", true);
            }
        }

        // go button click event
        protected void Button1_Click(object sender, EventArgs e)
        {
            getNames();
        }

        // user defined function
        void returnBook()
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                SqlCommand sqlCmd = new SqlCommand("Delete FROM book_issue_tbl WHERE book_id='"
                    + TextBox1.Text.Trim() + "' AND member_id='" + TextBox2.Text.Trim() + "'", connection);
                int result = sqlCmd.ExecuteNonQuery();
                if (result > 0)
                {
                    sqlCmd = new SqlCommand("UPDATE book_master_tbl SET current_stock = current_stock+1 WHERE book_id='" + TextBox1.Text.Trim() + "'", connection);
                    sqlCmd.ExecuteNonQuery();
                    connection.Close();
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.success('Book Returned Successfully')", true);
                    connection.Close();
                }
                else
                {
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Error - Invalid details')", true);
                }
            }
            catch (Exception ex)
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Error!!')", true);
            }
        }

        void issueBook()
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO book_issue_tbl(member_id,member_name,book_id,book_name,issue_date,due_date) " +
                    "values(@member_id,@member_name,@book_id,@book_name,@issue_date,@due_date)", connection);
                sqlCmd.Parameters.AddWithValue("@member_id", TextBox2.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@member_name", TextBox3.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@book_id", TextBox1.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@book_name", TextBox4.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@issue_date", TextBox5.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@due_date", TextBox6.Text.Trim());
                sqlCmd.ExecuteNonQuery();
                sqlCmd = new SqlCommand("UPDATE book_master_tbl SET current_stock = current_stock-1 WHERE book_id='" + TextBox1.Text.Trim() + "'", connection);
                sqlCmd.ExecuteNonQuery();
                connection.Close();
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.success('Book Issued Successfully')", true);
            }
            catch (Exception ex)
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Error!!')", true);
            }
        }

        bool checkIfBookExist()
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                SqlCommand sqlCmd = new SqlCommand("SELECT * FROM book_master_tbl WHERE book_id='" + TextBox1.Text.Trim() + "' AND current_stock >0", connection);
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
                return false;
            }
        }

        bool checkIfMemberExist()
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                SqlCommand sqlCmd = new SqlCommand("SELECT full_name FROM member_master_tbl WHERE member_id='" + TextBox2.Text.Trim() + "'", connection);
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
                return false;
            }
        }

        bool checkIfIssueEntryExist()
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                SqlCommand sqlCmd = new SqlCommand("SELECT * FROM book_issue_tbl WHERE member_id='"
                    + TextBox2.Text.Trim() + "' AND book_id='" + TextBox1.Text.Trim() + "'", connection);
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
                return false;
            }
        }

        void getNames()
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                SqlCommand sqlCmd = new SqlCommand("SELECT book_name FROM book_master_tbl WHERE book_id='" + TextBox1.Text.Trim() + "'", connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                if (dataTable.Rows.Count >= 1)
                {
                    TextBox4.Text = dataTable.Rows[0]["book_name"].ToString();
                }
                else
                {
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Wrong Book ID')", true);
                }
                sqlCmd = new SqlCommand("SELECT full_name FROM member_master_tbl WHERE member_id='" + TextBox2.Text.Trim() + "'", connection);
                dataAdapter = new SqlDataAdapter(sqlCmd);
                dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                if (dataTable.Rows.Count >= 1)
                {
                    TextBox3.Text = dataTable.Rows[0]["full_name"].ToString();
                }
                else
                {
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Wrong User ID')", true);
                }
            }
            catch (Exception ex)
            {

            }
        }
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //Check your condition here
                    DateTime dt = Convert.ToDateTime(e.Row.Cells[5].Text);
                    DateTime today = DateTime.Today;
                    if (today > dt)
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