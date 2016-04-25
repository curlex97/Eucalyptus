using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Eucalyptus
{

    public interface IMessageExecutor
    {
        void Execute(EucaClient client, string strs);
    }

    public class LoginExecutor : IMessageExecutor
    {
        public void Execute(EucaClient client, string strs)
        {
            string[] args = strs.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            if (args[0] == "lg")
            {
                try
                {
                    client.Address = long.Parse(args[1]);
                    MessageBox.Show(client.Address + "");
                }
                catch
                {

                }

            }
        }
    }

    public class RecieveExecutor : IMessageExecutor
    {
        public void Execute(EucaClient client, string strs)
        {
            string[] args = strs.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            if (args[0] == "rc")
            {
                try
                {
                    client.LastSenderAddress = long.Parse(args[1]);
                    string path = args[2];
                    string name = args[1];
                    string ext = Path.GetExtension(path);

                    using (WebClient wclient = new WebClient())
                    {
                        wclient.Credentials = new NetworkCredential("dresssho_eucalyptus", "NastyaCoala");
                        wclient.DownloadFile(path, name + ext);
                    }

                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);
                    request.Credentials = new NetworkCredential("dresssho_eucalyptus", "NastyaCoala");
                    request.Method = WebRequestMethods.Ftp.DeleteFile;
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    Console.WriteLine("Delete status: {0}", response.StatusDescription);
                    response.Close();

                    if (ext == ".wav")
                        using (var player = new SoundPlayer(name + ext))
                        {
                            player.PlaySync();
                        }

                    else Process.Start(Environment.CurrentDirectory + "\\" + name + ext);
                }
                catch
                {

                }

            }
        }
    }

    public class AddFriendExecutor : IMessageExecutor
    {
        public void Execute(EucaClient client, string strs)
        {
            string[] args = strs.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            if (args[0] == "ad")
            {
                try
                {
                    if (long.Parse(args[1]) == -1)
                    {
                        SpeechSynthesizer speech = new SpeechSynthesizer();
                        speech.Speak("Не могу добавить этот контакт");
                    }
                    else
                    {
                        DataManager.AddAddress(AddFriendQuerySender.Name, long.Parse(args[1]));
                        SpeechSynthesizer speech = new SpeechSynthesizer();
                        speech.Speak("Контакт добавлен");
                    }
                }
                catch
                {
                    SpeechSynthesizer speech = new SpeechSynthesizer();
                    speech.Speak("Не могу добавить этот контакт");
                }

            }
        }
    }


}
