namespace ClassGeneratorSDE
{
    partial class FrmClassGenerator
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
            this.txtBoxDB = new System.Windows.Forms.TextBox();
            this.txtBoxInstancia = new System.Windows.Forms.TextBox();
            this.txtBoxUsuario = new System.Windows.Forms.TextBox();
            this.txtBoxPassword = new System.Windows.Forms.TextBox();
            this.lbDb = new System.Windows.Forms.Label();
            this.lvInstancia = new System.Windows.Forms.Label();
            this.lbPassword = new System.Windows.Forms.Label();
            this.lbUsuario = new System.Windows.Forms.Label();
            this.btConnectar = new System.Windows.Forms.Button();
            this.treeViewDatasets = new System.Windows.Forms.TreeView();
            this.btGerar = new System.Windows.Forms.Button();
            this.txtBoxCaminho = new System.Windows.Forms.TextBox();
            this.btPesquisar = new System.Windows.Forms.Button();
            this.lstBoxArquivos = new System.Windows.Forms.ListBox();
            this.lbServer = new System.Windows.Forms.Label();
            this.txtBoxServer = new System.Windows.Forms.TextBox();
            this.lbVersao = new System.Windows.Forms.Label();
            this.txtBoxVersao = new System.Windows.Forms.TextBox();
            this.chkBoxFeatueClass = new System.Windows.Forms.CheckBox();
            this.chkBoxTables = new System.Windows.Forms.CheckBox();
            this.treeViewTables = new System.Windows.Forms.TreeView();
            this.chkBoxMarcarTodosFC = new System.Windows.Forms.CheckBox();
            this.chkBoxMarcarTodosTable = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pctBoxFC = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctBoxFC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // txtBoxDB
            // 
            this.txtBoxDB.Location = new System.Drawing.Point(71, 64);
            this.txtBoxDB.Name = "txtBoxDB";
            this.txtBoxDB.Size = new System.Drawing.Size(109, 20);
            this.txtBoxDB.TabIndex = 0;
            // 
            // txtBoxInstancia
            // 
            this.txtBoxInstancia.Location = new System.Drawing.Point(71, 38);
            this.txtBoxInstancia.Name = "txtBoxInstancia";
            this.txtBoxInstancia.Size = new System.Drawing.Size(109, 20);
            this.txtBoxInstancia.TabIndex = 1;
            this.txtBoxInstancia.Text = "5858";
            // 
            // txtBoxUsuario
            // 
            this.txtBoxUsuario.Location = new System.Drawing.Point(244, 12);
            this.txtBoxUsuario.Name = "txtBoxUsuario";
            this.txtBoxUsuario.Size = new System.Drawing.Size(110, 20);
            this.txtBoxUsuario.TabIndex = 2;
            this.txtBoxUsuario.Text = "neosde";
            // 
            // txtBoxPassword
            // 
            this.txtBoxPassword.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.txtBoxPassword.Location = new System.Drawing.Point(244, 39);
            this.txtBoxPassword.Name = "txtBoxPassword";
            this.txtBoxPassword.PasswordChar = 'l';
            this.txtBoxPassword.Size = new System.Drawing.Size(110, 20);
            this.txtBoxPassword.TabIndex = 3;
            this.txtBoxPassword.Text = "neosde";
            // 
            // lbDb
            // 
            this.lbDb.AutoSize = true;
            this.lbDb.Location = new System.Drawing.Point(13, 71);
            this.lbDb.Name = "lbDb";
            this.lbDb.Size = new System.Drawing.Size(19, 13);
            this.lbDb.TabIndex = 4;
            this.lbDb.Text = "db";
            // 
            // lvInstancia
            // 
            this.lvInstancia.AutoSize = true;
            this.lvInstancia.Location = new System.Drawing.Point(13, 41);
            this.lvInstancia.Name = "lvInstancia";
            this.lvInstancia.Size = new System.Drawing.Size(47, 13);
            this.lvInstancia.TabIndex = 5;
            this.lvInstancia.Text = "instance";
            // 
            // lbPassword
            // 
            this.lbPassword.AutoSize = true;
            this.lbPassword.Location = new System.Drawing.Point(186, 42);
            this.lbPassword.Name = "lbPassword";
            this.lbPassword.Size = new System.Drawing.Size(52, 13);
            this.lbPassword.TabIndex = 7;
            this.lbPassword.Text = "password";
            // 
            // lbUsuario
            // 
            this.lbUsuario.AutoSize = true;
            this.lbUsuario.Location = new System.Drawing.Point(186, 19);
            this.lbUsuario.Name = "lbUsuario";
            this.lbUsuario.Size = new System.Drawing.Size(27, 13);
            this.lbUsuario.TabIndex = 6;
            this.lbUsuario.Text = "user";
            // 
            // btConnectar
            // 
            this.btConnectar.Location = new System.Drawing.Point(401, 110);
            this.btConnectar.Name = "btConnectar";
            this.btConnectar.Size = new System.Drawing.Size(75, 23);
            this.btConnectar.TabIndex = 8;
            this.btConnectar.Text = "Connect";
            this.btConnectar.UseVisualStyleBackColor = true;
            this.btConnectar.Click += new System.EventHandler(this.BtConnectarClick);
            // 
            // treeViewDatasets
            // 
            this.treeViewDatasets.CheckBoxes = true;
            this.treeViewDatasets.Location = new System.Drawing.Point(16, 161);
            this.treeViewDatasets.Name = "treeViewDatasets";
            this.treeViewDatasets.Size = new System.Drawing.Size(264, 130);
            this.treeViewDatasets.TabIndex = 9;
            // 
            // btGerar
            // 
            this.btGerar.Location = new System.Drawing.Point(482, 109);
            this.btGerar.Name = "btGerar";
            this.btGerar.Size = new System.Drawing.Size(75, 23);
            this.btGerar.TabIndex = 10;
            this.btGerar.Text = "Generate";
            this.btGerar.UseVisualStyleBackColor = true;
            this.btGerar.Click += new System.EventHandler(this.BtGerarClick);
            // 
            // txtBoxCaminho
            // 
            this.txtBoxCaminho.Location = new System.Drawing.Point(16, 112);
            this.txtBoxCaminho.Name = "txtBoxCaminho";
            this.txtBoxCaminho.Size = new System.Drawing.Size(338, 20);
            this.txtBoxCaminho.TabIndex = 12;
            this.txtBoxCaminho.Text = "C:\\Extracao";
            // 
            // btPesquisar
            // 
            this.btPesquisar.Location = new System.Drawing.Point(360, 109);
            this.btPesquisar.Name = "btPesquisar";
            this.btPesquisar.Size = new System.Drawing.Size(24, 23);
            this.btPesquisar.TabIndex = 13;
            this.btPesquisar.Text = "...";
            this.btPesquisar.UseVisualStyleBackColor = true;
            // 
            // lstBoxArquivos
            // 
            this.lstBoxArquivos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstBoxArquivos.FormattingEnabled = true;
            this.lstBoxArquivos.Location = new System.Drawing.Point(16, 297);
            this.lstBoxArquivos.Name = "lstBoxArquivos";
            this.lstBoxArquivos.Size = new System.Drawing.Size(541, 108);
            this.lstBoxArquivos.TabIndex = 14;
            // 
            // lbServer
            // 
            this.lbServer.AutoSize = true;
            this.lbServer.Location = new System.Drawing.Point(13, 19);
            this.lbServer.Name = "lbServer";
            this.lbServer.Size = new System.Drawing.Size(36, 13);
            this.lbServer.TabIndex = 16;
            this.lbServer.Text = "server";
            // 
            // txtBoxServer
            // 
            this.txtBoxServer.Location = new System.Drawing.Point(71, 12);
            this.txtBoxServer.Name = "txtBoxServer";
            this.txtBoxServer.Size = new System.Drawing.Size(109, 20);
            this.txtBoxServer.TabIndex = 15;
            this.txtBoxServer.Text = "192.213.100.205";
            // 
            // lbVersao
            // 
            this.lbVersao.AutoSize = true;
            this.lbVersao.Location = new System.Drawing.Point(186, 72);
            this.lbVersao.Name = "lbVersao";
            this.lbVersao.Size = new System.Drawing.Size(41, 13);
            this.lbVersao.TabIndex = 18;
            this.lbVersao.Text = "version";
            // 
            // txtBoxVersao
            // 
            this.txtBoxVersao.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxVersao.Location = new System.Drawing.Point(244, 69);
            this.txtBoxVersao.Name = "txtBoxVersao";
            this.txtBoxVersao.Size = new System.Drawing.Size(110, 20);
            this.txtBoxVersao.TabIndex = 17;
            this.txtBoxVersao.Text = "SDE.DEFAULT";
            // 
            // chkBoxFeatueClass
            // 
            this.chkBoxFeatueClass.AutoSize = true;
            this.chkBoxFeatueClass.Checked = true;
            this.chkBoxFeatueClass.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBoxFeatueClass.Location = new System.Drawing.Point(18, 92);
            this.chkBoxFeatueClass.Name = "chkBoxFeatueClass";
            this.chkBoxFeatueClass.Size = new System.Drawing.Size(101, 17);
            this.chkBoxFeatueClass.TabIndex = 19;
            this.chkBoxFeatueClass.Text = "Feature Classes";
            this.chkBoxFeatueClass.UseVisualStyleBackColor = true;
            // 
            // chkBoxTables
            // 
            this.chkBoxTables.AutoSize = true;
            this.chkBoxTables.Checked = true;
            this.chkBoxTables.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBoxTables.Location = new System.Drawing.Point(125, 92);
            this.chkBoxTables.Name = "chkBoxTables";
            this.chkBoxTables.Size = new System.Drawing.Size(58, 17);
            this.chkBoxTables.TabIndex = 20;
            this.chkBoxTables.Text = "Tables";
            this.chkBoxTables.UseVisualStyleBackColor = true;
            // 
            // treeViewTables
            // 
            this.treeViewTables.CheckBoxes = true;
            this.treeViewTables.Location = new System.Drawing.Point(286, 161);
            this.treeViewTables.Name = "treeViewTables";
            this.treeViewTables.Size = new System.Drawing.Size(271, 130);
            this.treeViewTables.TabIndex = 21;
            // 
            // chkBoxMarcarTodosFC
            // 
            this.chkBoxMarcarTodosFC.AutoSize = true;
            this.chkBoxMarcarTodosFC.Location = new System.Drawing.Point(209, 142);
            this.chkBoxMarcarTodosFC.Name = "chkBoxMarcarTodosFC";
            this.chkBoxMarcarTodosFC.Size = new System.Drawing.Size(71, 17);
            this.chkBoxMarcarTodosFC.TabIndex = 22;
            this.chkBoxMarcarTodosFC.Text = "Check All";
            this.chkBoxMarcarTodosFC.UseVisualStyleBackColor = true;
            this.chkBoxMarcarTodosFC.CheckedChanged += new System.EventHandler(this.ChkBoxMarcarTodosFcCheckedChanged);
            // 
            // chkBoxMarcarTodosTable
            // 
            this.chkBoxMarcarTodosTable.AutoSize = true;
            this.chkBoxMarcarTodosTable.Location = new System.Drawing.Point(486, 142);
            this.chkBoxMarcarTodosTable.Name = "chkBoxMarcarTodosTable";
            this.chkBoxMarcarTodosTable.Size = new System.Drawing.Size(71, 17);
            this.chkBoxMarcarTodosTable.TabIndex = 23;
            this.chkBoxMarcarTodosTable.Text = "Check All";
            this.chkBoxMarcarTodosTable.UseVisualStyleBackColor = true;
            this.chkBoxMarcarTodosTable.CheckedChanged += new System.EventHandler(this.ChkBoxMarcarTodosTableCheckedChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ClassGeneratorSDE.Properties.Resources.geodatabaseTable;
            this.pictureBox1.Location = new System.Drawing.Point(331, 140);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(23, 19);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 25;
            this.pictureBox1.TabStop = false;
            // 
            // pctBoxFC
            // 
            this.pctBoxFC.Image = global::ClassGeneratorSDE.Properties.Resources.featureDatasets;
            this.pctBoxFC.Location = new System.Drawing.Point(18, 138);
            this.pctBoxFC.Name = "pctBoxFC";
            this.pctBoxFC.Size = new System.Drawing.Size(24, 21);
            this.pctBoxFC.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pctBoxFC.TabIndex = 24;
            this.pctBoxFC.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::ClassGeneratorSDE.Properties.Resources.layers;
            this.pictureBox2.InitialImage = global::ClassGeneratorSDE.Properties.Resources.layers;
            this.pictureBox2.Location = new System.Drawing.Point(372, 12);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(185, 97);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 26;
            this.pictureBox2.TabStop = false;
            // 
            // FrmClassGenerator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(564, 406);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pctBoxFC);
            this.Controls.Add(this.chkBoxMarcarTodosTable);
            this.Controls.Add(this.chkBoxMarcarTodosFC);
            this.Controls.Add(this.treeViewTables);
            this.Controls.Add(this.chkBoxTables);
            this.Controls.Add(this.chkBoxFeatueClass);
            this.Controls.Add(this.lbVersao);
            this.Controls.Add(this.txtBoxVersao);
            this.Controls.Add(this.lbServer);
            this.Controls.Add(this.txtBoxServer);
            this.Controls.Add(this.lstBoxArquivos);
            this.Controls.Add(this.btPesquisar);
            this.Controls.Add(this.txtBoxCaminho);
            this.Controls.Add(this.btGerar);
            this.Controls.Add(this.treeViewDatasets);
            this.Controls.Add(this.btConnectar);
            this.Controls.Add(this.lbPassword);
            this.Controls.Add(this.lbUsuario);
            this.Controls.Add(this.lvInstancia);
            this.Controls.Add(this.lbDb);
            this.Controls.Add(this.txtBoxPassword);
            this.Controls.Add(this.txtBoxUsuario);
            this.Controls.Add(this.txtBoxInstancia);
            this.Controls.Add(this.txtBoxDB);
            this.Controls.Add(this.pictureBox2);
            this.MaximumSize = new System.Drawing.Size(580, 500);
            this.MinimumSize = new System.Drawing.Size(580, 38);
            this.Name = "FrmClassGenerator";
            this.Text = "Generate Classes from ArcSDE Datasets For ArcGIS 10";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctBoxFC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtBoxDB;
        private System.Windows.Forms.TextBox txtBoxInstancia;
        private System.Windows.Forms.TextBox txtBoxUsuario;
        private System.Windows.Forms.TextBox txtBoxPassword;
        private System.Windows.Forms.Label lbDb;
        private System.Windows.Forms.Label lvInstancia;
        private System.Windows.Forms.Label lbPassword;
        private System.Windows.Forms.Label lbUsuario;
        private System.Windows.Forms.Button btConnectar;
        private System.Windows.Forms.TreeView treeViewDatasets;
        private System.Windows.Forms.Button btGerar;
        private System.Windows.Forms.TextBox txtBoxCaminho;
        private System.Windows.Forms.Button btPesquisar;
        private System.Windows.Forms.ListBox lstBoxArquivos;
        private System.Windows.Forms.Label lbServer;
        private System.Windows.Forms.TextBox txtBoxServer;
        private System.Windows.Forms.Label lbVersao;
        private System.Windows.Forms.TextBox txtBoxVersao;
        private System.Windows.Forms.CheckBox chkBoxFeatueClass;
        private System.Windows.Forms.CheckBox chkBoxTables;
        private System.Windows.Forms.TreeView treeViewTables;
        private System.Windows.Forms.CheckBox chkBoxMarcarTodosFC;
        private System.Windows.Forms.CheckBox chkBoxMarcarTodosTable;
        private System.Windows.Forms.PictureBox pctBoxFC;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}

