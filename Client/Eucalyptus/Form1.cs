using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Eucalyptus.Properties;
using Hik.Communication.Scs.Client;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.Scs.Communication.Messages;
using NAudio.Wave;

namespace Eucalyptus
{
    public partial class Form1 : Form
    {
        WaveIn waveIn;
        WaveFileWriter writer;
        string outputFilename = "demo.wav";
        bool ON = false;
        public EucaClient EucaClient = new EucaClient();
        private DateTime messageTime;
        private double totalMessageTime = -1;

        public Form1()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
          

            EucaClient.Executors.Add(new LoginExecutor());
            EucaClient. Executors.Add(new RecieveExecutor());
            EucaClient.Executors.Add(new AddFriendExecutor());

            EucaClient.Senders.Add(new MyAddressQuerySender());
            EucaClient.Senders.Add(new AddFriendQuerySender());
            EucaClient.Senders.Add(new YourNameQuerySender());
            EucaClient.Senders.Add(new SetCurrentRecieverQuerySender());
            EucaClient.Senders.Add(new SendFileQuerySender());



            notifyIcon1.Visible = false;
            this.notifyIcon1.MouseDoubleClick += (sender, args) =>
            {
                this.Show();
                notifyIcon1.Visible = false;
                WindowState = FormWindowState.Normal;
            };

            toolStripMenuItem1.Click += (sender, args) => Close();
            this.Resize += (sender, args) =>
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    Hide();
                    notifyIcon1.Visible = true;

                }
            };
        }

        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            try
            {
                writer.WriteData(e.Buffer, 0, e.BytesRecorded);

            }
            catch
            {

            }
        }

        void waveIn_RecordingStopped(object sender, EventArgs e)
        {
            try
            {
                waveIn.Dispose();
                waveIn = null;
                writer.Close();
                writer = null;
            }
            catch
            {

            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            try
            {
                WebRequest request = WebRequest.Create("https://www.google.com/speech-api/v2/recognize?output=json&lang=ru-RU&key=AIzaSyBOti4mM-6x9WDnZIjIeyEU21OpBXqWBgw");
                //
                request.Method = "POST";
                byte[] byteArray = File.ReadAllBytes(outputFilename);
                request.ContentType = "audio/l16; rate=16000"; //"16000";
                request.ContentLength = byteArray.Length;
                request.GetRequestStream().Write(byteArray, 0, byteArray.Length);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string str = reader.ReadToEnd();
                string[] strs = ParseJson(str);


                string phrase = "";
                if (strs.Length > 0) phrase = strs[0].ToLower();

                if (totalMessageTime > 700 &&
                    totalMessageTime < 10000)
                {
                    if (!phrase.Contains(DataManager.GetName())) new SendMessageQuerySender().Execute(EucaClient, phrase);

                    else foreach (IQuerySender querySender in EucaClient.Senders)
                            querySender.Execute(EucaClient, phrase);

                   
                }



                if (totalMessageTime < 700)
                    new SpeechSynthesizer().Speak("Слишком маленькое сообщение");



                if (totalMessageTime > 10000)
                    new SpeechSynthesizer().Speak("Слишком большое сообщение");



                new FileInfo(Environment.CurrentDirectory + "\\demo.wav").Delete();

                totalMessageTime = -1;


                reader.Close();
                response.Close();
                timer1.Enabled = false;


            }

            catch(Exception ex)
            {
               
            }


        }

        private void Timer2Tick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (VirtualInput.GetAsyncKeyState(Keys.F8) && !ON && EucaClient.Address != -1)
                {
                    ON = true;
                    messageTime = DateTime.Now;
                    waveIn = new WaveIn();
                    waveIn.DeviceNumber = 0;
                    waveIn.DataAvailable += waveIn_DataAvailable;
                    waveIn.RecordingStopped +=
                        new EventHandler<NAudio.Wave.StoppedEventArgs>(waveIn_RecordingStopped);
                    waveIn.WaveFormat = new WaveFormat(16000, 1);
                    writer = new WaveFileWriter(outputFilename, waveIn.WaveFormat);
                    // label1.Text = "Идет запись...";
                    waveIn.StartRecording();
                }
                else if (!VirtualInput.GetAsyncKeyState(Keys.F8) && ON && EucaClient.Address != -1 && totalMessageTime < 0)
                {

                  totalMessageTime = (DateTime.Now - messageTime).TotalMilliseconds;



                    waveIn.StopRecording();
                    waveIn.Dispose();
                    writer.Close();
                    writer.Dispose();
                    //  label1.Text = "";
                    ON = false;
                    timer1.Enabled = true;
                 
                }

               
            }
            catch (Exception ex)
            {

            }
        }



        string[] ParseJson(string json)
        {
            List<string> list = new List<string>();
            try
            {
                string[] lines = json.Split(new[] { "\"transcript\":\"" }, StringSplitOptions.RemoveEmptyEntries);


                for (int i = 1; i < lines.Length; i++)
                    list.Add(lines[i].Substring(0, lines[i].IndexOf("\"", StringComparison.Ordinal)));
            }
            catch
            {

            }

            return list.ToArray();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
                MessageBox.Show("Нажмите и удерживайте Home во время произношения", "Программа запущена");
            }

            timer2.Tick += Timer2Tick;
            
            timer2.Interval = 46;
            timer2.Enabled = true;

        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            EucaClient.Client = ScsClientFactory.CreateClient(new ScsTcpEndPoint(textBox4.Text, 10085));
            EucaClient.Client.Connect();
            EucaClient.Client.MessageReceived += ClientOnMessageReceived;
            EucaClient.Client.SendMessage(new ScsTextMessage("lg;" + textBox2.Text + ";" + textBox3.Text + ";_e"));
        }

        private void ClientOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message as ScsTextMessage;
            if (message == null) return;
            string text = message.Text;
            foreach (string str in text.Split(new[] { "_e" }, StringSplitOptions.RemoveEmptyEntries))
                foreach (IMessageExecutor messageExecutor in EucaClient.Executors)
                    messageExecutor.Execute(EucaClient, str);
        }

     
    }


}
