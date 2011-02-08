namespace WillowTree
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.UpdateButton = new System.Windows.Forms.Button();
            this.MainTab = new System.Windows.Forms.TabPage();
            this.GeneralTab = new System.Windows.Forms.TabPage();
            this.WeaponsTab = new System.Windows.Forms.TabPage();
            this.ItemsTab = new System.Windows.Forms.TabPage();
            this.SkillsTab = new System.Windows.Forms.TabPage();
            this.QuestsTab = new System.Windows.Forms.TabPage();
            this.AmmoTab = new System.Windows.Forms.TabPage();
            this.EchosTab = new System.Windows.Forms.TabPage();
            this.WTLTab = new System.Windows.Forms.TabPage();
            this.DebugTab = new System.Windows.Forms.TabPage();
            this.ExportToBackpack = new System.Windows.Forms.Button();
            this.ImportAllFromWeapons = new System.Windows.Forms.Button();
            this.ImportAllFromItems = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.DebugTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.MainTab);
            this.tabControl1.Controls.Add(this.GeneralTab);
            this.tabControl1.Controls.Add(this.WeaponsTab);
            this.tabControl1.Controls.Add(this.ItemsTab);
            this.tabControl1.Controls.Add(this.SkillsTab);
            this.tabControl1.Controls.Add(this.QuestsTab);
            this.tabControl1.Controls.Add(this.AmmoTab);
            this.tabControl1.Controls.Add(this.EchosTab);
            this.tabControl1.Controls.Add(this.WTLTab);
            this.tabControl1.Controls.Add(this.DebugTab);
            this.tabControl1.HotTrack = true;
            this.tabControl1.Location = new System.Drawing.Point(4, 1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(944, 965);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackgroundImage = global::WillowTree.Properties.Resources.background1;
            this.tabPage1.Controls.Add(this.UpdateButton);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(936, 939);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // UpdateButton
            // 
            this.UpdateButton.Location = new System.Drawing.Point(-4, 0);
            this.UpdateButton.Name = "UpdateButton";
            this.UpdateButton.Size = new System.Drawing.Size(944, 29);
            this.UpdateButton.TabIndex = 39;
            this.UpdateButton.TabStop = false;
            this.UpdateButton.Text = "There is a new version of WillowTree# available! Click here to download.";
            this.UpdateButton.UseVisualStyleBackColor = false;
            // 
            // MainTab
            // 
            this.MainTab.Location = new System.Drawing.Point(4, 22);
            this.MainTab.Name = "MainTab";
            this.MainTab.Padding = new System.Windows.Forms.Padding(3);
            this.MainTab.Size = new System.Drawing.Size(936, 939);
            this.MainTab.TabIndex = 1;
            this.MainTab.Text = "Main Page";
            this.MainTab.UseVisualStyleBackColor = true;
            // 
            // GeneralTab
            // 
            this.GeneralTab.Location = new System.Drawing.Point(4, 22);
            this.GeneralTab.Name = "GeneralTab";
            this.GeneralTab.Size = new System.Drawing.Size(936, 939);
            this.GeneralTab.TabIndex = 2;
            this.GeneralTab.Text = "General Info";
            this.GeneralTab.UseVisualStyleBackColor = true;
            // 
            // WeaponsTab
            // 
            this.WeaponsTab.Location = new System.Drawing.Point(4, 22);
            this.WeaponsTab.Name = "WeaponsTab";
            this.WeaponsTab.Size = new System.Drawing.Size(936, 939);
            this.WeaponsTab.TabIndex = 3;
            this.WeaponsTab.Text = "Weapons";
            this.WeaponsTab.UseVisualStyleBackColor = true;
            // 
            // ItemsTab
            // 
            this.ItemsTab.Location = new System.Drawing.Point(4, 22);
            this.ItemsTab.Name = "ItemsTab";
            this.ItemsTab.Size = new System.Drawing.Size(936, 939);
            this.ItemsTab.TabIndex = 4;
            this.ItemsTab.Text = "Items";
            this.ItemsTab.UseVisualStyleBackColor = true;
            // 
            // SkillsTab
            // 
            this.SkillsTab.Location = new System.Drawing.Point(4, 22);
            this.SkillsTab.Name = "SkillsTab";
            this.SkillsTab.Size = new System.Drawing.Size(936, 939);
            this.SkillsTab.TabIndex = 5;
            this.SkillsTab.Text = "Skills";
            this.SkillsTab.UseVisualStyleBackColor = true;
            // 
            // QuestsTab
            // 
            this.QuestsTab.Location = new System.Drawing.Point(4, 22);
            this.QuestsTab.Name = "QuestsTab";
            this.QuestsTab.Size = new System.Drawing.Size(936, 939);
            this.QuestsTab.TabIndex = 6;
            this.QuestsTab.Text = "Quests";
            this.QuestsTab.UseVisualStyleBackColor = true;
            // 
            // AmmoTab
            // 
            this.AmmoTab.Location = new System.Drawing.Point(4, 22);
            this.AmmoTab.Name = "AmmoTab";
            this.AmmoTab.Size = new System.Drawing.Size(936, 939);
            this.AmmoTab.TabIndex = 7;
            this.AmmoTab.Text = "Ammo Pools";
            this.AmmoTab.UseVisualStyleBackColor = true;
            // 
            // EchosTab
            // 
            this.EchosTab.Location = new System.Drawing.Point(4, 22);
            this.EchosTab.Name = "EchosTab";
            this.EchosTab.Size = new System.Drawing.Size(936, 939);
            this.EchosTab.TabIndex = 8;
            this.EchosTab.Text = "Echo Logs";
            this.EchosTab.UseVisualStyleBackColor = true;
            // 
            // WTLTab
            // 
            this.WTLTab.Location = new System.Drawing.Point(4, 22);
            this.WTLTab.Name = "WTLTab";
            this.WTLTab.Size = new System.Drawing.Size(936, 939);
            this.WTLTab.TabIndex = 9;
            this.WTLTab.Text = "WillowTree Locker";
            this.WTLTab.UseVisualStyleBackColor = true;
            // 
            // DebugTab
            // 
            this.DebugTab.Controls.Add(this.ImportAllFromItems);
            this.DebugTab.Controls.Add(this.ImportAllFromWeapons);
            this.DebugTab.Controls.Add(this.ExportToBackpack);
            this.DebugTab.Location = new System.Drawing.Point(4, 22);
            this.DebugTab.Name = "DebugTab";
            this.DebugTab.Size = new System.Drawing.Size(936, 939);
            this.DebugTab.TabIndex = 10;
            this.DebugTab.Text = "Debug";
            this.DebugTab.UseVisualStyleBackColor = true;
            // 
            // ExportToBackpack
            // 
            this.ExportToBackpack.Location = new System.Drawing.Point(20, 15);
            this.ExportToBackpack.Name = "ExportToBackpack";
            this.ExportToBackpack.Size = new System.Drawing.Size(75, 23);
            this.ExportToBackpack.TabIndex = 0;
            this.ExportToBackpack.Text = "button1";
            this.ExportToBackpack.UseVisualStyleBackColor = true;
            // 
            // ImportAllFromWeapons
            // 
            this.ImportAllFromWeapons.Location = new System.Drawing.Point(20, 73);
            this.ImportAllFromWeapons.Name = "ImportAllFromWeapons";
            this.ImportAllFromWeapons.Size = new System.Drawing.Size(75, 23);
            this.ImportAllFromWeapons.TabIndex = 1;
            this.ImportAllFromWeapons.Text = "button2";
            this.ImportAllFromWeapons.UseVisualStyleBackColor = true;
            // 
            // ImportAllFromItems
            // 
            this.ImportAllFromItems.Location = new System.Drawing.Point(20, 44);
            this.ImportAllFromItems.Name = "ImportAllFromItems";
            this.ImportAllFromItems.Size = new System.Drawing.Size(75, 23);
            this.ImportAllFromItems.TabIndex = 2;
            this.ImportAllFromItems.Text = "button3";
            this.ImportAllFromItems.UseVisualStyleBackColor = true;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::WillowTree.Properties.Resources.background1;
            this.ClientSize = new System.Drawing.Size(944, 646);
            this.Controls.Add(this.tabControl1);
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.DebugTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage MainTab;
        private System.Windows.Forms.TabPage GeneralTab;
        private System.Windows.Forms.TabPage WeaponsTab;
        private System.Windows.Forms.TabPage ItemsTab;
        private System.Windows.Forms.TabPage SkillsTab;
        private System.Windows.Forms.TabPage QuestsTab;
        private System.Windows.Forms.TabPage AmmoTab;
        private System.Windows.Forms.TabPage EchosTab;
        private System.Windows.Forms.TabPage WTLTab;
        private System.Windows.Forms.Button UpdateButton;
        private System.Windows.Forms.TabPage DebugTab;
        private System.Windows.Forms.Button ImportAllFromItems;
        private System.Windows.Forms.Button ImportAllFromWeapons;
        private System.Windows.Forms.Button ExportToBackpack;
    }
}