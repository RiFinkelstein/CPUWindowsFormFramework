﻿using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CPUWindowsFormFramework
{
    public class WindowsFormUtility
    {
        public static void SetListBinding(ComboBox lst, DataTable sourceDT, DataTable? targetDT, string tablename)
        {
            lst.DataSource = sourceDT;
            lst.ValueMember = tablename + "ID";
            lst.DisplayMember = lst.Name.Substring(3);
            lst.DataBindings.Clear();

            if (targetDT != null)
            {
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
                case "chb":
                    propertyname = "checked";
                    break;

            }
            if (!string.IsNullOrWhiteSpace(propertyname) && !string.IsNullOrWhiteSpace(columnname))
            {
                if (controltype == "chb" && ctrl is CheckBox)
                {
                    // Bind CheckBox.Checked to an int field (0 or 1)
                    Binding binding = new Binding(propertyname, bindsource, columnname, true, DataSourceUpdateMode.OnPropertyChanged);
                    binding.Format += (sender, e) =>
                    {
                        if (e.Value is int intValue)
                        {
                            e.Value = intValue == 1; // Convert int to bool
                        }
                    };
                    binding.Parse += (sender, e) =>
                    {
                        if (e.Value is bool boolValue)
                        {
                            e.Value = boolValue ? 1 : 0; // Convert bool to int
                        }
                    };
                    ctrl.DataBindings.Add(binding);
                }
                else
                {
                    ctrl.DataBindings.Add(propertyname, bindsource, columnname, true, DataSourceUpdateMode.OnPropertyChanged);
                }
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
            grid.EditMode = DataGridViewEditMode.EditOnEnter;
            DoFormatGrid(grid, tablename);
        }
        private static void DoFormatGrid(DataGridView grid, string tablename)
        {
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            grid.RowHeadersWidth = 25;
            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (col.Name.ToUpper().EndsWith("ID"))
                {
                    col.Visible = false;
                }
            }
            string pkname = tablename + "ID";
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
            if (lst.SelectedValue != null && lst.SelectedValue is int)
            {
                value = (int)lst.SelectedValue;
            }

            return value;
        }
        public static void AddComboBoxToGrid(DataGridView grid, DataTable datasource, string tablename, string displayMember)
        {
            DataGridViewComboBoxColumn c = new();
            c.DataSource = datasource;
            c.DisplayMember = displayMember;
            c.ValueMember = tablename + "id";
            c.DataPropertyName = c.ValueMember;
            c.HeaderText = tablename;
            c.ReadOnly = false;
            grid.AllowDrop = true;
            grid.Columns.Insert(0, c);
            grid.Columns[0].ReadOnly = false;

        }

        public static void AddDeleteButtonToGrid(DataGridView grid, string deleteColName, int displayIndex = -1)
        {
            // Check if the column already exists
            if (grid.Columns.Contains(deleteColName))
                return;

            // Add the delete button column
            DataGridViewButtonColumn deleteButton = new()
            {
                Name = deleteColName,
                HeaderText = "Delete",
                Text = "X",
                UseColumnTextForButtonValue = true
            };

            grid.Columns.Add(deleteButton);

            // Set the column's display index if specified
            if (displayIndex >= 0 && displayIndex < grid.Columns.Count)
            {
                deleteButton.DisplayIndex = displayIndex;
            }
        }


        public static bool IsFormOpen(Type formtype, int pkvalue = 0)
        {
            bool exists = false;
            foreach (Form frm in Application.OpenForms)
            {
                int frmpkvalue = 0;
                if (frm.Tag != null && frm.Tag is int)
                {
                    frmpkvalue = (int)frm.Tag;
                }
                if (frm.GetType() == formtype && frmpkvalue == pkvalue)
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

        public static void EnforceNumericInput(TextBox textBox)
        {
            textBox.KeyPress += (sender, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true; // Prevent the character from being entered

                    // Show a friendly error message
                    MessageBox.Show("Please enter only numeric values.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
        }

        public static void EnforceNumericInputInGrid(DataGridView grid, params string[] numericColumnNames)
        {
            grid.EditingControlShowing -= (sender, e) => Grid_EditingControlShowing(sender, e, grid, numericColumnNames);
            grid.EditingControlShowing += (sender, e) => Grid_EditingControlShowing(sender, e, grid, numericColumnNames);
        }

        private static void Grid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e, DataGridView grid, string[] numericColumnNames)
        {
            if (grid.CurrentCell == null || !numericColumnNames.Contains(grid.Columns[grid.CurrentCell.ColumnIndex].Name))
                return;

            if (e.Control is TextBox tb)
            {
                tb.KeyPress -= NumericColumn_KeyPress; // Remove any existing handler
                tb.KeyPress += NumericColumn_KeyPress; // Attach handler
            }
        }

        private static void NumericColumn_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }




    }
}
