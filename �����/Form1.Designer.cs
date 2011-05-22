using System;

namespace Шашки
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.новаяИграToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.сКомпьютеромToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.сЧеловекомToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.сдатьсяToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.статистикаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.настройкиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.выходToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.справкаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.какИгратьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.оПрограммеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timerLabel = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lblFirstPlayerName = new System.Windows.Forms.Label();
            this.lblSeconPlayerName = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem,
            this.справкаToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(799, 24);
            this.menuStrip1.TabIndex = 35;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.новаяИграToolStripMenuItem,
            this.сдатьсяToolStripMenuItem,
            this.статистикаToolStripMenuItem,
            this.toolStripMenuItem2,
            this.настройкиToolStripMenuItem,
            this.toolStripMenuItem1,
            this.выходToolStripMenuItem});
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            this.файлToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.файлToolStripMenuItem.Text = "Игра";
            this.файлToolStripMenuItem.Click += new System.EventHandler(this.файлToolStripMenuItemClick);
            // 
            // новаяИграToolStripMenuItem
            // 
            this.новаяИграToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.сКомпьютеромToolStripMenuItem,
            this.сЧеловекомToolStripMenuItem});
            this.новаяИграToolStripMenuItem.Name = "новаяИграToolStripMenuItem";
            this.новаяИграToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.новаяИграToolStripMenuItem.Text = "Новая игра";
            this.новаяИграToolStripMenuItem.Click += new System.EventHandler(this.новаяИграToolStripMenuItemClick);
            // 
            // сКомпьютеромToolStripMenuItem
            // 
            this.сКомпьютеромToolStripMenuItem.Name = "сКомпьютеромToolStripMenuItem";
            this.сКомпьютеромToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.сКомпьютеромToolStripMenuItem.Text = "С компьютером";
            this.сКомпьютеромToolStripMenuItem.Click += new System.EventHandler(this.сКомпьютеромToolStripMenuItemClick);
            // 
            // сЧеловекомToolStripMenuItem
            // 
            this.сЧеловекомToolStripMenuItem.Name = "сЧеловекомToolStripMenuItem";
            this.сЧеловекомToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.сЧеловекомToolStripMenuItem.Text = "С человеком";
            this.сЧеловекомToolStripMenuItem.Click += new System.EventHandler(this.сЧеловекомToolStripMenuItemClick);
            // 
            // сдатьсяToolStripMenuItem
            // 
            this.сдатьсяToolStripMenuItem.Name = "сдатьсяToolStripMenuItem";
            this.сдатьсяToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.сдатьсяToolStripMenuItem.Text = "Сдаться";
            this.сдатьсяToolStripMenuItem.Click += new System.EventHandler(this.сдатьсяToolStripMenuItemClick);
            // 
            // статистикаToolStripMenuItem
            // 
            this.статистикаToolStripMenuItem.Name = "статистикаToolStripMenuItem";
            this.статистикаToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.статистикаToolStripMenuItem.Text = "Статистика";
            this.статистикаToolStripMenuItem.Click += new System.EventHandler(this.статистикаToolStripMenuItemClick);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(133, 6);
            // 
            // настройкиToolStripMenuItem
            // 
            this.настройкиToolStripMenuItem.Name = "настройкиToolStripMenuItem";
            this.настройкиToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.настройкиToolStripMenuItem.Text = "Настройки";
            this.настройкиToolStripMenuItem.Click += new System.EventHandler(this.настройкиToolStripMenuItemClick);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(133, 6);
            // 
            // выходToolStripMenuItem
            // 
            this.выходToolStripMenuItem.Name = "выходToolStripMenuItem";
            this.выходToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.выходToolStripMenuItem.Text = "Выход";
            this.выходToolStripMenuItem.Click += new System.EventHandler(this.выходToolStripMenuItemClick);
            // 
            // справкаToolStripMenuItem
            // 
            this.справкаToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.какИгратьToolStripMenuItem,
            this.оПрограммеToolStripMenuItem});
            this.справкаToolStripMenuItem.Name = "справкаToolStripMenuItem";
            this.справкаToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.справкаToolStripMenuItem.Text = "Справка";
            // 
            // какИгратьToolStripMenuItem
            // 
            this.какИгратьToolStripMenuItem.Name = "какИгратьToolStripMenuItem";
            this.какИгратьToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.какИгратьToolStripMenuItem.Text = "Как играть?";
            this.какИгратьToolStripMenuItem.Click += new System.EventHandler(this.какИгратьToolStripMenuItemClick);
            // 
            // оПрограммеToolStripMenuItem
            // 
            this.оПрограммеToolStripMenuItem.Name = "оПрограммеToolStripMenuItem";
            this.оПрограммеToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.оПрограммеToolStripMenuItem.Text = "О программе";
            this.оПрограммеToolStripMenuItem.Click += new System.EventHandler(this.оПрограммеToolStripMenuItemClick);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1Tick);
            // 
            // timerLabel
            // 
            this.timerLabel.AutoSize = true;
            this.timerLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(41)))));
            this.timerLabel.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.timerLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(112)))), ((int)(((byte)(91)))), ((int)(((byte)(68)))));
            this.timerLabel.Location = new System.Drawing.Point(377, 90);
            this.timerLabel.Name = "timerLabel";
            this.timerLabel.Size = new System.Drawing.Size(46, 19);
            this.timerLabel.TabIndex = 36;
            this.timerLabel.Text = "00:00";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(200, 160);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(400, 400);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1Paint);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1MouseDown);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1MouseUp);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::Шашки.Properties.Resources.Подложка_стола;
            this.pictureBox2.Location = new System.Drawing.Point(0, -40);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(800, 800);
            this.pictureBox2.TabIndex = 37;
            this.pictureBox2.TabStop = false;
            // 
            // lblFirstPlayerName
            // 
            this.lblFirstPlayerName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(41)))));
            this.lblFirstPlayerName.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.lblFirstPlayerName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(112)))), ((int)(((byte)(91)))), ((int)(((byte)(68)))));
            this.lblFirstPlayerName.Location = new System.Drawing.Point(43, 90);
            this.lblFirstPlayerName.MaximumSize = new System.Drawing.Size(130, 19);
            this.lblFirstPlayerName.Name = "lblFirstPlayerName";
            this.lblFirstPlayerName.Size = new System.Drawing.Size(130, 19);
            this.lblFirstPlayerName.TabIndex = 38;
            this.lblFirstPlayerName.Text = "label1";
            this.lblFirstPlayerName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblSeconPlayerName
            // 
            this.lblSeconPlayerName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(41)))));
            this.lblSeconPlayerName.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.lblSeconPlayerName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(112)))), ((int)(((byte)(91)))), ((int)(((byte)(68)))));
            this.lblSeconPlayerName.Location = new System.Drawing.Point(626, 90);
            this.lblSeconPlayerName.MaximumSize = new System.Drawing.Size(133, 19);
            this.lblSeconPlayerName.Name = "lblSeconPlayerName";
            this.lblSeconPlayerName.Size = new System.Drawing.Size(133, 19);
            this.lblSeconPlayerName.TabIndex = 39;
            this.lblSeconPlayerName.Text = "label1";
            this.lblSeconPlayerName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.ClientSize = new System.Drawing.Size(799, 662);
            this.Controls.Add(this.lblSeconPlayerName);
            this.Controls.Add(this.lblFirstPlayerName);
            this.Controls.Add(this.timerLabel);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.pictureBox2);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximumSize = new System.Drawing.Size(815, 700);
            this.MinimumSize = new System.Drawing.Size(645, 626);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Игра шашки";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.form1FormClosing);
            this.Shown += new System.EventHandler(this.form1Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.form1MouseUp);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem настройкиToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem справкаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem какИгратьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem оПрограммеToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripMenuItem новаяИграToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem сдатьсяToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem выходToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem сКомпьютеромToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem сЧеловекомToolStripMenuItem;
        private Checker[] checkerArray;  //массив шашек

        /// <summary>
        ///   Чей ход  true = верхние false = нижние
        /// </summary>
        private bool step;

        private bool logMove; //отображать ходы в подписи формы?
        private static PfSearchInfo method = new PfSearchInfo(Form1.seacrResultDelegate);

        /// <summary>
        ///   Создание новой игры в движке
        /// </summary>
        partial void newGame();

        private System.Windows.Forms.ToolStripMenuItem статистикаToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label timerLabel;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label lblFirstPlayerName;
        private System.Windows.Forms.Label lblSeconPlayerName;
    }
}

