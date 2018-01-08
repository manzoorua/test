namespace MonitorAgent
{
	partial class Form1
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.partialImage = new System.Windows.Forms.Label();
			this.fullImage = new System.Windows.Forms.Label();
			this.pollInterval = new System.Windows.Forms.Label();
			this.pollMode = new System.Windows.Forms.Label();
			this.lastUpdate = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.currentDirectory = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(269, 45);
			this.label1.TabIndex = 8;
			this.label1.Text = "Poll Mode:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(12, 59);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(269, 45);
			this.label2.TabIndex = 9;
			this.label2.Text = "Poll Interval (ms):";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(12, 108);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(269, 45);
			this.label3.TabIndex = 10;
			this.label3.Text = "Full Image:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(12, 156);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(269, 45);
			this.label4.TabIndex = 11;
			this.label4.Text = "Partial Image:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// partialImage
			// 
			this.partialImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.partialImage.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.partialImage.Location = new System.Drawing.Point(275, 156);
			this.partialImage.Name = "partialImage";
			this.partialImage.Size = new System.Drawing.Size(269, 45);
			this.partialImage.TabIndex = 15;
			this.partialImage.Text = "Unknown";
			this.partialImage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// fullImage
			// 
			this.fullImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.fullImage.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.fullImage.Location = new System.Drawing.Point(275, 108);
			this.fullImage.Name = "fullImage";
			this.fullImage.Size = new System.Drawing.Size(269, 45);
			this.fullImage.TabIndex = 14;
			this.fullImage.Text = "Unknown";
			this.fullImage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pollInterval
			// 
			this.pollInterval.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pollInterval.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.pollInterval.Location = new System.Drawing.Point(275, 59);
			this.pollInterval.Name = "pollInterval";
			this.pollInterval.Size = new System.Drawing.Size(269, 45);
			this.pollInterval.TabIndex = 13;
			this.pollInterval.Text = "Unknown";
			this.pollInterval.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pollMode
			// 
			this.pollMode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pollMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.pollMode.Location = new System.Drawing.Point(275, 9);
			this.pollMode.Name = "pollMode";
			this.pollMode.Size = new System.Drawing.Size(269, 45);
			this.pollMode.TabIndex = 12;
			this.pollMode.Text = "Unknown";
			this.pollMode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lastUpdate
			// 
			this.lastUpdate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lastUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lastUpdate.Location = new System.Drawing.Point(275, 204);
			this.lastUpdate.Name = "lastUpdate";
			this.lastUpdate.Size = new System.Drawing.Size(269, 45);
			this.lastUpdate.TabIndex = 17;
			this.lastUpdate.Text = "Unknown";
			this.lastUpdate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(12, 204);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(269, 45);
			this.label6.TabIndex = 16;
			this.label6.Text = "Last Update:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// currentDirectory
			// 
			this.currentDirectory.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.currentDirectory.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.currentDirectory.Location = new System.Drawing.Point(275, 253);
			this.currentDirectory.Name = "currentDirectory";
			this.currentDirectory.Size = new System.Drawing.Size(269, 45);
			this.currentDirectory.TabIndex = 19;
			this.currentDirectory.Text = "Unknown";
			this.currentDirectory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label7
			// 
			this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label7.Location = new System.Drawing.Point(12, 253);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(269, 45);
			this.label7.TabIndex = 18;
			this.label7.Text = "Current Directory:";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(563, 356);
			this.Controls.Add(this.currentDirectory);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.lastUpdate);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.partialImage);
			this.Controls.Add(this.fullImage);
			this.Controls.Add(this.pollInterval);
			this.Controls.Add(this.pollMode);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Name = "Form1";
			this.Text = "Monitor Agent";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label partialImage;
		private System.Windows.Forms.Label fullImage;
		private System.Windows.Forms.Label pollInterval;
		private System.Windows.Forms.Label pollMode;
		private System.Windows.Forms.Label lastUpdate;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label currentDirectory;
		private System.Windows.Forms.Label label7;
	}
}

