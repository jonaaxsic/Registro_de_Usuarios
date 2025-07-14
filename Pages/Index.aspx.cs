using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;


namespace Crud.Pages
{
    public partial class Index : System.Web.UI.Page
    {
        readonly SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conexion"].ConnectionString);
        protected void Page_Load(object sender, EventArgs e)
        {
            CargarTabla();
        }

        void CargarTabla()
        {
            SqlCommand cmd = new SqlCommand("sp_CargarDatos", con);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            gvusuarios.DataSource = dt;
            gvusuarios.DataBind();
            con.Close();

        }

        protected void BtnCrear_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Pages/Crud.aspx?op=C");
        }




        protected void BtnRead_Click(object sender, EventArgs e)
        {
            string id;
            Button BtnConsultar = (Button)sender;
            GridViewRow selecedrow = (GridViewRow)BtnConsultar.NamingContainer;
            id = selecedrow.Cells[1].Text;
            Response.Redirect("/Pages/Crud.aspx?id="+id+"&op=R");
        }

        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            string id;
            Button BtnConsultar = (Button)sender;
            GridViewRow selecedrow = (GridViewRow)BtnConsultar.NamingContainer;
            id = selecedrow.Cells[1].Text;
            Response.Redirect("/Pages/Crud.aspx?id=" + id + "&op=U");
        }

        protected void BtnDelete_Click(object sender, EventArgs e)
        {
            string id;
            Button BtnConsultar = (Button)sender;
            GridViewRow selecedrow = (GridViewRow)BtnConsultar.NamingContainer;
            id = selecedrow.Cells[1].Text;
            Response.Redirect("/Pages/Crud.aspx?id=" + id + "&op=D ");
        }
    }
}