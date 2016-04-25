using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace EucaServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.Resize += (sender, args) =>
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    Hide();
                    //notifyIcon1.Visible = true;

                }
            };
        }

        TcpWorker worker = new TcpWorker();

        private void button1_Click(object sender, EventArgs e)
        {
            worker.Start(1671);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            worker.SendToAll("test");
        }
    }


  public  class TcpWorker
    {
        public static bool IsStart = false;
        public static bool IsConnected = false;
        public static string LastMode = "e";
        Thread ListenerThread, SocketThread;
        TcpListener listener;
        TcpAuthClient client;
     public   List<TcpAuthClient> lst = new List<TcpAuthClient>();
        byte[] myReadBuffer = new byte[8128];
        int isBeginTransfer = 0;
        int x = 0, y = 0, w = 0, h = 0;
        private NetworkStream netStream;
        public Queue<string> CommandQueue = new Queue<string>();
        public List<IMessageExecutor> Executors = new List<IMessageExecutor>();

        public delegate void Recieve(string name);

        public Recieve OnRecieve;
        //  Bitmap bitmap;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetIpAddress()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start(int port)
        {
            Executors.Add(new LoginExecutor());
            Executors.Add(new SendExecutor());

            listener = new TcpListener(System.Net.IPAddress.Any, port);
            listener.Start();

            SocketThread = new Thread(AcceptClient);
            SocketThread.IsBackground = true;
            SocketThread.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        void AcceptClient()
        {
            while (true)
            {

                // SetText1("Поиск...");
                TcpClient tcpc = listener.AcceptTcpClient();
                IsConnected = true;
                client = new TcpAuthClient(tcpc, TcpAuthClient.GetName());
                lst.Add(new TcpAuthClient(client));
                CommandQueue.Enqueue("Соединение с " + lst[lst.Count - 1].Name + " установлено");
                OnRecieve?.Invoke("Сервер");
                //  SetText1();
                //client = null;
                ParameterizedThreadStart p1 = new ParameterizedThreadStart(DoListen);
                ListenerThread = new Thread(p1);
                ListenerThread.IsBackground = true;
                ListenerThread.Start(client);

                // Получение адреса клиента
                //MessageBox.Show(IPAddress.Parse(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString()).ToString());

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client2"></param>
        void DoListen(object client2)
        {
            TcpAuthClient innerClient = (TcpAuthClient)client2;
            netStream = innerClient.Tcp.GetStream();
            while (true)
            {

                if (netStream.CanRead)
                {

                    int numberOfBytesRead = 0;
                    string str1 = "", str2 = "";

                    try
                    {
                        str1 = innerClient.Name;
                        numberOfBytesRead = netStream.Read(myReadBuffer, 0, myReadBuffer.Length);
                        str2 += Encoding.UTF8.GetString(myReadBuffer, 0, numberOfBytesRead);
                        if (str2.Length > 0)
                        {

                            foreach (string s in str2.Split(new[] { "_e" }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                CommandQueue.Enqueue(s);
                            }

                            while (CommandQueue.Count > 0)
                            {
                                string str = CommandQueue.Dequeue();
                                foreach (IMessageExecutor executor in Executors)
                                    executor.Execute(this, innerClient, str);
                            }



                            OnRecieve?.Invoke(str1);
                            //MessageBox.Show("str2");
                        }//SetText2(str1 + ": " + str2);
                     //  Send(innerClient.Tcp);

                      //  myReadBuffer = new byte[8128];

                    }
                    catch (Exception e)
                    {
                        //SetText1(str1 + " отключился от сервера");
                        lst.Remove(client);
                        break;
                    }

                }
                else
                {
                    MessageBox.Show("Can`t read!!!");
                }

            }
            netStream.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public TcpWorker()
        {
            IsStart = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostString"></param>
        public static void GetIpAddressList(String hostString)
        {
            try
            {
                // Get 'IPHostEntry' object containing information like host name, IP addresses, aliases for a host.
                IPHostEntry hostInfo = Dns.GetHostByName(hostString);
                Console.WriteLine("Host name : " + hostInfo.HostName);
                Console.WriteLine("IP address List : ");
                for (int index = 0; index < hostInfo.AddressList.Length; index++)
                {
                    MessageBox.Show(hostInfo.AddressList[index].ToString());
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException caught!!!");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException caught!!!");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught!!!");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="msg"></param>
        public void Send(TcpClient client, string msg = "")
        {
            netStream = client.GetStream();

            if (netStream.CanWrite)
            {
                byte[] buf;
                if (msg.Length == 0) buf = Encoding.UTF8.GetBytes(msg);
                else buf = Encoding.UTF8.GetBytes(msg);
                netStream.Write(buf, 0, buf.Length);
                //   netStream.Flush();
            }
            else
            {
                //SetText1("Error!!!");
            }
        }


        public void SendToAll(string msg)
        {
            foreach (TcpAuthClient authClient in lst)
            {
                Send(authClient.Tcp, msg);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        string BufferToMessage(string[] buffer)
        {
            string ret = "speechbuffer";
            for (int i = 0; i < buffer.Length; i++) ret += "_" + buffer[i];
            return ret;
        }

    }

    public  class TcpAuthClient
    {
        private TcpClient tcp;
        private string name;
        private static int client = 0;
        private string info;

        public string Address
        {
            get { return info; }
            set { info = value; }
        }

        public TcpClient Tcp
        {
            get { return tcp; }
            set { tcp = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public TcpAuthClient(TcpClient tcp, string name)
        {
            this.tcp = tcp;
            this.name = name;
            this.Address = "";
        }

        public TcpAuthClient(TcpAuthClient ctpaClient)
        {
            this.name = ctpaClient.Name;
            this.tcp = ctpaClient.Tcp;
            this.Address = "";
        }

        public static string GetName()
        {
            client++;
            return "tcpclient" + client;

        }



    }


    class Md5Encryptor
    {
        public static string GetHash(string input)
        {
            MD5 md5Hash = MD5.Create();
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.

        public static bool VerifyMd5Hash(string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetHash(input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class DataBaseParameter
    {
        public string Name,
            Value;

        public DataBaseParameter(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }


    /// <summary>
    /// Интерфейс подключения к базе данных
    /// </summary>
    public interface IDataBaseConnector
    {
        /// <summary>
        /// Путь к серверу
        /// </summary>
        string FullPath { get; }

        /// <summary>
        /// Инициализация базы данных
        /// </summary>
        /// <param name="serverPath">полный физический путь к сайту</param>
        void Initialize(string serverPath);

        /// <summary>
        /// Открывает подключение
        /// </summary>
        /// <returns></returns>
        bool Open();

        /// <summary>
        /// Закрывает подключение
        /// </summary>
        /// <returns></returns>
        bool Close();

        /// <summary>
        /// Выборка из базы данных
        /// </summary>
        /// <param name="table">таблица</param>
        /// <param name="parameters">стобцы для вывода</param>
        /// <param name="condition">условие (писать с WHERE) </param>
        /// <returns></returns>
        List<string[]> Select(string table, DataBaseParameter[] parameters, string condition = "");

        /// <summary>
        /// Update метод
        /// </summary>
        /// <param name="table"></param>
        /// <param name="parameters"></param>
        /// <param name="condition"></param>
        void Update(string table, DataBaseParameter[] parameters, string condition = "");

        /// <summary>
        /// Delete метод
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition"></param>
        void Delete(string table, string condition = "");

        /// <summary>
        /// Insert метод
        /// </summary>
        /// <param name="table"></param>
        /// <param name="parameters"></param>
        void Insert(string table, DataBaseParameter[] parameters);

        /// <summary>
        /// Count метод
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        int Count(string table, string condition = "");
    }


    /// <summary>
    /// MySql коннектор к базе
    /// </summary>
    class MySqlConnector : IDataBaseConnector
    {
        private MySqlConnection _connection;


        public MySqlConnector(string connection)
        {
            Initialize(connection);
        }

        public string FullPath { get; private set; }

        public void Initialize(string connection)
        {
            try
            {
               // var connectionString = Configurator.GetDataBaseConnection(serverPath);
                _connection = new MySqlConnection(connection);
            }

            catch (Exception ex)
            {
               ////  Log.Write(FullPath, "MySqlConnector, ошибка первичной инициализации, код: " + ex.Message);
            }
        }

        //open connection to database
        public bool Open()
        {
            try
            {
                _connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {

                //Log.Write(FullPath, "MySqlException, ошибка при открытии соединения, код: " + ex.Number);
                return false;
            }
        }

        //Close connection
        public bool Close()
        {
            try
            {
                _connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {

               //  Log.Write(FullPath, "MySqlException, ошибка при закрытии соединения, код: " + ex.Number);
                return false;
            }
        }

        //Insert statement 
        public void Insert(string table, DataBaseParameter[] parameters)
        {
            try
            {
                string query = "";
                query += "INSERT INTO " + table + " (";

                for (int i = 0; i < parameters.Length - 1; i++)
                    query += "" + parameters[i].Name + " ,";
                query += "" + parameters[parameters.Length - 1].Name + "";

                query += ") VALUES (";

                for (int i = 0; i < parameters.Length - 1; i++)
                    query += "" + parameters[i].Value + " ,";
                query += "" + parameters[parameters.Length - 1].Value + "";

                query += ");";
                this.Close();
                File.AppendAllText(FullPath + "/App_Data/ee.txt", query + "\n");
                //open connection
                if (this.Open() == true)
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, _connection);

                    //Execute command
                    cmd.ExecuteNonQuery();

                    //close connection
                    this.Close();
                }
            }


            catch (Exception ex)
            {
               //  Log.Write(FullPath, "MySqlConnector, ошибка вставки, код: " + ex.Message);
            }
        }

        //Update statement
        public void Update(string table, DataBaseParameter[] parameters, string condition = "")
        {
            try
            {
                if (parameters == null || parameters.Length == 0 || condition == null || table == null) return;
                string query = "";
                query += "UPDATE " + table;
                query += " SET ";
                query = parameters.Aggregate(query, (current, p) => current + (p.Name + " = '" + p.Value + "' "));
                query += condition;
                File.AppendAllText(FullPath + "/App_Data/ee.txt", query + "\n");
                //Open connection
                if (this.Open() == true)
                {
                    //create mysql command
                    MySqlCommand cmd = new MySqlCommand();
                    //Assign the query using CommandText
                    cmd.CommandText = query;
                    //Assign the connection using Connection
                    cmd.Connection = _connection;

                    //Execute query
                    cmd.ExecuteNonQuery();

                    //close connection
                    this.Close();
                }
            }

            catch (Exception ex)
            {
               //  Log.Write(FullPath, "MySqlConnector, ошибка обновления, код: " + ex.Message);
            }
        }

        //Delete statement
        public void Delete(string table, string condition = "")
        {
            try
            {
                string query = "DELETE FROM " + table + " " + condition;
                File.AppendAllText(FullPath + "/App_Data/ee.txt", query + "\n");
                if (this.Open() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, _connection);
                    cmd.ExecuteNonQuery();
                    this.Close();
                }
            }

            catch (Exception ex)
            {
               //  Log.Write(FullPath, "MySqlConnector, ошибка удаления, код: " + ex.Message);
            }
        }

        //Select statement
        public List<string[]> Select(string table, DataBaseParameter[] parameters, string condition = "")
        {



            try
            {
                string query = "SELECT * FROM " + table + " " + condition;
                //       File.AppendAllText(FullPath + "/App_Data/ee.txt", query + "\n");
                if (parameters != null)
                {
                    string pa = "";
                    for (int i = 0; i < parameters.Length - 1; i++)
                        pa += parameters[i].Name + ", ";
                    pa += parameters[parameters.Length - 1].Name;
                    query = query.Replace("*", pa);
                }

                //Create a list to store the result

                List<string[]> list = new List<string[]>();

                //Open connection
                if (this.Open() == true)
                {
                    //Create Command
                    MySqlCommand cmd = new MySqlCommand(query, _connection);
                    //Create a data reader and Execute the command
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    //Read the data and store them in the list
                    while (dataReader.Read())
                    {
                        List<string> lst = new List<string>();
                        for (int j = 0; j < dataReader.FieldCount; j++)
                            lst.Add(dataReader.GetString(j));
                        list.Add(lst.ToArray());
                    }

                    //close Data Reader
                    dataReader.Close();

                    //close Connection
                    this.Close();

                    //return list to be displayed
                    return list;
                }

            }

            catch (Exception ex)
            {
               //  Log.Write(FullPath, "MySqlConnector, ошибка выборки, код: " + ex.Message);

            }
            return new List<string[]>();
        }

        //Count statement
        public int Count(string table, string condition = "")
        {


            string query = "SELECT Count(*) FROM " + table + " " + condition;
            int Count = -1;

            //Open Connection
            if (this.Open() == true)
            {
                //Create Mysql Command
                MySqlCommand cmd = new MySqlCommand(query, _connection);

                //ExecuteScalar will return one value
                Count = int.Parse(cmd.ExecuteScalar() + "");

                //close Connection
                this.Close();

                return Count;
            }
            else
            {
                return Count;
            }
        }


    }


    public interface IMessageExecutor
    {
        void Execute(TcpWorker worker, TcpAuthClient client, string str);
    }

    public class LoginExecutor : IMessageExecutor
    {
        public void Execute(TcpWorker worker, TcpAuthClient client, string str)
        {
            string[] args = str.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);
            if (args[0] == "lg")
            {

                MySqlConnectionStringBuilder conn_string = new MySqlConnectionStringBuilder();
                conn_string.Server = "dresssho.mysql.ukraine.com.ua";
                conn_string.UserID = "dresssho_euca";
                conn_string.Password = "zzylrbfm";
                conn_string.Database = "dresssho_euca";

                string con = "WHERE name = '" + args[1] + "' AND pass = '" + Md5Encryptor.GetHash(args[2]) + "'";

               List<string[]> lst = new MySqlConnector(conn_string.ToString()).Select("users", null, con);

                if (lst.Count == 1)
                {
                    foreach (TcpAuthClient tcpAuthClient in worker.lst)
                    {
                        if (tcpAuthClient.Name == client.Name)
                        {
                            tcpAuthClient.Name = lst[0][1];
                            tcpAuthClient.Address = lst[0][0];
                            client.Name = tcpAuthClient.Name;
                            client.Address = tcpAuthClient.Address;
                        }
                    }
                }
            }
        }


    }

    public class SendExecutor : IMessageExecutor
    {
        public void Execute(TcpWorker worker, TcpAuthClient client, string str)
        {
            string[] args = str.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (args[0] == "sd")
            {
                foreach (TcpAuthClient tcpAuthClient in worker.lst)
                {
                    if (tcpAuthClient.Address == args[1])
                    {

                        foreach (TcpAuthClient authClient in worker.lst)
                        {
                            if (authClient.Name == client.Name)
                            {
                                worker.Send(tcpAuthClient.Tcp, "rc;" + authClient.Address + ";" + args[2]);
                                return;
                            }
                           
                        }

                     
                    }
                }
               
            }
        }


    }

}
