using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Windows.Forms;
using System.Xml.Linq;

namespace UngDungVe2D
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
            openglControl1 = new SharpGL.OpenGLControl();
            ((System.ComponentModel.ISupportInitialize)openglControl1).BeginInit();
            SuspendLayout();
            // 
            // openglControl1
            // 
            openglControl1.BackColor = SystemColors.ControlDark;
            openglControl1.DrawFPS = false;
            openglControl1.Location = new Point(0, 0);
            openglControl1.Margin = new Padding(4, 3, 4, 3);
            openglControl1.Name = "openglControl1";
            openglControl1.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL2_1;
            openglControl1.RenderContextType = SharpGL.RenderContextType.DIBSection;
            openglControl1.RenderTrigger = SharpGL.RenderTrigger.TimerBased;
            openglControl1.Size = new Size(800, 800);
            openglControl1.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(882, 659);
            Controls.Add(openglControl1);
            ForeColor = SystemColors.WindowText;
            HelpButton = true;
            IsMdiContainer = true;
            Name = "Form1";
            ((System.ComponentModel.ISupportInitialize)openglControl1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private SharpGL.OpenGLControl openglControl1;
    }
}
