using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hik.Communication.Scs.Communication.Messages;

namespace Eucalyptus
{

    public interface IQuerySender
    {
        void Execute(EucaClient client, string str);
    }

    public class SendMessageQuerySender : IQuerySender
    {

        public static long MaxFileLength = 1000000;

        public static string Upload(string file, string extension)
        {
            if (!new FileInfo(file).Exists) return "-1";
            if (new FileInfo(file).Length > MaxFileLength) return "-2";
            string name = (DateTime.Now - DateTime.MinValue).TotalMilliseconds.ToString().Replace(",", String.Empty);
            string path = "ftp://dresssho.ftp.ukraine.com.ua/dress-shop.com.ua/www/eucalyptus/" + name + extension;
            using (WebClient wclient = new WebClient())
            {
                wclient.Credentials = new NetworkCredential("dresssho_eucalyptus", "NastyaCoala");
                wclient.UploadFile(path, "STOR", file);
            }
            return path;
        }

        public void Execute(EucaClient client, string str)
        {
            if (client.DialogSenderAddress != -1)
            {
               string path = Upload("demo.wav", ".wav");
                if(path == "-1") new SpeechSynthesizer().Speak("Не могу найти такой файл");
                else if(path == "-2") new SpeechSynthesizer().Speak("Слишком большой файл");
                else client.Client.SendMessage(new ScsTextMessage("sd;" + client.DialogSenderAddress + ";" + path +";_e"));         
            }
            else new SpeechSynthesizer().Speak("Выберите получателя");
        }
    }

    public class MyAddressQuerySender : IQuerySender
    {
        public void Execute(EucaClient client, string str)
        {
            if (str.Contains("адрес"))
            {
                SpeechSynthesizer speech = new SpeechSynthesizer();
                speech.Speak("Ваш адрес. " + client.Address);
            }
        }
    }

    public class AddFriendQuerySender : IQuerySender
    {

        public static string Name = "";

        public void Execute(EucaClient client, string str)
        {
            if (str.Contains("добав") && str.Contains("контакт"))
            {
                try
                {
                    string[] args = str.Substring(str.IndexOf("контакт") + "контакт".Length)
                   .Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    long ad = Convert.ToInt64(args[1]);
                    Name = args[0];
                    client.Client.SendMessage(new ScsTextMessage("ad;" + ad + ";_e"));
                }
                catch (Exception)
                {
                    SpeechSynthesizer speech = new SpeechSynthesizer();
                    speech.Speak("Не могу добавить этот контакт");
                }

            }
        }
    }

    public class YourNameQuerySender : IQuerySender
    {
        public void Execute(EucaClient client, string str)
        {
            if (str.Contains("твое") && str.Contains("имя"))
            {
                try
                {
                    string name = str.Substring(str.IndexOf("имя") + "имя".Length);
                    DataManager.SetName(name);
                    new SpeechSynthesizer().Speak("Теперь моё имя " + name);

                }
                catch (Exception)
                {
                    new SpeechSynthesizer().Speak("Не могу себя так назвать");
                }

            }
        }
    }

    public class SendFileQuerySender : IQuerySender
    {
        public void Execute(EucaClient client, string str)
        {
            if (str.Contains("отправ") && str.Contains("файл"))
            {
                if (client.DialogSenderAddress != -1)
                {
                    VirtualKeyboard.SendCtrl_C();
                    StringCollection files = Clipboard.GetFileDropList();
                    string filePath = "";

                    foreach (string s in files)
                        filePath = s;
                   
                    string path = SendMessageQuerySender.Upload(filePath, Path.GetExtension(filePath));
                    if (path == "-1") new SpeechSynthesizer().Speak("Не могу найти такой файл");
                    else if (path == "-2") new SpeechSynthesizer().Speak("Слишком большой файл");
                    else
                    {
                        client.Client.SendMessage(new ScsTextMessage("sd;" + client.DialogSenderAddress + ";" + path + ";_e"));
                        new SpeechSynthesizer().Speak("Файл отправлен");
                    }
                }
                else new SpeechSynthesizer().Speak("Выберите получателя");
            }
        }
    }

    public class SetCurrentRecieverQuerySender : IQuerySender
    {
        public void Execute(EucaClient client, string str)
        {
            if (str.Contains("получате"))
            {
                try
                {
                    string name = str.Substring(str.IndexOf("получатель") + "получатель".Length + 1);
                    if (name.Contains(' '))
                        name = name.Substring(0, name.IndexOf(' '));
                    long id = DataManager.GetAddressByName(name);
                    if (id != -1)
                    {
                        client.DialogSenderAddress = id;
                        new SpeechSynthesizer().Speak("Диалог с пользователем " + name);
                    }
                    else new SpeechSynthesizer().Speak("Не могу установить получателя");

                }
                catch (Exception)
                {
                    new SpeechSynthesizer().Speak("Не могу установить получателя");
                }

            }
        }
    }
}
