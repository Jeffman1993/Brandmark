using MovablePython;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Brandmark
{
    public partial class Form1 : Form
    {

        private Hotkey hk;
        private List<Bookmark> bookmarks = null;
        private bool isShown = false;
        public static Form1 form;

        public Form1()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            InitializeComponent();
            dGrid.RowHeadersVisible = false;
            dGrid.LostFocus += delegate { this.Hide(); isShown = false; };

            //bookmarks = Library.getChromeBookmarks();
            bookmarks = Library.getFireBookmarks();

            popGrid();

            hk = new Hotkey();
            hk.KeyCode = Keys.F2;
            hk.Windows = false;
            hk.Pressed += getHotKey;

            if (!hk.GetCanRegister(this))
            { Console.WriteLine("Whoops, looks like attempts to register will fail or throw an exception, show an error / visual user feedback"); }
            else
            { hk.Register(this); }

            this.Opacity = 0;
            form = this;
            //button1.Text = "\u269B";
        }

        private void getHotKey(object sender, HandledEventArgs e)
        {
            if (isShown)
            {
                this.Hide();
                dGrid.ClearSelection();
                isShown = false;
            }
            else
            {
                moveToCursor();
                this.Show();
                this.Activate();
                dGrid.Focus();
                isShown = true;
            }
        }

        private void popGrid()
        {
            foreach(Bookmark bookmark in bookmarks)
            {
                int rowId = dGrid.Rows.Add(bookmark.icon, bookmark.name, bookmark.url);
                dGrid.Rows[rowId].Cells[0].ToolTipText = bookmark.name;
                dGrid.Rows[rowId].Cells[1].ToolTipText = bookmark.name;
            }

            dGrid.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dGrid.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            LinkUtility.openLink((string)dGrid.Rows[e.RowIndex].Cells[2].Value);
            this.Hide();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (hk.Registered)
            { hk.Unregister(); }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.Hide();
            isShown = false;
            this.Opacity = 100;
            dGrid.ClearSelection();
        }

        private static void moveToCursor()
        {
            form.Location = MousePosition;
        }
    }
}
