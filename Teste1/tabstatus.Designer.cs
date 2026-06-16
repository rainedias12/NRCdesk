namespace Teste1
{
    partial class tabstatus
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(tabstatus));
            this.BtnTodosOsTickets = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.dataGridView3 = new System.Windows.Forms.DataGridView();
            this.TxtFiltrarPor = new System.Windows.Forms.TextBox();
            this.LblFiltrarPor = new System.Windows.Forms.Label();
            this.BtnBuscar = new System.Windows.Forms.Button();
            this.BtnCriarTicket = new System.Windows.Forms.Button();
            this.PanelPrincipal = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.BtnRelatorio2 = new System.Windows.Forms.Button();
            this.BtnSair2 = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).BeginInit();
            this.PanelPrincipal.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnTodosOsTickets
            // 
            this.BtnTodosOsTickets.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(74)))));
            this.BtnTodosOsTickets.ForeColor = System.Drawing.Color.White;
            this.BtnTodosOsTickets.Location = new System.Drawing.Point(189, 102);
            this.BtnTodosOsTickets.Name = "BtnTodosOsTickets";
            this.BtnTodosOsTickets.Size = new System.Drawing.Size(112, 49);
            this.BtnTodosOsTickets.TabIndex = 37;
            this.BtnTodosOsTickets.Text = "Todos os Tickets";
            this.BtnTodosOsTickets.UseVisualStyleBackColor = false;
            this.BtnTodosOsTickets.Click += new System.EventHandler(this.BtnTodosOsTickets_Click_1);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 707);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1008, 22);
            this.statusStrip1.TabIndex = 36;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // dataGridView3
            // 
            this.dataGridView3.AllowUserToAddRows = false;
            this.dataGridView3.AllowUserToDeleteRows = false;
            this.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView3.Location = new System.Drawing.Point(189, 157);
            this.dataGridView3.Name = "dataGridView3";
            this.dataGridView3.ReadOnly = true;
            this.dataGridView3.Size = new System.Drawing.Size(706, 481);
            this.dataGridView3.TabIndex = 33;
            // 
            // TxtFiltrarPor
            // 
            this.TxtFiltrarPor.Location = new System.Drawing.Point(521, 128);
            this.TxtFiltrarPor.MaxLength = 100;
            this.TxtFiltrarPor.Name = "TxtFiltrarPor";
            this.TxtFiltrarPor.Size = new System.Drawing.Size(245, 20);
            this.TxtFiltrarPor.TabIndex = 38;
            // 
            // LblFiltrarPor
            // 
            this.LblFiltrarPor.AutoSize = true;
            this.LblFiltrarPor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(74)))));
            this.LblFiltrarPor.Location = new System.Drawing.Point(462, 131);
            this.LblFiltrarPor.Name = "LblFiltrarPor";
            this.LblFiltrarPor.Size = new System.Drawing.Size(53, 13);
            this.LblFiltrarPor.TabIndex = 39;
            this.LblFiltrarPor.Text = "Filtrar por:";
            // 
            // BtnBuscar
            // 
            this.BtnBuscar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(74)))));
            this.BtnBuscar.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.BtnBuscar.Location = new System.Drawing.Point(772, 121);
            this.BtnBuscar.Name = "BtnBuscar";
            this.BtnBuscar.Size = new System.Drawing.Size(98, 33);
            this.BtnBuscar.TabIndex = 40;
            this.BtnBuscar.Text = "Buscar";
            this.BtnBuscar.UseVisualStyleBackColor = false;
            this.BtnBuscar.Click += new System.EventHandler(this.BtnBuscar_Click);
            // 
            // BtnCriarTicket
            // 
            this.BtnCriarTicket.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(74)))));
            this.BtnCriarTicket.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnCriarTicket.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.BtnCriarTicket.Location = new System.Drawing.Point(-3, 90);
            this.BtnCriarTicket.Name = "BtnCriarTicket";
            this.BtnCriarTicket.Size = new System.Drawing.Size(153, 52);
            this.BtnCriarTicket.TabIndex = 41;
            this.BtnCriarTicket.Text = "Criar Ticket";
            this.BtnCriarTicket.UseVisualStyleBackColor = false;
            this.BtnCriarTicket.Click += new System.EventHandler(this.BtnCriarTicket_Click);
            // 
            // PanelPrincipal
            // 
            this.PanelPrincipal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(74)))));
            this.PanelPrincipal.Controls.Add(this.BtnSair2);
            this.PanelPrincipal.Controls.Add(this.BtnRelatorio2);
            this.PanelPrincipal.Controls.Add(this.BtnCriarTicket);
            this.PanelPrincipal.Controls.Add(this.panel1);
            this.PanelPrincipal.Dock = System.Windows.Forms.DockStyle.Left;
            this.PanelPrincipal.Location = new System.Drawing.Point(0, 0);
            this.PanelPrincipal.Name = "PanelPrincipal";
            this.PanelPrincipal.Size = new System.Drawing.Size(150, 707);
            this.PanelPrincipal.TabIndex = 54;
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(-23, -99);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 100);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(150, 87);
            this.panel2.TabIndex = 54;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Teste1.Properties.Resources.NRCdesk__1_;
            this.pictureBox1.Location = new System.Drawing.Point(0, 11);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(150, 73);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 34;
            this.pictureBox1.TabStop = false;
//            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // BtnRelatorio2
            // 
            this.BtnRelatorio2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(74)))));
            this.BtnRelatorio2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnRelatorio2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.BtnRelatorio2.Location = new System.Drawing.Point(-3, 157);
            this.BtnRelatorio2.Name = "BtnRelatorio2";
            this.BtnRelatorio2.Size = new System.Drawing.Size(153, 53);
            this.BtnRelatorio2.TabIndex = 55;
            this.BtnRelatorio2.Text = "Relatótio";
            this.BtnRelatorio2.UseVisualStyleBackColor = false;
            this.BtnRelatorio2.Click += new System.EventHandler(this.BtnRelatorio2_Click);
            // 
            // BtnSair2
            // 
            this.BtnSair2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(74)))));
            this.BtnSair2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnSair2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.BtnSair2.Location = new System.Drawing.Point(-3, 627);
            this.BtnSair2.Name = "BtnSair2";
            this.BtnSair2.Size = new System.Drawing.Size(153, 39);
            this.BtnSair2.TabIndex = 55;
            this.BtnSair2.Text = "Sair";
            this.BtnSair2.UseVisualStyleBackColor = false;
            this.BtnSair2.Click += new System.EventHandler(this.BtnSair2_Click);
            // 
            // tabstatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.PanelPrincipal);
            this.Controls.Add(this.BtnBuscar);
            this.Controls.Add(this.LblFiltrarPor);
            this.Controls.Add(this.TxtFiltrarPor);
            this.Controls.Add(this.BtnTodosOsTickets);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.dataGridView3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "tabstatus";
            this.Text = "tabstatus";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).EndInit();
            this.PanelPrincipal.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnTodosOsTickets;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.DataGridView dataGridView3;
        private System.Windows.Forms.TextBox TxtFiltrarPor;
        private System.Windows.Forms.Label LblFiltrarPor;
        private System.Windows.Forms.Button BtnBuscar;
        private System.Windows.Forms.Button BtnCriarTicket;
        private System.Windows.Forms.Panel PanelPrincipal;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button BtnRelatorio2;
        private System.Windows.Forms.Button BtnSair2;
        //    private System.Windows.Forms.Panel panelForm;
    }
}