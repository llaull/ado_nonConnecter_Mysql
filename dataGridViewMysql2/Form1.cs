using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Diagnostics;

namespace dataGridViewMysql2
{
    public partial class Form1 : Form
    {
        MySqlDataAdapter dataAdap;
        MySqlConnection connex;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            connex = new MySqlConnection("Server=localhost;Database=app_domobox;Uid=root;Pwd=;");
            dataAdap = new MySqlDataAdapter();
            dataAdap.SelectCommand = connex.CreateCommand();
            dataAdap.UpdateCommand = connex.CreateCommand();
            dataAdap.InsertCommand = connex.CreateCommand();
            dataAdap.DeleteCommand = connex.CreateCommand();

            // mapping des collones avec la table MySQL appelé 
            DataTableMapping tableMapping = new DataTableMapping();
            tableMapping.SourceTable = "Table";
            tableMapping.DataSetTable = "TableModule";
            tableMapping.ColumnMappings.Add("id", "id");
            tableMapping.ColumnMappings.Add("module_ref", "module_ref");
            tableMapping.ColumnMappings.Add("module_nom", "module_nom");
            tableMapping.ColumnMappings.Add("module_emplacement", "module_emplacement");
            tableMapping.ColumnMappings.Add("nrf_id", "nrf_id");
            tableMapping.ColumnMappings.Add("module_type", "module_type");
            dataAdap.TableMappings.Add(tableMapping);

            //adapteur pour le SELECT
            dataAdap.SelectCommand.CommandText = "SELECT * FROM app_domobox.domotic_sensor_module;";

            //adapteur pour le DELETE
            dataAdap.DeleteCommand.CommandText = "DELETE FROM app_domobox.domotic_sensor_module WHERE id=@id;";
            dataAdap.DeleteCommand.Parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32, 0, "id"));

            //adapteur pour l'UPDATE
            dataAdap.UpdateCommand.CommandText = "UPDATE app_domobox.domotic_sensor_module SET module_ref=@module_ref,module_nom=@module_nom,module_emplacement=@module_emplacement,nrf_id=@nrf_id,module_type=@module_type WHERE id=@id;";
            dataAdap.UpdateCommand.Parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32, 0, "id"));
            dataAdap.UpdateCommand.Parameters.Add(new MySqlParameter("@module_ref", MySqlDbType.Int32, 0, "module_ref"));
            dataAdap.UpdateCommand.Parameters.Add(new MySqlParameter("@module_nom", MySqlDbType.VarChar, 0, "module_nom"));
            dataAdap.UpdateCommand.Parameters.Add(new MySqlParameter("@module_emplacement", MySqlDbType.VarChar, 0, "module_emplacement"));
            dataAdap.UpdateCommand.Parameters.Add(new MySqlParameter("@nrf_id", MySqlDbType.Int32, 0, "nrf_id"));
            dataAdap.UpdateCommand.Parameters.Add(new MySqlParameter("@module_type", MySqlDbType.VarChar, 0, "module_type"));

            //adapteur pour l'INSERT
            dataAdap.InsertCommand.CommandText = "INSERT INTO app_domobox.domotic_sensor_module (module_ref,module_nom,module_emplacement,nrf_id,module_type) VALUES (@module_ref,@module_nom,@module_emplacement,@nrf_id,@module_type);";// select LAST_INSERT_ID() AS id;
            dataAdap.InsertCommand.Parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32, 0, "id")).Direction = ParameterDirection.Output;
            dataAdap.InsertCommand.Parameters.Add(new MySqlParameter("@module_ref", MySqlDbType.Int32, 0, "module_ref"));
            dataAdap.InsertCommand.Parameters.Add(new MySqlParameter("@module_nom", MySqlDbType.VarChar, 0, "module_nom"));
            dataAdap.InsertCommand.Parameters.Add(new MySqlParameter("@module_emplacement", MySqlDbType.VarChar, 0, "module_emplacement"));
            dataAdap.InsertCommand.Parameters.Add(new MySqlParameter("@nrf_id", MySqlDbType.Int32, 0, "nrf_id"));
            dataAdap.InsertCommand.Parameters.Add(new MySqlParameter("@module_type", MySqlDbType.VarChar, 0, "module_type"));
            
            //attache le resultat du SELECT dans un DataTable
            DataTable table = new DataTable("TableModule");
            dataAdap.Fill(table);

            //attache la DataTable dans le Navigator
            BindingSource binder = new BindingSource(table,null);
            dataGridView1.DataSource = binder;
            bindingNavigator1.BindingSource = binder;

            //function sur les actions effectué sur le DataTable
            dataAdap.RowUpdated += dataAdap_RowUpdated;
            table.RowChanged += table_RowChanged;
            table.RowDeleted += table_RowChanged;

            //dataAdap.RowUpdating += new MySqlRowUpdatingEventHandler(adapRowUpdatingEventHandler); //debug
        }

        /// <summary>
        /// appeler si suppression d'elements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dataAdap_RowUpdated(object sender, MySqlRowUpdatedEventArgs e)
        {
            if (e.RecordsAffected == 1 && e.Command == dataAdap.InsertCommand)//si une ligne a été modifié et que la commande sql est un insert
            {
                //SET LAST_INSERT_ID();
                MySqlCommand idQuery = connex.CreateCommand();
                idQuery.CommandText = "SELECT LAST_INSERT_ID();"; //renvoie l'id du dernier enregistement
                int id = (int)(ulong)idQuery.ExecuteScalar();
                dataAdap.InsertCommand.Parameters["@id"].Value = id;
                e.Row["id"] = id; // affiche l'id dans la collone id
            }
        }

        /// <summary>
        /// appeler si un changement à lieu sur une ligne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void table_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            //connex.CancelQuery(0);
            dataAdap.Update((DataTable)((BindingSource)dataGridView1.DataSource).DataSource);
        }

        //debug
        /*void adapRowUpdatingEventHandler(
	        Object sender,
	        MySqlRowUpdatingEventArgs e
        )
        {
            PrintEventArgs(e);
        }

        private static void PrintEventArgs(MySqlRowUpdatingEventArgs args)
        {
            Debug.Print("OnRowUpdating");
            Debug.Print("  event args: (" +
                " command=" + args.Command +
                " commandType=" + args.StatementType +
                " status=" + args.Status + ")");
        }*/
    }
}
