namespace mytetris
{
    partial class MyTetrisForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.ScoreLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ScoreLabel
            // 
            this.ScoreLabel.Location = new System.Drawing.Point(744, 62);
            this.ScoreLabel.Name = "ScoreLabel";
            this.ScoreLabel.Size = new System.Drawing.Size(37, 21);
            this.ScoreLabel.TabIndex = 0;
            this.ScoreLabel.Text = "0";
            // 
            // MyTetrisForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(836, 736);
            this.Controls.Add(this.ScoreLabel);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Name = "MyTetrisForm";
            this.Text = "僕のテトリス";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Draw);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyInput);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label ScoreLabel;
    }
}

