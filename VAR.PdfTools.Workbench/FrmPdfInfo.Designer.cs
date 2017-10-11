namespace VAR.PdfTools.Workbench
{
    partial class FrmPdfInfo
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
            this.lblOutputs = new System.Windows.Forms.Label();
            this.lblInputs = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtPdfPath = new System.Windows.Forms.TextBox();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.btnProcess = new System.Windows.Forms.Button();
            this.btnGetColumn1 = new System.Windows.Forms.Button();
            this.txtField1 = new System.Windows.Forms.TextBox();
            this.btnGetField1 = new System.Windows.Forms.Button();
            this.btnHasText1 = new System.Windows.Forms.Button();
            this.btnRender = new System.Windows.Forms.Button();
            this.btnHasText2 = new System.Windows.Forms.Button();
            this.btnGetField2 = new System.Windows.Forms.Button();
            this.txtField2 = new System.Windows.Forms.TextBox();
            this.btnGetColumn2 = new System.Windows.Forms.Button();
            this.btnHasText3 = new System.Windows.Forms.Button();
            this.btnGetField3 = new System.Windows.Forms.Button();
            this.txtField3 = new System.Windows.Forms.TextBox();
            this.btnGetColumn3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblOutputs
            // 
            this.lblOutputs.AutoSize = true;
            this.lblOutputs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOutputs.Location = new System.Drawing.Point(12, 143);
            this.lblOutputs.Name = "lblOutputs";
            this.lblOutputs.Size = new System.Drawing.Size(51, 13);
            this.lblOutputs.TabIndex = 11;
            this.lblOutputs.Text = "Outputs";
            // 
            // lblInputs
            // 
            this.lblInputs.AutoSize = true;
            this.lblInputs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInputs.Location = new System.Drawing.Point(12, 9);
            this.lblInputs.Name = "lblInputs";
            this.lblInputs.Size = new System.Drawing.Size(42, 13);
            this.lblInputs.TabIndex = 10;
            this.lblInputs.Text = "Inputs";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(316, 23);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 9;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtPdfPath
            // 
            this.txtPdfPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPdfPath.Location = new System.Drawing.Point(15, 25);
            this.txtPdfPath.Name = "txtPdfPath";
            this.txtPdfPath.Size = new System.Drawing.Size(295, 20);
            this.txtPdfPath.TabIndex = 8;
            // 
            // txtOutput
            // 
            this.txtOutput.AcceptsReturn = true;
            this.txtOutput.AcceptsTab = true;
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOutput.Location = new System.Drawing.Point(15, 159);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOutput.Size = new System.Drawing.Size(457, 290);
            this.txtOutput.TabIndex = 7;
            // 
            // btnProcess
            // 
            this.btnProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnProcess.Location = new System.Drawing.Point(397, 23);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(75, 23);
            this.btnProcess.TabIndex = 6;
            this.btnProcess.Text = "Process";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // btnGetColumn1
            // 
            this.btnGetColumn1.Location = new System.Drawing.Point(292, 51);
            this.btnGetColumn1.Name = "btnGetColumn1";
            this.btnGetColumn1.Size = new System.Drawing.Size(60, 23);
            this.btnGetColumn1.TabIndex = 12;
            this.btnGetColumn1.Text = "GetColumn";
            this.btnGetColumn1.UseVisualStyleBackColor = true;
            this.btnGetColumn1.Click += new System.EventHandler(this.btnGetColumn1_Click);
            // 
            // txtField1
            // 
            this.txtField1.Location = new System.Drawing.Point(15, 53);
            this.txtField1.Name = "txtField1";
            this.txtField1.Size = new System.Drawing.Size(142, 20);
            this.txtField1.TabIndex = 13;
            // 
            // btnGetField1
            // 
            this.btnGetField1.Location = new System.Drawing.Point(226, 51);
            this.btnGetField1.Name = "btnGetField1";
            this.btnGetField1.Size = new System.Drawing.Size(60, 23);
            this.btnGetField1.TabIndex = 14;
            this.btnGetField1.Text = "GetField";
            this.btnGetField1.UseVisualStyleBackColor = true;
            this.btnGetField1.Click += new System.EventHandler(this.btnGetField1_Click);
            // 
            // btnHasText1
            // 
            this.btnHasText1.Location = new System.Drawing.Point(163, 51);
            this.btnHasText1.Name = "btnHasText1";
            this.btnHasText1.Size = new System.Drawing.Size(57, 23);
            this.btnHasText1.TabIndex = 16;
            this.btnHasText1.Text = "HasText";
            this.btnHasText1.UseVisualStyleBackColor = true;
            this.btnHasText1.Click += new System.EventHandler(this.btnHasText1_Click);
            // 
            // btnRender
            // 
            this.btnRender.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRender.Location = new System.Drawing.Point(397, 52);
            this.btnRender.Name = "btnRender";
            this.btnRender.Size = new System.Drawing.Size(75, 23);
            this.btnRender.TabIndex = 18;
            this.btnRender.Text = "Render";
            this.btnRender.UseVisualStyleBackColor = true;
            this.btnRender.Click += new System.EventHandler(this.btnRender_Click);
            // 
            // btnHasText2
            // 
            this.btnHasText2.Location = new System.Drawing.Point(163, 80);
            this.btnHasText2.Name = "btnHasText2";
            this.btnHasText2.Size = new System.Drawing.Size(57, 23);
            this.btnHasText2.TabIndex = 22;
            this.btnHasText2.Text = "HasText";
            this.btnHasText2.UseVisualStyleBackColor = true;
            this.btnHasText2.Click += new System.EventHandler(this.btnHasText2_Click);
            // 
            // btnGetField2
            // 
            this.btnGetField2.Location = new System.Drawing.Point(226, 80);
            this.btnGetField2.Name = "btnGetField2";
            this.btnGetField2.Size = new System.Drawing.Size(60, 23);
            this.btnGetField2.TabIndex = 21;
            this.btnGetField2.Text = "GetField";
            this.btnGetField2.UseVisualStyleBackColor = true;
            this.btnGetField2.Click += new System.EventHandler(this.btnGetField2_Click);
            // 
            // txtField2
            // 
            this.txtField2.Location = new System.Drawing.Point(15, 82);
            this.txtField2.Name = "txtField2";
            this.txtField2.Size = new System.Drawing.Size(142, 20);
            this.txtField2.TabIndex = 20;
            // 
            // btnGetColumn2
            // 
            this.btnGetColumn2.Location = new System.Drawing.Point(292, 80);
            this.btnGetColumn2.Name = "btnGetColumn2";
            this.btnGetColumn2.Size = new System.Drawing.Size(60, 23);
            this.btnGetColumn2.TabIndex = 19;
            this.btnGetColumn2.Text = "GetColumn";
            this.btnGetColumn2.UseVisualStyleBackColor = true;
            this.btnGetColumn2.Click += new System.EventHandler(this.btnGetColumn2_Click);
            // 
            // btnHasText3
            // 
            this.btnHasText3.Location = new System.Drawing.Point(163, 109);
            this.btnHasText3.Name = "btnHasText3";
            this.btnHasText3.Size = new System.Drawing.Size(57, 23);
            this.btnHasText3.TabIndex = 26;
            this.btnHasText3.Text = "HasText";
            this.btnHasText3.UseVisualStyleBackColor = true;
            this.btnHasText3.Click += new System.EventHandler(this.btnHasText3_Click);
            // 
            // btnGetField3
            // 
            this.btnGetField3.Location = new System.Drawing.Point(226, 109);
            this.btnGetField3.Name = "btnGetField3";
            this.btnGetField3.Size = new System.Drawing.Size(60, 23);
            this.btnGetField3.TabIndex = 25;
            this.btnGetField3.Text = "GetField";
            this.btnGetField3.UseVisualStyleBackColor = true;
            this.btnGetField3.Click += new System.EventHandler(this.btnGetField3_Click);
            // 
            // txtField3
            // 
            this.txtField3.Location = new System.Drawing.Point(15, 111);
            this.txtField3.Name = "txtField3";
            this.txtField3.Size = new System.Drawing.Size(142, 20);
            this.txtField3.TabIndex = 24;
            // 
            // btnGetColumn3
            // 
            this.btnGetColumn3.Location = new System.Drawing.Point(292, 109);
            this.btnGetColumn3.Name = "btnGetColumn3";
            this.btnGetColumn3.Size = new System.Drawing.Size(60, 23);
            this.btnGetColumn3.TabIndex = 23;
            this.btnGetColumn3.Text = "GetColumn";
            this.btnGetColumn3.UseVisualStyleBackColor = true;
            this.btnGetColumn3.Click += new System.EventHandler(this.btnGetColumn3_Click);
            // 
            // FrmPdfInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 461);
            this.Controls.Add(this.btnHasText3);
            this.Controls.Add(this.btnGetField3);
            this.Controls.Add(this.txtField3);
            this.Controls.Add(this.btnGetColumn3);
            this.Controls.Add(this.btnHasText2);
            this.Controls.Add(this.btnGetField2);
            this.Controls.Add(this.txtField2);
            this.Controls.Add(this.btnGetColumn2);
            this.Controls.Add(this.btnRender);
            this.Controls.Add(this.btnHasText1);
            this.Controls.Add(this.btnGetField1);
            this.Controls.Add(this.txtField1);
            this.Controls.Add(this.btnGetColumn1);
            this.Controls.Add(this.lblOutputs);
            this.Controls.Add(this.lblInputs);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtPdfPath);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.btnProcess);
            this.Name = "FrmPdfInfo";
            this.Text = "PdfInfo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmPdfInfo_FormClosing);
            this.Load += new System.EventHandler(this.FrmPdfInfo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblOutputs;
        private System.Windows.Forms.Label lblInputs;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtPdfPath;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Button btnProcess;
        private System.Windows.Forms.Button btnGetColumn1;
        private System.Windows.Forms.TextBox txtField1;
        private System.Windows.Forms.Button btnGetField1;
        private System.Windows.Forms.Button btnHasText1;
        private System.Windows.Forms.Button btnRender;
        private System.Windows.Forms.Button btnHasText2;
        private System.Windows.Forms.Button btnGetField2;
        private System.Windows.Forms.TextBox txtField2;
        private System.Windows.Forms.Button btnGetColumn2;
        private System.Windows.Forms.Button btnHasText3;
        private System.Windows.Forms.Button btnGetField3;
        private System.Windows.Forms.TextBox txtField3;
        private System.Windows.Forms.Button btnGetColumn3;
    }
}