namespace ScrumStatsHelper
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnGetTime = new Button();
            txtLog = new TextBox();
            txtAnalysis = new TextBox();
            label1 = new Label();
            cbSprint = new ComboBox();
            dtSprintStart = new DateTimePicker();
            dtSprintEnd = new DateTimePicker();
            SuspendLayout();
            // 
            // btnGetTime
            // 
            btnGetTime.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnGetTime.Location = new Point(566, 9);
            btnGetTime.Name = "btnGetTime";
            btnGetTime.Size = new Size(1274, 23);
            btnGetTime.TabIndex = 0;
            btnGetTime.Text = "Go";
            btnGetTime.UseVisualStyleBackColor = true;
            btnGetTime.Click += AnalyzeSprint;
            // 
            // txtLog
            // 
            txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            txtLog.Location = new Point(20, 38);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ScrollBars = ScrollBars.Both;
            txtLog.Size = new Size(540, 621);
            txtLog.TabIndex = 3;
            // 
            // txtAnalysis
            // 
            txtAnalysis.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtAnalysis.Location = new Point(566, 38);
            txtAnalysis.Multiline = true;
            txtAnalysis.Name = "txtAnalysis";
            txtAnalysis.ScrollBars = ScrollBars.Both;
            txtAnalysis.Size = new Size(1274, 621);
            txtAnalysis.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(19, 12);
            label1.Name = "label1";
            label1.Size = new Size(38, 15);
            label1.TabIndex = 5;
            label1.Text = "Sprint";
            // 
            // cbSprint
            // 
            cbSprint.FormattingEnabled = true;
            cbSprint.Location = new Point(60, 9);
            cbSprint.Name = "cbSprint";
            cbSprint.Size = new Size(40, 23);
            cbSprint.TabIndex = 6;
            cbSprint.SelectedIndexChanged += cbSprint_SelectedIndexChanged;
            // 
            // dtSprintStart
            // 
            dtSprintStart.Location = new Point(114, 9);
            dtSprintStart.Name = "dtSprintStart";
            dtSprintStart.Size = new Size(211, 23);
            dtSprintStart.TabIndex = 7;
            // 
            // dtSprintEnd
            // 
            dtSprintEnd.Location = new Point(345, 9);
            dtSprintEnd.Name = "dtSprintEnd";
            dtSprintEnd.Size = new Size(211, 23);
            dtSprintEnd.TabIndex = 8;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1852, 688);
            Controls.Add(dtSprintEnd);
            Controls.Add(dtSprintStart);
            Controls.Add(cbSprint);
            Controls.Add(label1);
            Controls.Add(txtAnalysis);
            Controls.Add(txtLog);
            Controls.Add(btnGetTime);
            Name = "Form1";
            Text = "ScrumStats";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnGetTime;
        private TextBox txtLog;
        private TextBox txtAnalysis;
        private Label label1;
        private ComboBox cbSprint;
        private DateTimePicker dtSprintStart;
        private DateTimePicker dtSprintEnd;
    }
}