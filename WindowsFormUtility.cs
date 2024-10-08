using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUWindowsFormFramework
{
    public class WindowsFormUtility
    {
        public static void SetListBinding(ComboBox lst, DataTable sourceDT, DataTable? targetDT, string tablename)
        {
            lst.DataSource = sourceDT;
            lst.ValueMember = tablename + "ID";
            lst.DisplayMember = lst.Name.Substring(3);
            if (targetDT != null) { 
            lst.DataBindings.Add("selectedValue", targetDT, lst.ValueMember, false, DataSourceUpdateMode.OnPropertyChanged);
        }
        }

        public static void SetControlBinding(Control ctrl, BindingSource bindsource)
        {
            string propertyname = "";
            string controlname = ctrl.Name.ToLower();
            string controltype = controlname.Substring(0, 3);
            string columnname = controlname.Substring(3);
            switch (controltype)
            {
                case "txt":
                case "lbl":
                    propertyname = "text";
                    break;
                case "dtp":
                    propertyname = "value";
                    break;
            }
            if (propertyname != " " && columnname != " ")
            {
                ctrl.DataBindings.Add(propertyname, bindsource, columnname, true, DataSourceUpdateMode.OnPropertyChanged);

            }
        }

        public static void FormatGridLforSearchResults(DataGridView grid, string tablename)
        {
            grid.AllowUserToAddRows = false;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DoFormatGrid(grid, tablename);

        }

        public static void FormatGridforEdit(DataGridView grid, string tablename)
        {
            grid.EditMode= DataGridViewEditMode.EditOnEnter;
            DoFormatGrid(grid, tablename);
        }
        private static void DoFormatGrid(DataGridView grid, string tablename)
        {
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            grid.RowHeadersWidth = 25;
            foreach(DataGridViewColumn col in grid.Columns)
            {
                if (col.Name.EndsWith("ID"))
                {
                    col.Visible = false;
                }
            }
            string pkname= tablename+"ID";
            if (grid.Columns.Contains(pkname))
            {
                grid.Columns[pkname].Visible = false;
            }

        }

        public static int GetIDFromGrid(DataGridView grid, int rowindex, string columnname)
        {
            int id = 0;
            if (rowindex < grid.Rows.Count && grid.Columns.Contains(columnname) && grid.Rows[rowindex].Cells[columnname].Value != DBNull.Value)
            {
                if (grid.Rows[rowindex].Cells[columnname].Value is int)
                {
                    id = (int)grid.Rows[rowindex].Cells[columnname].Value;
                }
            }
            return id;
        }

        public static int GetIDFromComboBox(ComboBox lst)
        {
            int value = 0;
            if(lst.SelectedValue != null && lst.SelectedValue is int)
            {
                value = (int)lst.SelectedValue;
            }

            return value;
        }
        public static void AddComboBoxToGrid(DataGridView grid, DataTable datasource, string tablename, string displaymember)
        {
            DataGridViewComboBoxColumn c = new();
            c.DataSource = datasource;
            c.DisplayMember = displaymember;
            c.ValueMember = tablename + "ID";
            c.DataPropertyName = c.ValueMember;
            c.HeaderText = tablename;
            grid.Columns.Insert(0, c);
        }

        public static void AddDeleteButtonToGrid(DataGridView grid, string deletecolname)
        {
            grid.Columns.Add(new DataGridViewButtonColumn() { Text = "X", HeaderText = "Delete", Name = deletecolname, UseColumnTextForButtonValue = true });

        }

        public static bool IsFormOpen(Type formtype, int pkvalue= 0)
        {
            bool exists = false;
            foreach (Form frm in Application.OpenForms)
            {
                int frmpkvalue = 0;
                if (frm.Tag != null && frm.Tag is int)
                {
                    frmpkvalue = (int)frm.Tag;
                }
                if (frm.GetType() == formtype && frmpkvalue== pkvalue)
                {
                    frm.Activate();
                    exists = true;
                    break;
                }
            }
            return exists;
        }
        public static void SetupNav(ToolStrip ts)
        {
            ts.Items.Clear();
            foreach (Form f in Application.OpenForms)
            {
                if (f.IsMdiContainer == false)
                {
                    ToolStripButton btn = new();
                    btn.Text = f.Text;
                    btn.Tag = f;
                    btn.Click += Btn_Click;
                    ts.Items.Add(btn);
                    ts.Items.Add(new ToolStripSeparator());
                }
            }
        }

        public static void Btn_Click(object? sender, EventArgs e)
        {
            if (sender != null && sender is ToolStripButton)
            {
                ToolStripButton btn = (ToolStripButton)sender;
                if (btn.Tag != null && btn.Tag is Form)
                {
                    ((Form)btn.Tag).Activate();
                }
            }
        }
    }
}
