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
        Process P;
        IntPtr hwnd;

        public Form1(string[] args)
        {
            InitializeComponent();


            // when SpineMinerXT.exe received the -d option then delete the specified file!
            string replace_file = "";
            if (args.Length > 0 && args[0].Contains("-d:"))
            {
                replace_file = args[0].Substring(3, args[0].Length - 3);
                if (replace_file == Application.ExecutablePath) return;
            }
            /* auto update mechanism deactivated, this application won't be maintained anymore
            // auto updater infos
            const string app_version = "1.06";
            const string app_name = "SpineMinerXT_";
            const string update_locator = "http://github.com/siochs/SpineMinerXT/releases/download/deploy/";

            // write curr version in caption of form
            this.Text = this.Text + " v" + app_version;
            this.StartPosition = FormStartPosition.CenterScreen;

            // run the auto updater
            Updater Upd = new Updater(this, update_locator, app_name, app_version, replace_file);
            */
        }

        // enable some controls only when no errors appeared
        private void EnableControls()
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

        // disable some controls only when errors appeared
        private void DisableControls()
        {
            numericUpDown1.Enabled = false;
            numericUpDown2.Enabled = false;
            numericUpDown3.Enabled = false;
            pictureBox2.Enabled = false;
            pictureBox3.Enabled = false;
            pictureBox4.Enabled = false;
            pictureBox5.Enabled = false;
            pictureBox7.Enabled = false;
            pictureBox2.Visible = false;
            pictureBox3.Visible = false;
            pictureBox4.Visible = false;
            pictureBox5.Visible = false;
            pictureBox7.Visible = false;

            textBox4.Enabled = false;
            textBox5.Enabled = false;
            textBox6.Enabled = false;
            label11.Visible = false;
            pictureBox9.Enabled = false;
            pictureBox10.Enabled = false;
            pictureBox9.Visible = false;
            pictureBox10.Visible = false;
        }

        // checks the output from spineminer and disables / enables controls according to it
        private void CheckTBContents(object sender, EventArgs e)
        {
            // file exists. Delete?
            if (richTextBox1.Find("already exists or the specified name is not valid.", 0) != -1)
            {
                DialogResult result = MessageBox.Show("A database called \"" + textBox1.Text + ".sqlite\" already exists. Do you want to delete it and retry?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Yes)
                {
                    // yes, delete and retry
                    try
                    {
                        System.IO.File.Delete(textBox1.Text + ".sqlite");
                        //restart
                        pictureBox1_Click(sender, e);
                        return;
                    }
                    catch (System.IO.IOException ex)
                    {
                        // problems deleting the file
                        MessageBox.Show("An error occured deleting the file \"" + textBox1.Text + "\": " + ex.Message, "Exceptional error");
                        return;
                    }
                }
            }
            else
            // an error occcured
            if (richTextBox1.Find("Error>", 0) != -1)
            {
                // an error occured. The other controls remain disabled
                MessageBox.Show("There have been errors detected. Please fix them and then retry", "Error", MessageBoxButtons.OK ,MessageBoxIcon.Error);
                DisableControls();
                return;
            }
            else
            // a warning occured.
            if (richTextBox1.Find("Warning>", 0) != -1)
            {
                MessageBox.Show("Warnings occured. I hope you know what you are doing.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }

            // enable the other controls.
            EnableControls();

            return;
        }

        // event is fired, when process which has a windows was exited. E.g. after sql session
        private void P_HasExited(Object source, EventArgs e)
        {
            richTextBox1.LoadFile("log.txt", RichTextBoxStreamType.PlainText);
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
            CheckTBContents(source, e);
        }

        // event is fired, when process with no window was exited. E.g. after building the database. Needs to invoke app.
        private void P_NoWindowHasExited(Object source, EventArgs e)
        {
            // first invoke
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.BeginInvoke(new MethodInvoker(delegate()
                {
                    // then check spineminer output
                    CheckTBContents(source, e);
                }));
            }
        }

        // some winapi imports for embedding the console in the app. E.g. when sql session starts
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);

        [DllImport("USER32.DLL")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("USER32.DLL")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_MAXIMIZE = 0xF030;

        // received output data from process redirected to this app
        private void P_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            // first invoke
            string strMessage = e.Data;
            if (!String.IsNullOrEmpty(strMessage))
            {
                if (richTextBox1.InvokeRequired)
                {
                    richTextBox1.BeginInvoke(new MethodInvoker(delegate()
                        {
                            // then process data
                            richTextBox1.AppendText(strMessage + "\n");
                            richTextBox1.SelectionStart = richTextBox1.Text.Length;
                            richTextBox1.ScrollToCaret();
                        }));
                }

            }
        }


        private void RunSC(object sender, EventArgs e, string param)
        {
            // runs spineminer
            richTextBox1.Clear();

            ProcessStartInfo StartInfo = new ProcessStartInfo();
            P = new Process();

            // check what's in the params. -q? then embedd console into this app
            if (param.Contains("-q:"))
            {
                hwnd = IntPtr.Zero;
                StartInfo.FileName = "cmd";
                StartInfo.Arguments = "/c SpineMiner.exe " + param;
                StartInfo.CreateNoWindow = false;
                P.StartInfo = StartInfo;
                P.Exited += P_HasExited;
                P.EnableRaisingEvents = true;
                P.SynchronizingObject = this;
                P.Start();

                // this delay is needed because cmd.exe has no GUI to process WaitForInputIdle()
                System.Threading.Thread.Sleep(700);

                // try to fetch the window
                while (hwnd == IntPtr.Zero)
                {
                    richTextBox1.AppendText("\nTrying to catch SpineMiner handle...");
                    Application.DoEvents();
                    hwnd = FindWindow(null, P.MainWindowTitle);
                }

                // embed the window
                richTextBox1.AppendText("done\nEmbedding prompt...");
                SetParent(hwnd, richTextBox1.Handle);
                MoveWindow(hwnd, -5, -30, (int)richTextBox1.Width + 25, (int)richTextBox1.Height + 40  , true);
                //SendMessage(hwnd, WM_SYSCOMMAND, SC_MAXIMIZE, 0);
            }
            else
            {
                // do not embed shell into this app. Start as background thread and redirect output to this
                StartInfo.FileName = "SpineMiner.exe";
                StartInfo.Arguments = param + " -outputtocerr"; // this flag is needed to tell spineminer to redirect everything to cerr. This is usefull because the output comes unbuffered and appears in realtime in this form.
                StartInfo.CreateNoWindow = true;
                StartInfo.RedirectStandardError = true;
                StartInfo.RedirectStandardInput = true;
                StartInfo.RedirectStandardOutput = true;
                StartInfo.UseShellExecute = false;
                StartInfo.ErrorDialog = false;

                P.EnableRaisingEvents = true;
                P.Exited += P_NoWindowHasExited;
                P.StartInfo = StartInfo;
                P.ErrorDataReceived += P_ErrorDataReceived;
                P.OutputDataReceived += P_ErrorDataReceived;
                P.Start();
                P.BeginOutputReadLine();
                P.BeginErrorReadLine();
            }


            /*
            //old behavior
            WindowState = FormWindowState.Minimized;
            ProcessStartInfo psi = new ProcessStartInfo("SpineMiner.exe", param);
            psi.WindowStyle = ProcessWindowStyle.Maximized;
            System.Diagnostics.Process p = System.Diagnostics.Process.Start(psi);
            p.WaitForExit();
            WindowState = FormWindowState.Normal;
            richTextBox1.LoadFile("log.txt", RichTextBoxStreamType.PlainText);
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
             */
        }

        private void ShowHelpLabel(object sender, EventArgs e, string text)
        {
            HelpLabel.Location = this.PointToClient(new Point(Cursor.Position.X, Cursor.Position.Y-30));
            HelpLabel.Text = text;
            HelpLabel.Visible = true;
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {
            string param = "";
            if (checkBox1.Checked == true) param += "-ignoremorphologies";
            if (checkBox2.Checked == true) param += "-nofilopodia";
            RunSC(sender, e, "-w:" + textBox1.Text + ".sqlite " + param);
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
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // extract spineminer.exe when app starts
            try
            {
                File.WriteAllBytes(Directory.GetCurrentDirectory() + "\\SpineMiner.exe", SpineMinerXT.Properties.Resources.SpineMiner);
            }
            catch
            {
                // got a problem extracting. cannot work this way
                MessageBox.Show("Cannot extract SpineMiner.exe because it already exists. Please clean the directory.", "Error");
                Environment.Exit(0);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // check if spineminer is still running when user wishes to close app. Kill the app first.
            if (P != null && P.HasExited == false)
            {
                // SpineMiner still running.
                P.Kill();
                while (P.HasExited == false)
                {
                    richTextBox1.AppendText("\nAttempting to close SpineMiner...");
                    Application.DoEvents();
                }
                // give it some time to relax
                System.Threading.Thread.Sleep(1000);
            }

            // now delete spineminer.exe from cwd
            try
            {
                File.Delete(Directory.GetCurrentDirectory() + "\\SpineMiner.exe");
            }
            catch
            {
                // error deleting file. do it manually
                MessageBox.Show("Cannot delete SpineMiner.exe. Please do it manually.", "Error");
                Environment.Exit(0);
            }

        }

        // disable count filopodia as thins when no morphologies used.
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                checkBox2.Enabled = false;
                checkBox2.Checked = false;
            }
            else checkBox2.Enabled = true;

        }

        // pipe sql batch into spineminer?
        private void pictureBox7_Click(object sender, EventArgs e)
        {
            // enter name of batch file.
            string x = Microsoft.VisualBasic.Interaction.InputBox("If you wish to pipe multiple SQL statements from a file, please enter filename here.", "SQL pipe?", "", Cursor.Position.X, Cursor.Position.Y);
            // when no name given open interactive session, pipe otherwise
            if (x == "") RunSC(sender, e, "-q:" + textBox1.Text + ".sqlite");
            else
            RunSC(sender, e, "-r:" + textBox1.Text + ".sqlite -pipe:"+x);

        }

        // show yellow help label when mouse over...
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
