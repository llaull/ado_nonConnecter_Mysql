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
            // TODO: cette ligne de code charge les données dans la table 'dataSet1.DataTable'. Vous pouvez la déplacer ou la supprimer selon vos besoins.
            //this.dataTableTableAdapter.Fill(this.dataSet1.DataTable);
            connex = new MySqlConnection("Server=localhost;Database=app_domobox;Uid=root;Pwd=;");
            dataAdap = new MySqlDataAdapter();
            dataAdap.SelectCommand = connex.CreateCommand();
            dataAdap.UpdateCommand = connex.CreateCommand();
            dataAdap.InsertCommand = connex.CreateCommand();
            dataAdap.DeleteCommand = connex.CreateCommand();

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

            dataAdap.SelectCommand.CommandText = "SELECT * FROM app_domobox.domotic_sensor_module;";
            //dataAdap.DeleteCommand.CommandText = "DELETE FROM app_domobox.domotic_sensor_module WHERE ([id] = @id);";
            dataAdap.DeleteCommand.CommandText = "DELETE FROM app_domobox.domotic_sensor_module WHERE id=@id;";
            dataAdap.DeleteCommand.Parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32, 0, "id"));

            dataAdap.UpdateCommand.CommandText = "UPDATE app_domobox.domotic_sensor_module SET module_ref=@module_ref,module_nom=@module_nom,module_emplacement=@module_emplacement,nrf_id=@nrf_id,module_type=@module_type WHERE id=@id;";
            dataAdap.UpdateCommand.Parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32, 0, "id"));
            dataAdap.UpdateCommand.Parameters.Add(new MySqlParameter("@module_ref", MySqlDbType.Int32, 0, "module_ref"));
            dataAdap.UpdateCommand.Parameters.Add(new MySqlParameter("@module_nom", MySqlDbType.VarChar, 0, "module_nom"));
            dataAdap.UpdateCommand.Parameters.Add(new MySqlParameter("@module_emplacement", MySqlDbType.VarChar, 0, "module_emplacement"));
            dataAdap.UpdateCommand.Parameters.Add(new MySqlParameter("@nrf_id", MySqlDbType.Int32, 0, "nrf_id"));
            dataAdap.UpdateCommand.Parameters.Add(new MySqlParameter("@module_type", MySqlDbType.VarChar, 0, "module_type"));

            dataAdap.InsertCommand.CommandText = "INSERT INTO app_domobox.domotic_sensor_module (module_ref,module_nom,module_emplacement,nrf_id,module_type) VALUES (@module_ref,@module_nom,@module_emplacement,@nrf_id,@module_type);";// select LAST_INSERT_ID() AS id;
            dataAdap.InsertCommand.Parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32, 0, "id")).Direction = ParameterDirection.Output;
            dataAdap.InsertCommand.Parameters.Add(new MySqlParameter("@module_ref", MySqlDbType.Int32, 0, "module_ref"));
            dataAdap.InsertCommand.Parameters.Add(new MySqlParameter("@module_nom", MySqlDbType.VarChar, 0, "module_nom"));
            dataAdap.InsertCommand.Parameters.Add(new MySqlParameter("@module_emplacement", MySqlDbType.VarChar, 0, "module_emplacement"));
            dataAdap.InsertCommand.Parameters.Add(new MySqlParameter("@nrf_id", MySqlDbType.Int32, 0, "nrf_id"));
            dataAdap.InsertCommand.Parameters.Add(new MySqlParameter("@module_type", MySqlDbType.VarChar, 0, "module_type"));
            
            DataTable table = new DataTable("TableModule");
            dataAdap.Fill(table);
            BindingSource binder = new BindingSource(table,null);
            dataGridView1.DataSource = binder;
            bindingNavigator1.BindingSource = binder;
            //dataAdap.RowUpdating += new MySqlRowUpdatingEventHandler(adapRowUpdatingEventHandler);
            dataAdap.RowUpdated += dataAdap_RowUpdated;
            table.RowChanged += table_RowChanged;
            table.RowDeleted += table_RowChanged;
        }

        void dataAdap_RowUpdated(object sender, MySqlRowUpdatedEventArgs e)
        {
            if (e.RecordsAffected == 1 && e.Command == dataAdap.InsertCommand)
            {
                //SET LAST_INSERT_ID();
                MySqlCommand idQuery = connex.CreateCommand();
                idQuery.CommandText = "SELECT LAST_INSERT_ID();";
                int id = (int)(ulong)idQuery.ExecuteScalar();
                dataAdap.InsertCommand.Parameters["@id"].Value = id;
                e.Row["id"] = id;
            }
        }

        void table_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            //connex.CancelQuery(0);
            dataAdap.Update((DataTable)((BindingSource)dataGridView1.DataSource).DataSource);
        }

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
