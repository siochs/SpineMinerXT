using System.Windows.Forms;
using System.Drawing;
using System.Net;
using System.IO;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace MyUpdater
{
    public class Updater
    {
        Form uProgress = new Form();
        Label uLabel = new Label();
        ProgressBar uPBar = new ProgressBar();

        //download progress event handler
        private void progressChanged(object sender, DownloadProgressChangedEventArgs e)
        {

            if (e.ProgressPercentage < 100)
                uLabel.Text = "Download " + Convert.ToString(e.ProgressPercentage) + " % completed.";

            uPBar.Value = e.ProgressPercentage;
            uProgress.Update();
        }

        private void progressCompleted(object sender, string uProgramName, string uNewVersion)
        {
            uPBar.Value = 100;
            uLabel.Text = "Done. Restarting...";
            uProgress.Update();

            Process uP = new Process();
            uP.StartInfo.FileName = uProgramName + uNewVersion + ".exe";
            uP.StartInfo.Arguments = "-d:" + Application.ExecutablePath;
            uP.Start();
            Application.Exit();
        }

        
        private string uCheckVersion(string uLocator, string uCurrVersion)
        {
            string uVersionOnline = "";

            //read current online version from the web
            try
            {
                //try to connect: http://server/folder/curr_vers.txt
                //inside txt file: version number
                WebClient uWebClient = new WebClient();
                uWebClient.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                Stream uStream = uWebClient.OpenRead(uLocator + "curr_vers.txt");
                StreamReader uStreamReader = new StreamReader(uStream);

                uVersionOnline = uStreamReader.ReadToEnd();
                uStreamReader.Close();
            }
            catch (WebException uWebEx)
            {
                //connection failed
                uLabel.Text = "Cannot reach server: " + uWebEx.Status;
                uPBar.Value = 100;
                uProgress.Update();
                System.Threading.Thread.Sleep(1000);
                return "";
            }

            //online version is newer?
            if (Convert.ToDouble(uVersionOnline) > Convert.ToDouble(uCurrVersion)) return uVersionOnline;
            else
            return "";
        }

        public Updater(Form uForm, string uLocator, string uProgramName, string uVersionInfo, string uReplaceFile)
        {
            //uReplaceFile is empty => check for new version and download if necessary
            if (uReplaceFile == "") 
            {
                //create update form
                uLabel.Size = new Size(400,20);
                uLabel.Text = "Checking for Updates...";
                uLabel.Location = new Point(10, 10);

                uPBar.Size = new Size(480, 30);
                uPBar.Location = new Point(10, 40);
                uPBar.Minimum = 1;
                uPBar.Maximum = 100;
                uPBar.Value = 1;


                uProgress.FormBorderStyle = FormBorderStyle.None;
                uProgress.Text = "Updater";
                uProgress.SetBounds(0, 0, 500, 80);
                uProgress.StartPosition = FormStartPosition.CenterScreen;

                uProgress.Controls.Add(uLabel);
                uProgress.Controls.Add(uPBar);

                uProgress.Show();
                uProgress.Update();
                
                //check the version
                string uVersionOnline = uCheckVersion(uLocator, uVersionInfo);
                if (uVersionOnline != "")
                {
                    uPBar.Value = 30;
                    uProgress.Update();
                    //new update exists, ask for install
                    if (MessageBox.Show("New update available. Download & Install?", "Update", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        //close form and exit
                        uProgress.Close();
                        return;
                    }

                    uForm.WindowState = FormWindowState.Minimized;

                    //yes, user wants to install it. Begin download
                    uLabel.Text = "Downloading ";
                    uProgress.Update();

                    Uri uUri = new Uri(uLocator + uProgramName + uVersionOnline + ".exe");
                    WebClient uDownloadClient = new WebClient();
                    uDownloadClient.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);

                    //add download progress event handlers
                    uDownloadClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(progressChanged);

                    uDownloadClient.DownloadFileCompleted += new AsyncCompletedEventHandler((EventHandler)delegate(object sender, EventArgs args)
                    {
                        progressCompleted(sender, uProgramName, uVersionOnline);
                    });

                    //does file exist?

                    WebRequest request = WebRequest.Create(uUri);
                    request.Method = "HEAD";

                    try
                    {
                        WebResponse response = request.GetResponse();
                    }
                    catch (WebException uWebEx)
                    {
                        //connection failed
                        uLabel.Text = "Cannot reach server: " + uWebEx.Status;
                        uPBar.Value = 100;
                        uProgress.Update();
                        System.Threading.Thread.Sleep(1000);
                        uProgress.Close();
                        uForm.WindowState = FormWindowState.Normal;
                        return;
                    }

/*                    using ()
                    {
                        MessageBox.Show("here:" +response.ContentLength + "-" + response.ContentType);
                    }*/

                    //test if the file already exists in the folder and delete it...
                    System.IO.FileInfo uFile = new System.IO.FileInfo(uProgramName + uVersionOnline + ".exe");
                    try
                    {
                        uFile.Delete();
                    }
                    catch //(System.IO.IOException e)
                    {
                        MessageBox.Show(uProgramName + uVersionOnline + ".exe already exists in folder. Please delete it first.", "Update Error");
                        //uLabel.Text = ;
                        uPBar.Value = 100;
                        uProgress.Update();
                        //System.Threading.Thread.Sleep(2000);
                        uProgress.Close();
                        uForm.WindowState = FormWindowState.Normal;
                        return;
                    }

                    //try downloading the file now
                    uDownloadClient.DownloadFileAsync(uUri, uProgramName + uVersionOnline + ".exe");
                }
                else
                //no new update available
                {
                    uLabel.Text = "Program is up to date.";
                    uPBar.Value = 100;
                    uProgress.Update();
                    System.Threading.Thread.Sleep(1000);
                    uProgress.Close();
                }
            }
            else
            //uEeplaceFile contains a filename => new version has been downloaded. Install it.
            {
                System.Threading.Thread.Sleep(1000); 
                System.IO.FileInfo fi = new System.IO.FileInfo(@uReplaceFile);
                System.IO.FileInfo fa = new System.IO.FileInfo(@"SpineMiner.exe");
                try
                {
                    fi.Delete();
                    fa.Delete();
                }
                catch (System.IO.IOException e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }
        
    }
    
}