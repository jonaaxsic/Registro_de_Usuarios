using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Crud.Pages
{
    public partial class Crud : System.Web.UI.Page
    {

        readonly SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conexion"].ConnectionString);
        private void CargarRoles()
        {
            ddlRol.Items.Clear();
            ddlRol.Items.Add(new ListItem("Seleccione un rol...", "")); // Opción por defecto

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conexion"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT Id, NombreRol FROM Roles", con);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ddlRol.Items.Add(new ListItem(dr["NombreRol"].ToString(), dr["Id"].ToString()));
                }
                con.Close();
            }
        }
        public static string sID = "0";
        public static string sOpc = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            //obtener id
            if (!Page.IsPostBack)
            {
                CargarRoles();

                if (Request.QueryString["id"]!=null)
                {
                    sID = Request.QueryString["id"].ToString();
                    CargarDatos();
                }

                if (Request.QueryString["op"] != null)
                {
                    sOpc = Request.QueryString["op"].ToString();

                    switch (sOpc)
                    {
                        case "C":
                            this.lbltitulo.Text = "Ingresar nuevo Usuario";
                            this.BtnCreate.Visible = true;
                            break;
                        case "R":
                            this.lbltitulo.Text = "Modificar ususario";
                            this.BtnUpdate.Visible = true;
                            break;
                        case "D":
                            this.lbltitulo.Text = "Eliminar Usuario";
                            this.BtnDelete.Visible = true;
                            break;
                    }
                }

            }
        }
       
        void CargarDatos()
        {
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter("sp_leer", con);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.Add("@id", SqlDbType.Int).Value = sID;
            DataSet ds = new DataSet();
            ds.Clear();
            da.Fill(ds);
            DataTable dt = ds.Tables[0];
            DataRow row = dt.Rows[0];
            tbnombre.Text = row[1].ToString();
            tbedad.Text = row[2].ToString();
            tbemail.Text = row[3].ToString();
            tbdireccion.Text = row[4].ToString();
            DateTime d= (DateTime)row[5];
            tbdate.Text = d.ToString("dd/MM/YYYY");
            ddlRol.Text = row[6].ToString();
            con.Close();
        }
        protected void BtnCreate_Click(object sender, EventArgs e)
        {

            lblError.Text = ""; // Limpia mensajes previos

            // Validar nombre
            if (string.IsNullOrWhiteSpace(tbnombre.Text))
            {
                lblError.Text = "El nombre es obligatorio.";
                return;
            }

            // Validar edad
            int edad;
            if (!int.TryParse(tbedad.Text, out edad))
            {
                lblError.Text = "La edad debe ser un número válido.";
                return;
            }

            // Validar email
            if (string.IsNullOrWhiteSpace(tbemail.Text))
            {
                lblError.Text = "El email es obligatorio.";
                return;
            }

            // Validar dirección
            if (string.IsNullOrWhiteSpace(tbdireccion.Text))
            {
                lblError.Text = "La dirección es obligatoria.";
                return;
            }

            // Validar fecha de nacimiento
            DateTime fechaNac;
            if (!DateTime.TryParse(tbdate.Text, out fechaNac))
            {
                lblError.Text = "La fecha de nacimiento no es válida.";
                return;
            }

            // Validar rol
            int rolId;
            if (!int.TryParse(ddlRol.Text, out rolId))
            {
                lblError.Text = "Debe seleccionar un rol válido.";
                return;
            }


            SqlCommand cmd = new SqlCommand("sp_crear", con);
            con.Open();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Nombre_APe",SqlDbType.NVarChar).Value = tbnombre.Text;
            cmd.Parameters.Add("@edad", SqlDbType.Int).Value = edad;
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = tbemail.Text;
            cmd.Parameters.Add("@Direccion", SqlDbType.NVarChar).Value = tbdireccion.Text;
            cmd.Parameters.Add("@Fecha_nac", SqlDbType.Date).Value = fechaNac;
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = rolId;
            cmd.ExecuteNonQuery();
            con.Close();
            Response.Redirect("Index.aspx");
        }

        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("sp_actualizar", con);
            con.Open();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("Id_User", SqlDbType.Int).Value = sID;
            cmd.Parameters.Add("@Nombre_APe", SqlDbType.NVarChar).Value = tbnombre.Text;
            cmd.Parameters.Add("@edad", SqlDbType.Int).Value = tbedad.Text;
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = tbemail.Text;
            cmd.Parameters.Add("@Direccion", SqlDbType.NVarChar).Value = tbdireccion.Text;
            cmd.Parameters.Add("@Fecha_nac", SqlDbType.Date).Value = tbdate.Text;

            int rolId;
            if (!int.TryParse(ddlRol.Text, out rolId))
            {
                lblError.Text = "Debe seleccionar un rol válido.";
                con.Close();
                return;
            }
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = rolId;

            cmd.ExecuteNonQuery();
            con.Close();
            Response.Redirect("Crud.aspx?id=" + sID + "&op=R");
        }

        protected void BtnDelete_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("sp_eliminar", con);
            con.Open();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("Id_User", SqlDbType.Int).Value = sID;
            cmd.ExecuteNonQuery();
            con.Close();
            Response.Redirect("Index.aspx");
        }
      
        protected void BtnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("Index.aspx");
        }
    }
}