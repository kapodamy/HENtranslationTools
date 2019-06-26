namespace HEN_locate
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_custom = new System.Windows.Forms.TextBox();
            this.textBox_res = new System.Windows.Forms.TextBox();
            this.textBox_orig = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button_apply = new System.Windows.Forms.Button();
            this.comboBox_src = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button_open = new System.Windows.Forms.Button();
            this.textBox_path = new System.Windows.Forms.TextBox();
            this.label_warn = new System.Windows.Forms.Label();
            this.timer_warn_fadeOut = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 262);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Result:";
            // 
            // textBox_custom
            // 
            this.textBox_custom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_custom.Enabled = false;
            this.textBox_custom.Location = new System.Drawing.Point(12, 220);
            this.textBox_custom.Name = "textBox_custom";
            this.textBox_custom.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.textBox_custom.Size = new System.Drawing.Size(368, 20);
            this.textBox_custom.TabIndex = 5;
            this.textBox_custom.TextChanged += new System.EventHandler(this.TextBox_custom_TextChanged);
            // 
            // textBox_res
            // 
            this.textBox_res.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_res.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_res.Location = new System.Drawing.Point(12, 278);
            this.textBox_res.Name = "textBox_res";
            this.textBox_res.ReadOnly = true;
            this.textBox_res.Size = new System.Drawing.Size(368, 20);
            this.textBox_res.TabIndex = 6;
            // 
            // textBox_orig
            // 
            this.textBox_orig.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_orig.Location = new System.Drawing.Point(12, 164);
            this.textBox_orig.Name = "textBox_orig";
            this.textBox_orig.ReadOnly = true;
            this.textBox_orig.Size = new System.Drawing.Size(368, 20);
            this.textBox_orig.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 204);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Custom:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 148);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Original (from the file):";
            // 
            // button_apply
            // 
            this.button_apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_apply.Enabled = false;
            this.button_apply.Location = new System.Drawing.Point(280, 324);
            this.button_apply.Name = "button_apply";
            this.button_apply.Size = new System.Drawing.Size(100, 25);
            this.button_apply.TabIndex = 7;
            this.button_apply.Text = "Apply changes";
            this.button_apply.UseVisualStyleBackColor = true;
            this.button_apply.Click += new System.EventHandler(this.Button_apply_Click);
            // 
            // comboBox_src
            // 
            this.comboBox_src.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_src.Enabled = false;
            this.comboBox_src.Location = new System.Drawing.Point(15, 105);
            this.comboBox_src.Name = "comboBox_src";
            this.comboBox_src.Size = new System.Drawing.Size(365, 21);
            this.comboBox_src.TabIndex = 3;
            this.comboBox_src.SelectedIndexChanged += new System.EventHandler(this.ComboBox_src_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "String:";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(368, 74);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File source";
            // 
            // panel1
            // 
            this.panel1.AllowDrop = true;
            this.panel1.Controls.Add(this.button_open);
            this.panel1.Controls.Add(this.textBox_path);
            this.panel1.Location = new System.Drawing.Point(6, 19);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(356, 49);
            this.panel1.TabIndex = 2;
            this.panel1.DragDrop += new System.Windows.Forms.DragEventHandler(this.Panel1_DragDrop);
            this.panel1.DragEnter += new System.Windows.Forms.DragEventHandler(this.Panel1_DragEnter);
            // 
            // button_open
            // 
            this.button_open.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_open.Location = new System.Drawing.Point(281, 26);
            this.button_open.Name = "button_open";
            this.button_open.Size = new System.Drawing.Size(75, 23);
            this.button_open.TabIndex = 1;
            this.button_open.Text = "...";
            this.button_open.UseVisualStyleBackColor = true;
            this.button_open.Click += new System.EventHandler(this.Button_open_Click);
            // 
            // textBox_path
            // 
            this.textBox_path.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_path.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.textBox_path.Location = new System.Drawing.Point(0, 0);
            this.textBox_path.MaxLength = 3200;
            this.textBox_path.Name = "textBox_path";
            this.textBox_path.ReadOnly = true;
            this.textBox_path.ShortcutsEnabled = false;
            this.textBox_path.Size = new System.Drawing.Size(356, 13);
            this.textBox_path.TabIndex = 0;
            this.textBox_path.TabStop = false;
            this.textBox_path.Text = "(no file loaded)";
            // 
            // label_warn
            // 
            this.label_warn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label_warn.AutoSize = true;
            this.label_warn.ForeColor = System.Drawing.Color.Red;
            this.label_warn.Location = new System.Drawing.Point(323, 204);
            this.label_warn.Name = "label_warn";
            this.label_warn.Size = new System.Drawing.Size(57, 13);
            this.label_warn.TabIndex = 8;
            this.label_warn.Text = "MAX SIZE";
            this.label_warn.Visible = false;
            // 
            // timer_warn_fadeOut
            // 
            this.timer_warn_fadeOut.Interval = 1500;
            this.timer_warn_fadeOut.Tick += new System.EventHandler(this.Timer_warn_fadeOut_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(393, 357);
            this.Controls.Add(this.label_warn);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.comboBox_src);
            this.Controls.Add(this.button_apply);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_orig);
            this.Controls.Add(this.textBox_res);
            this.Controls.Add(this.textBox_custom);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "HEN locate";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_custom;
        private System.Windows.Forms.TextBox textBox_res;
        private System.Windows.Forms.TextBox textBox_orig;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button_apply;
        private System.Windows.Forms.ComboBox comboBox_src;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox_path;
        private System.Windows.Forms.Button button_open;
        private System.Windows.Forms.Label label_warn;
        private System.Windows.Forms.Timer timer_warn_fadeOut;
        private System.Windows.Forms.Panel panel1;
    }
}

