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
        public static void SetListBinding(ComboBox lst, DataTable sourceDT, DataTable targetDT, string tablename)
        {
            lst.DataSource = sourceDT;
            lst.ValueMember = tablename + "ID";
            lst.DisplayMember = lst.Name.Substring(3);
            lst.DataBindings.Add("selectedValue", targetDT, lst.ValueMember, false, DataSourceUpdateMode.OnPropertyChanged);
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

        public static void FormatGridLforSearchResults(DataGridView grid)
        {
            grid.AllowUserToAddRows = false;
            grid.ReadOnly = true;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
           grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
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
