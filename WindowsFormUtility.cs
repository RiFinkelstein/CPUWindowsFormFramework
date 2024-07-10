using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUWindowsFormFramework
{
    public class WindowsFormUtility
    {
        public static void SetListBinding(ComboBox lst, DataTable sourceDT, DataTable targetDT string tablename)
        {
            lst.DataSource = sourceDT;
            lst.ValueMember = tablename + "ID";
            lst.DisplayMember = lst.Name.Substring(3);
            lst.DataBindings.Add("selectedValue", targetDT, lst.ValueMember, false, DataSourceUpdateMode.OnPropertyChanged);
        }

        public static void SetControlBinding(Control ctrl, DataTable dt)
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
                ctrl.DataBindings.Add(propertyname, dt, columnname, true, DataSourceUpdateMode.OnPropertyChanged);

            }
        }

        public static void FormatGridLforSearchResults(DataGridView grid)
        {
            grid.AllowUserToAddRows = false;
            grid.ReadOnly = true;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
           grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }
    }
}
