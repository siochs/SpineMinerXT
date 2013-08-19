using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using MyUpdater;


namespace SpineMinerXT
{
    public partial class Form1 : Form
    {
        const int DO_NOTHING = 0;
        const int DO_REPEAT = 1;
        const int DO_NOENABLE = 2;
        const int DO_ENABLE = 3;

        public Form1(string[] args)
        {
            InitializeComponent();

            const string app_version = "1.00";
            const string app_name = "SpineMinerXT_";
            const string update_locator = "http://github.com/siochs/SpineMinerXT/releases/download/deploy/";
            
            string replace_file = "";
            if (args.Length > 0 && args[0].Contains("-d:"))
            {
                replace_file = args[0].Substring(3, args[0].Length - 3);
                if (replace_file == Application.ExecutablePath) return;
            }

            this.Text = this.Text + " v" + app_version;
            this.StartPosition = FormStartPosition.CenterScreen;

            Updater Upd = new Updater(this, update_locator, app_name, app_version, replace_file);
        }

        private int CheckTBContents(object sender, EventArgs e)
        {

            if (richTextBox1.Find("already exists or the specified name is not valid.", 0) != -1)
            {
                DialogResult result = MessageBox.Show("A database called \"" + textBox1.Text + ".sqlite\" already exists. Do you want to delete it and retry?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        System.IO.File.Delete(textBox1.Text + ".sqlite");
                        return DO_REPEAT;
                    }
                    catch (System.IO.IOException ex)
                    {
                        MessageBox.Show("An error occured deleting the file \"" + textBox1.Text + "\": " + ex.Message, "Exceptional error");
                        return DO_NOENABLE;
                    }
                }
                else
                    return DO_NOENABLE;
            }
            else
            if (richTextBox1.Find("Error>", 0) != -1)
            {
                MessageBox.Show("There have been errors detected. Please fix them and then retry", "Error", MessageBoxButtons.OK ,MessageBoxIcon.Error);
                return DO_NOENABLE;
            }
            else
                if (richTextBox1.Find("Warning>", 0) != -1)
                {
                    MessageBox.Show("Warnings occured. I hope you know what you are doing.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return DO_ENABLE;
                }

            return DO_ENABLE;
        }


        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        private void RunSC(object sender, EventArgs e, string param)
        {
            richTextBox1.Clear();
            WindowState = FormWindowState.Minimized;

            ProcessStartInfo psi = new ProcessStartInfo("SpineMiner.exe", param);
            psi.WindowStyle = ProcessWindowStyle.Maximized;
            System.Diagnostics.Process p = System.Diagnostics.Process.Start(psi);
            p.WaitForExit();
            WindowState = FormWindowState.Normal;
            richTextBox1.LoadFile("log.txt", RichTextBoxStreamType.PlainText);
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }

        private void ShowHelpLabel(object sender, EventArgs e, string text)
        {
            HelpLabel.Location = this.PointToClient(new Point(Cursor.Position.X, Cursor.Position.Y-30));
            HelpLabel.Text = text;
            HelpLabel.Visible = true;
        }

        

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Begin:
            int r = 0;
            string param = "";
            if (checkBox1.Checked == true) param += "-ignoremorphologies";
            if (checkBox2.Checked == true) param += "-nofilopodia";
            RunSC(sender, e, "-w:" + textBox1.Text + ".sqlite " + param);
            r = CheckTBContents(sender, e);
            if (r == DO_REPEAT) goto Begin;
            else if (r == DO_ENABLE)
            {
                numericUpDown1.Enabled = true;
                numericUpDown2.Enabled = true;
                numericUpDown3.Enabled = true;
                pictureBox2.Enabled = true;
                pictureBox3.Enabled = true;
                pictureBox4.Enabled = true;
                pictureBox5.Enabled = true;
                pictureBox7.Enabled = true;
                pictureBox2.Visible = true;
                pictureBox3.Visible = true;
                pictureBox4.Visible = true;
                pictureBox5.Visible = true;
                pictureBox7.Visible = true;

                textBox4.Enabled = true;
                textBox5.Enabled = true;
                textBox6.Enabled = true;
                label11.Visible = true;
                pictureBox9.Enabled = true;
                pictureBox10.Enabled = true;
                pictureBox9.Visible = true;
                pictureBox10.Visible = true;

            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            RunSC(sender, e, "-r:"+textBox1.Text+".sqlite -calcsurvival:"+numericUpDown1.Value);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            RunSC(sender, e, "-r:" + textBox1.Text + ".sqlite -calctransients:" + numericUpDown2.Value);
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            RunSC(sender, e, "-r:" + textBox1.Text + ".sqlite -calcnewgained:" + numericUpDown3.Value);
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            RunSC(sender, e, "-c:" + textBox1.Text + ".sqlite");
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            RunSC(sender, e, "-r:" + textBox1.Text + ".sqlite");
            int r = CheckTBContents(sender, e);
            if (r == DO_ENABLE)
            {
                numericUpDown1.Enabled = true;
                numericUpDown2.Enabled = true;
                numericUpDown3.Enabled = true;
                pictureBox2.Enabled = true;
                pictureBox3.Enabled = true;
                pictureBox4.Enabled = true;
                pictureBox5.Enabled = true;
                pictureBox7.Enabled = true;
                pictureBox2.Visible = true;
                pictureBox3.Visible = true;
                pictureBox4.Visible = true;
                pictureBox5.Visible = true;
                pictureBox7.Visible = true;
                
                textBox4.Enabled = true;
                textBox5.Enabled = true;
                textBox6.Enabled = true;
                label11.Visible = true;
                pictureBox9.Enabled = true;
                pictureBox10.Enabled = true;
                pictureBox9.Visible = true;
                pictureBox10.Visible = true;
                

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try //hier
            {
                File.WriteAllBytes(Directory.GetCurrentDirectory() + "\\SpineMiner.exe", SpineMinerXT.Properties.Resources.SpineMiner);
            }
            catch
            {
                MessageBox.Show("Cannot extract SpineMiner.exe because it already exists. Please clean the directory.", "Error");
                Environment.Exit(0);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                File.Delete(Directory.GetCurrentDirectory() + "\\SpineMiner.exe");
            }
            catch
            {
                MessageBox.Show("Cannot delete SpineMiner.exe. Please do it manually.", "Error");
                Environment.Exit(0);
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                checkBox2.Enabled = false;
                checkBox2.Checked = false;
            }
            else checkBox2.Enabled = true;

        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            string x = Microsoft.VisualBasic.Interaction.InputBox("If you wish to pipe multiple SQL statements from a file, please enter filename here.", "SQL pipe?", "", Cursor.Position.X, Cursor.Position.Y);
            if (x == "") RunSC(sender, e, "-q:" + textBox1.Text + ".sqlite");
            else
                RunSC(sender, e, "-r:" + textBox1.Text + ".sqlite -pipe:"+x);

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
            
        }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            ShowHelpLabel(sender, e, "Run analysis for " + textBox1.Text);
        }

        private void groupBox1_MouseHover(object sender, EventArgs e)
        {
            HelpLabel.Visible = false;
        }

        private void richTextBox1_MouseHover(object sender, EventArgs e)
        {
            HelpLabel.Visible = false;
        }

        private void pictureBox6_MouseHover(object sender, EventArgs e)
        {
            ShowHelpLabel(sender, e, "Open database " + textBox1.Text + ".sqlite here.");
        }

        private void pictureBox7_MouseHover(object sender, EventArgs e)
        {
            ShowHelpLabel(sender, e, "Start SQL session with " + textBox1.Text + ".sqlite.");
        }

        private void pictureBox2_MouseHover(object sender, EventArgs e)
        {
            ShowHelpLabel(sender, e, "Recalculate values using\nthis setting.");
        }

        private void pictureBox3_MouseHover(object sender, EventArgs e)
        {
            ShowHelpLabel(sender, e, "Recalculate values using\nthis setting.");
        }

        private void pictureBox4_MouseHover(object sender, EventArgs e)
        {
            ShowHelpLabel(sender, e, "Recalculate values using\nthis setting.");
        }

        private void pictureBox5_MouseHover(object sender, EventArgs e)
        {
            ShowHelpLabel(sender, e, "Export database\nto CSV.");
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This function is currently deactivated.", "Disabled");
            //RunSC(sender, e, "-replace:"+textBox2.Text+","+textBox3.Text);
        }

        private void pictureBox8_MouseHover(object sender, EventArgs e)
        {
            ShowHelpLabel(sender, e, "Do find & replace\nWarning: beta status!"); 
        }

        private void pictureBox10_MouseHover(object sender, EventArgs e)
        {
            ShowHelpLabel(sender, e, "Reset\nDBSCAN"); 
        }

        private void pictureBox9_MouseHover(object sender, EventArgs e)
        {
            ShowHelpLabel(sender, e, "Rund DBSCAN"); 
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            RunSC(sender, e, "-r:" + textBox1.Text + ".sqlite -dbscan:" + textBox4.Text + "," + textBox5.Text + "," + textBox6.Text);
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            RunSC(sender, e, "-r:" + textBox1.Text + ".sqlite -quickdbscan");
        }
    }
}
