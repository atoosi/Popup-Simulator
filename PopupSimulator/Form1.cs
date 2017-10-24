using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PopupSimulator
{

    public partial class Form1 : Form
    {

        Random rnd;
        public char[] browsers;
        bool loop;
        public Form1()
        {
            InitializeComponent();
            rnd = new Random();
            browsers = BrowserChecker();
            textBox1.GotFocus += RemoveText;
            textBox1.LostFocus += AddText;
          
        }


        bool UrlChecker(string url1)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(url1, UriKind.Absolute, out uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return result;
        }


        char[] BrowserChecker()
        {
            try
            {
                RegistryKey browserKeys;
                List<char> paths = new List<char>();
                //on 64bit the browsers are in a different location
                browserKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Clients\StartMenuInternet");
                if (browserKeys == null)
                {
                    browserKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet");

                }
                string[] browserNames = browserKeys.GetSubKeyNames();

                foreach (string browser in browserNames)
                {
                    var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Clients\StartMenuInternet\" + browser + @"\InstallInfo");
                    var path = key.GetValue("ReinstallCommand").ToString();
                    if (path.Contains("iexplore.exe") || browser.ToLower().Contains("iexplore.exe"))
                        paths.Add('i');
                    //if (path.Contains("chrome.exe"))
                    //    paths.Add('c');
                    //if (path.Contains("firefox.exe"))
                    //    paths.Add('f');
                }

                return paths.ToArray();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private async void Popup()
        {
           
            List < string > url = new List<string>();

            string urls = richTextBox1.Text;
            do
            {

                if (urls.IndexOf("\n", 1) == -1)
                {
                    url.Add(urls);
                    urls = "";
                    break;
                }
                url.Add(urls.Substring(0, urls.IndexOf("\n", 1)));
                urls = urls.Substring(urls.IndexOf("\n", 1) + 1);
            } while (urls != "");

            Process[] proc = new Process[url.Count];
            do
            {
              
                do
                {
                    try
                    {

                        Ping myPing = new Ping();
                        String host = "google.com";
                        byte[] buffer = new byte[32];
                        int timeout = 1000;
                        PingOptions pingOptions = new PingOptions();
                        PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                        if (reply.Status == IPStatus.Success&&loop)
                        {
                     
                            int browser;
                            browser = rnd.Next(browsers.Length);

                            for (int i = 0; i < url.Count; i++)
                            {
                                if (!UrlChecker(url[i])) continue;

                                switch (browsers[browser])
                                {
                                    case 'i':
                                        proc[i] = System.Diagnostics.Process.Start("iexplore.exe", url[i]);
                                        // proc[1] = System.Diagnostics.Process.Start("iexplore.exe", "https://www.easysocialmarket.com");
                                        break;
                                    case 'c':

                                        proc[0].StartInfo.FileName = "chrome";
                                        proc[0].StartInfo.Arguments = url[i] + " --new-window";
                                        proc[0].Start();

                                        //proc[1].StartInfo.FileName = "chrome";
                                        //proc[1].StartInfo.Arguments = "https://www.easysocialmarket.com" + " --new-window";
                                        //proc[1].Start();

                                        break;
                                    case 'f':
                                        proc[0] = System.Diagnostics.Process.Start("firefox.exe", url[i]);
                                        //  proc[1] = System.Diagnostics.Process.Start("firefox.exe", "https://www.easysocialmarket.com");
                                        break;
                                }
                            }
                    }

                    break;
                    }
                    catch (Exception e)
                    {
                        await Task.Delay(2000);

                    }
                } while (loop);


             

                //await Task.Delay(120000 + rnd.Next(1, 60) * 1000);

               
                do
                {
                    try
                    {
                        Ping myPing = new Ping();
                        String host = "google.com";
                        byte[] buffer = new byte[32];
                        int timeout = 1000;
                        PingOptions pingOptions = new PingOptions();
                        PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                        
                        //if (reply.Status == IPStatus.Success&&loop)
                        //{
                        //    await Task.Delay(1000);
                            
                        //}
                    }
                    catch (Exception e)
                    {
                        for (int i = 0; i < url.Count; i++)
                        {
                            if (!proc[i].HasExited)
                            {
                                proc[i].Kill();

                            }
                      
                        }

                        break;
               
                    }
                } while (loop);


            } while (loop);

            for (int i = 0; i < url.Count; i++)
            {
                if (!proc[i].HasExited)
                {
                    proc[i].Kill();

                }

            }
        }


        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }


        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = true;
            button3.Enabled = false;
            button4.Enabled = false;

            loop = true;
            Popup();

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = true;
            button4.Enabled = true;

            loop = false;

        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (textBox1.Text != "")
            {
                button1.Enabled = true;
                button4.Enabled = true;
                if (!string.IsNullOrWhiteSpace(richTextBox1.Text))
                {
                    richTextBox1.AppendText("\r\n" + textBox1.Text);
                }
                else
                {
                    richTextBox1.AppendText(textBox1.Text);
                }
            }
        }


        void RemoveText(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        void AddText(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(textBox1.Text))
                textBox1.Text = "https://www.easysocialmarket.com";
        }
      
       
        private void button4_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text!="")
            {

                richTextBox1.Text = "";

                button1.Enabled = false;
                button4.Enabled = false;
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
              
                textBox2.Text = openFileDialog1.FileName;
                using (StreamReader sr = new StreamReader(openFileDialog1.FileName))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (string.IsNullOrEmpty(richTextBox1.Text))
                        {
                            richTextBox1.Text += line;
                        }
                        else { richTextBox1.Text +="\n"+ line; }
                           
                    }
                }
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(richTextBox1.Text))
            {
                button1.Enabled = false;
            }
            else
            {
                button1.Enabled = true;
            }
        }
    }
}
