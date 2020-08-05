using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace LibraryManager
{
    public partial class adminbookinventory : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        static string global_filepath;
        static int global_actual_stock, global_current_stock, global_issued_books;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                fillAuthorPublisherValues();
            }
            //GridView1.DataBind();
        }

        // go button click
        protected void Button4_Click(object sender, EventArgs e)
        {
            getBookByID();
        }


        // update button click
        protected void Button3_Click(object sender, EventArgs e)
        {
            updateBookByID();
        }
        // delete button click
        protected void Button2_Click(object sender, EventArgs e)
        {
            deleteBookByID();
        }
        // add button click
        protected void Button1_Click(object sender, EventArgs e)
        {
            if (checkIfBookExists())
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.warning('Book Already Exists, try some other Book ID')", true);
            }
            else
            {
                addNewBook();
            }
        }



        // user defined functions

        void deleteBookByID()
        {
            if (checkIfBookExists())
            {
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    SqlCommand sqlCmd = new SqlCommand("DELETE FROM book_master_tbl WHERE book_id='" + TextBox1.Text.Trim() + "'", connection);
                    sqlCmd.ExecuteNonQuery();
                    connection.Close();
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.success('Book Deleted Successfully')", true);
                    //GridView1.DataBind();
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

        void updateBookByID()
        {
            if (checkIfBookExists())
            {
                try
                {
                    int actual_stock = Convert.ToInt32(TextBox4.Text.Trim());
                    int current_stock = Convert.ToInt32(TextBox5.Text.Trim());
                    if (global_actual_stock == actual_stock)
                    {
                        // do nothing
                    }
                    else
                    {
                        if (actual_stock < global_issued_books)
                        {
                            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Actual Stock value cannot be less than the Issued books')", true);
                            return;
                        }
                        else
                        {
                            current_stock = actual_stock - global_issued_books;
                            TextBox5.Text = "" + current_stock;
                        }
                    }

                    string genres = "";
                    foreach (int i in ListBox1.GetSelectedIndices())
                    {
                        genres = genres + ListBox1.Items[i] + ",";
                    }
                    genres = genres.Remove(genres.Length - 1);
                    string filepath = "~/book_inventory/books1";
                    string filename = Path.GetFileName(FileUpload1.PostedFile.FileName);
                    if (filename == "" || filename == null)
                    {
                        filepath = global_filepath;
                    }
                    else
                    {
                        FileUpload1.SaveAs(Server.MapPath("book_inventory/" + filename));
                        filepath = "~/book_inventory/" + filename;
                    }

                    SqlConnection connection = new SqlConnection(connectionString);
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    SqlCommand sqlCmd = new SqlCommand("UPDATE book_master_tbl set book_name=@book_name, genre=@genre, " +
                        "author_name=@author_name, publisher_name=@publisher_name, publish_date=@publish_date, language=@language, " +
                        "edition=@edition, book_cost=@book_cost, no_of_pages=@no_of_pages, book_description=@book_description, " +
                        "actual_stock=@actual_stock, current_stock=@current_stock, book_img_link=@book_img_link where book_id='" + 
                        TextBox1.Text.Trim() + "'", connection);

                    sqlCmd.Parameters.AddWithValue("@book_name", TextBox2.Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@genre", genres);
                    sqlCmd.Parameters.AddWithValue("@author_name", DropDownList3.SelectedItem.Value);
                    sqlCmd.Parameters.AddWithValue("@publisher_name", DropDownList2.SelectedItem.Value);
                    sqlCmd.Parameters.AddWithValue("@publish_date", TextBox3.Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@language", DropDownList1.SelectedItem.Value);
                    sqlCmd.Parameters.AddWithValue("@edition", TextBox9.Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@book_cost", TextBox10.Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@no_of_pages", TextBox11.Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@book_description", TextBox6.Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@actual_stock", actual_stock.ToString());
                    sqlCmd.Parameters.AddWithValue("@current_stock", current_stock.ToString());
                    sqlCmd.Parameters.AddWithValue("@book_img_link", filepath);
                    sqlCmd.ExecuteNonQuery();
                    connection.Close();
                    //GridView1.DataBind();
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.success('Book Updated Successfully')", true);
                }
                catch (Exception ex)
                {
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Error!!')", true);
                }
            }
            else
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Invalid Book ID')", true);
            }
        }


        void getBookByID()
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                SqlCommand sqlCmd = new SqlCommand("SELECT * FROM book_master_tbl WHERE book_id='" + TextBox1.Text.Trim() + "';", connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                if (dataTable.Rows.Count >= 1)
                {
                    TextBox2.Text = dataTable.Rows[0]["book_name"].ToString();
                    TextBox3.Text = dataTable.Rows[0]["publish_date"].ToString();
                    TextBox9.Text = dataTable.Rows[0]["edition"].ToString();
                    TextBox10.Text = dataTable.Rows[0]["book_cost"].ToString().Trim();
                    TextBox11.Text = dataTable.Rows[0]["no_of_pages"].ToString().Trim();
                    TextBox4.Text = dataTable.Rows[0]["actual_stock"].ToString().Trim();
                    TextBox5.Text = dataTable.Rows[0]["current_stock"].ToString().Trim();
                    TextBox6.Text = dataTable.Rows[0]["book_description"].ToString();
                    TextBox7.Text = "" + (Convert.ToInt32(dataTable.Rows[0]["actual_stock"].ToString()) - 
                        Convert.ToInt32(dataTable.Rows[0]["current_stock"].ToString()));

                    DropDownList1.SelectedValue = dataTable.Rows[0]["language"].ToString().Trim();
                    DropDownList2.SelectedValue = dataTable.Rows[0]["publisher_name"].ToString().Trim();
                    DropDownList3.SelectedValue = dataTable.Rows[0]["author_name"].ToString().Trim();

                    ListBox1.ClearSelection();
                    string[] genre = dataTable.Rows[0]["genre"].ToString().Trim().Split(',');
                    for (int i = 0; i < genre.Length; i++)
                    {
                        for (int j = 0; j < ListBox1.Items.Count; j++)
                        {
                            if (ListBox1.Items[j].ToString() == genre[i])
                            {
                                ListBox1.Items[j].Selected = true;
                            }
                        }
                    }
                    global_actual_stock = Convert.ToInt32(dataTable.Rows[0]["actual_stock"].ToString().Trim());
                    global_current_stock = Convert.ToInt32(dataTable.Rows[0]["current_stock"].ToString().Trim());
                    global_issued_books = global_actual_stock - global_current_stock;
                    global_filepath = dataTable.Rows[0]["book_img_link"].ToString();
                }
                else
                {
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Invalid Book ID')", true);
                }
            }
            catch (Exception ex)
            {

            }
        }

        void fillAuthorPublisherValues()
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                SqlCommand sqlCmd = new SqlCommand("SELECT author_name FROM author_master_tbl;", connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                DropDownList3.DataSource = dataTable;
                DropDownList3.DataValueField = "author_name";
                DropDownList3.DataBind();

                sqlCmd = new SqlCommand("SELECT publisher_name FROM publisher_master_tbl;", connection);
                dataAdapter = new SqlDataAdapter(sqlCmd);
                dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                DropDownList2.DataSource = dataTable;
                DropDownList2.DataValueField = "publisher_name";
                DropDownList2.DataBind();
            }
            catch (Exception ex)
            {

            }
        }

        bool checkIfBookExists()
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                SqlCommand sqlCmd = new SqlCommand("SELECT * from book_master_tbl where book_id='" + 
                    TextBox1.Text.Trim() + "' OR book_name='" + TextBox2.Text.Trim() + "';", connection);
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

        void addNewBook()
        {
            try
            {
                string genres = "";
                foreach (int i in ListBox1.GetSelectedIndices())
                {
                    genres = genres + ListBox1.Items[i] + ",";
                }
                genres = genres.Remove(genres.Length - 1);
                string filepath = "~/imgs/books1.png";
                string filename = Path.GetFileName(FileUpload1.PostedFile.FileName);
                FileUpload1.SaveAs(Server.MapPath("book_inventory/" + filename));
                filepath = "~/book_inventory/" + filename;

                SqlConnection connection = new SqlConnection(connectionString);
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO book_master_tbl(book_id,book_name,genre,author_name,publisher_name,publish_date," +
                    "language,edition,book_cost,no_of_pages,book_description,actual_stock,current_stock,book_img_link) " +
                    "values(@book_id,@book_name,@genre,@author_name,@publisher_name,@publish_date,@language,@edition,@book_cost,@no_of_pages," +
                    "@book_description,@actual_stock,@current_stock,@book_img_link)", connection);
                sqlCmd.Parameters.AddWithValue("@book_id", TextBox1.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@book_name", TextBox2.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@genre", genres);
                sqlCmd.Parameters.AddWithValue("@author_name", DropDownList3.SelectedItem.Value);
                sqlCmd.Parameters.AddWithValue("@publisher_name", DropDownList2.SelectedItem.Value);
                sqlCmd.Parameters.AddWithValue("@publish_date", TextBox3.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@language", DropDownList1.SelectedItem.Value);
                sqlCmd.Parameters.AddWithValue("@edition", TextBox9.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@book_cost", TextBox10.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@no_of_pages", TextBox11.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@book_description", TextBox6.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@actual_stock", TextBox4.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@current_stock", TextBox4.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@book_img_link", filepath);
                sqlCmd.ExecuteNonQuery();
                connection.Close();
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.success('Book added successfully')", true);
                //GridView1.DataBind();
            }
            catch (Exception ex)
            {

                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.error('Error!!')", true);
            }
        }
    }
}