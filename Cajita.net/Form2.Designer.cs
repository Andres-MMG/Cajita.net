﻿namespace Cajita.net
{
    partial class Form2
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
            components = new System.ComponentModel.Container();
            TabControl1 = new TabControl();
            cntxtMenuStrip = new ContextMenuStrip(components);
            copiarToolStripMenuItem = new ToolStripMenuItem();
            pegarToolStripMenuItem = new ToolStripMenuItem();
            moverToolStripMenuItem = new ToolStripMenuItem();
            separadorToolStripMenuItem = new ToolStripSeparator();
            salirToolStripMenuItem = new ToolStripMenuItem();
            timer1 = new System.Windows.Forms.Timer(components);
            btnAddTabPage = new Button();
            cntxtMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // TabControl1
            // 
            TabControl1.ContextMenuStrip = cntxtMenuStrip;
            TabControl1.Dock = DockStyle.Fill;
            TabControl1.Location = new Point(0, 0);
            TabControl1.Name = "TabControl1";
            TabControl1.SelectedIndex = 0;
            TabControl1.Size = new Size(800, 450);
            TabControl1.TabIndex = 0;
            // 
            // cntxtMenuStrip
            // 
            cntxtMenuStrip.Items.AddRange(new ToolStripItem[] { copiarToolStripMenuItem, pegarToolStripMenuItem, moverToolStripMenuItem, separadorToolStripMenuItem, salirToolStripMenuItem });
            cntxtMenuStrip.Name = "cntxtMenuStrip";
            cntxtMenuStrip.Size = new Size(110, 98);
            // 
            // copiarToolStripMenuItem
            // 
            copiarToolStripMenuItem.Name = "copiarToolStripMenuItem";
            copiarToolStripMenuItem.Size = new Size(109, 22);
            copiarToolStripMenuItem.Text = "Copiar";
            // 
            // pegarToolStripMenuItem
            // 
            pegarToolStripMenuItem.Name = "pegarToolStripMenuItem";
            pegarToolStripMenuItem.Size = new Size(109, 22);
            pegarToolStripMenuItem.Text = "Pegar";
            // 
            // moverToolStripMenuItem
            // 
            moverToolStripMenuItem.Name = "moverToolStripMenuItem";
            moverToolStripMenuItem.Size = new Size(109, 22);
            moverToolStripMenuItem.Text = "Mover";
            // 
            // separadorToolStripMenuItem
            // 
            separadorToolStripMenuItem.Name = "separadorToolStripMenuItem";
            separadorToolStripMenuItem.Size = new Size(106, 6);
            // 
            // salirToolStripMenuItem
            // 
            salirToolStripMenuItem.Name = "salirToolStripMenuItem";
            salirToolStripMenuItem.Size = new Size(109, 22);
            salirToolStripMenuItem.Text = "Salir";
            // 
            // btnAddTabPage
            // 
            btnAddTabPage.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAddTabPage.Location = new Point(772, 0);
            btnAddTabPage.Name = "btnAddTabPage";
            btnAddTabPage.Size = new Size(25, 25);
            btnAddTabPage.TabIndex = 1;
            btnAddTabPage.Text = "+";
            btnAddTabPage.UseVisualStyleBackColor = true;
            btnAddTabPage.Click += btnAddTabPage_Click;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            ContextMenuStrip = cntxtMenuStrip;
            Controls.Add(btnAddTabPage);
            Controls.Add(TabControl1);
            Name = "Form2";
            Text = "Form2";
            TopMost = true;
            FormClosing += Form2_FormClosing;
            Load += Form2_Load;
            cntxtMenuStrip.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabControl TabControl1;
        private System.Windows.Forms.Timer timer1;
        private ContextMenuStrip cntxtMenuStrip;
        private ToolStripMenuItem copiarToolStripMenuItem;
        private ToolStripMenuItem pegarToolStripMenuItem;
        private ToolStripMenuItem moverToolStripMenuItem;
        private ToolStripSeparator separadorToolStripMenuItem;
        private ToolStripMenuItem salirToolStripMenuItem;
        private Button btnAddTabPage;
    }
}