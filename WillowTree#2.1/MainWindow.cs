using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace WillowTree
{
    public partial class MainWindow : Form
    {
        private string AppDir = Path.GetDirectoryName(Application.ExecutablePath);

        public MainWindow()
        {
            InitializeComponent();

            UpdateButton.Hide();

            MainTab.Select();
            if (System.IO.Directory.Exists(AppDir + "\\Data") == false)
            {
                MessageBox.Show("Couldn't find the 'Data' folder! Please make sure that WillowTree# and the 'Data' folder are in the same directory.");
                Application.Exit();
            }

            tabPage1.Enabled = false;
            GeneralTab.Enabled = false;
            SkillsTab.Enabled = false;
            AmmoTab.Enabled = false;
            ItemsTab.Enabled = false;
            WeaponsTab.Enabled = false;
            QuestsTab.Enabled = false;
            EchosTab.Enabled = false;
            WTLTab.Enabled = false;
            ExportToBackpack.Enabled = false;
            ImportAllFromItems.Enabled = false;
            ImportAllFromWeapons.Enabled = false;

            if (System.Diagnostics.Debugger.IsAttached == true) DebugTab.Enabled = true;
            else
            {
                DebugTab.Enabled = false;
                DebugTab.Visible = false;
            }
        }
    }
}
