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
            this.btnGetColumn = new System.Windows.Forms.Button();
            this.txtColumnName = new System.Windows.Forms.TextBox();
            this.txtFieldName = new System.Windows.Forms.TextBox();
            this.btnGetField = new System.Windows.Forms.Button();
            this.txtText = new System.Windows.Forms.TextBox();
            this.btnHasText = new System.Windows.Forms.Button();
            this.btnRender = new System.Windows.Forms.Button();
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
            // btnGetColumn
            // 
            this.btnGetColumn.Location = new System.Drawing.Point(163, 51);
            this.btnGetColumn.Name = "btnGetColumn";
            this.btnGetColumn.Size = new System.Drawing.Size(75, 23);
            this.btnGetColumn.TabIndex = 12;
            this.btnGetColumn.Text = "GetColumn";
            this.btnGetColumn.UseVisualStyleBackColor = true;
            this.btnGetColumn.Click += new System.EventHandler(this.btnGetColumn_Click);
            // 
            // txtColumnName
            // 
            this.txtColumnName.Location = new System.Drawing.Point(15, 53);
            this.txtColumnName.Name = "txtColumnName";
            this.txtColumnName.Size = new System.Drawing.Size(142, 20);
            this.txtColumnName.TabIndex = 13;
            // 
            // txtFieldName
            // 
            this.txtFieldName.Location = new System.Drawing.Point(15, 82);
            this.txtFieldName.Name = "txtFieldName";
            this.txtFieldName.Size = new System.Drawing.Size(142, 20);
            this.txtFieldName.TabIndex = 15;
            // 
            // btnGetField
            // 
            this.btnGetField.Location = new System.Drawing.Point(163, 80);
            this.btnGetField.Name = "btnGetField";
            this.btnGetField.Size = new System.Drawing.Size(75, 23);
            this.btnGetField.TabIndex = 14;
            this.btnGetField.Text = "GetField";
            this.btnGetField.UseVisualStyleBackColor = true;
            this.btnGetField.Click += new System.EventHandler(this.btnGetField_Click);
            // 
            // txtText
            // 
            this.txtText.Location = new System.Drawing.Point(15, 111);
            this.txtText.Name = "txtText";
            this.txtText.Size = new System.Drawing.Size(142, 20);
            this.txtText.TabIndex = 17;
            // 
            // btnHasText
            // 
            this.btnHasText.Location = new System.Drawing.Point(163, 109);
            this.btnHasText.Name = "btnHasText";
            this.btnHasText.Size = new System.Drawing.Size(75, 23);
            this.btnHasText.TabIndex = 16;
            this.btnHasText.Text = "HasText";
            this.btnHasText.UseVisualStyleBackColor = true;
            this.btnHasText.Click += new System.EventHandler(this.btnHasText_Click);
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
            // FrmPdfInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 461);
            this.Controls.Add(this.btnRender);
            this.Controls.Add(this.txtText);
            this.Controls.Add(this.btnHasText);
            this.Controls.Add(this.txtFieldName);
            this.Controls.Add(this.btnGetField);
            this.Controls.Add(this.txtColumnName);
            this.Controls.Add(this.btnGetColumn);
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
        private System.Windows.Forms.Button btnGetColumn;
        private System.Windows.Forms.TextBox txtColumnName;
        private System.Windows.Forms.TextBox txtFieldName;
        private System.Windows.Forms.Button btnGetField;
        private System.Windows.Forms.TextBox txtText;
        private System.Windows.Forms.Button btnHasText;
        private System.Windows.Forms.Button btnRender;
    }
}