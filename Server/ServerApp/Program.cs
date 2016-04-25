using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Hik.Communication.Scs.Client;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Server;
using MySql.Data.MySqlClient;

/* This program is build to demonstrate a server application that listens incoming
 * client connections and reply messages.
 */

namespace ServerApp
{
    class Program
    {

      public static List<TcpAuthClient> AuthClients = new List<TcpAuthClient>();
        public static List<IScsServerClient> Clients = new List<IScsServerClient>();
        public static List<IMessageExecutor> Executors = new List<IMessageExecutor>(); 


        static void Main()
        {
            Executors.Add(new LoginExecutor());
            Executors.Add(new SendExecutor());
            Executors.Add(new AddFriendExecutor());

            //Create a server that listens 10085 TCP port for incoming connections
            var server = ScsServerFactory.CreateServer(new ScsTcpEndPoint(10085));

            //Register events of the server to be informed about clients
            server.ClientConnected += Server_ClientConnected;
            server.ClientDisconnected += Server_ClientDisconnected;

            server.Start(); //Start the server

            Console.WriteLine("Server is started successfully. Press enter to stop...");
            Console.ReadLine(); //Wait user to press enter

            server.Stop(); //Stop the server
        }

        static void Server_ClientConnected(object sender, ServerClientEventArgs e)
        {
            Console.WriteLine("A new client is connected. Client Id = " + e.Client.ClientId);
            Clients.Add(e.Client);
            AuthClients.Add(new TcpAuthClient(e.Client.ClientId));
            //Register to MessageReceived event to receive messages from new client
            e.Client.MessageReceived += Client_MessageReceived;
        }

        static void Server_ClientDisconnected(object sender, ServerClientEventArgs e)
        {
            Console.WriteLine("A client is disconnected! Client Id = " + e.Client.ClientId);
            Clients.Remove(e.Client);
        }

        static void Client_MessageReceived(object sender, MessageEventArgs e)
        {

            //Get a reference to the client
            var client = (IScsServerClient)sender;
            var message = e.Message as ScsTextMessage;

            if (message == null) return;

            string text = message.Text;

          
            foreach (string str in text.Split(new[] {"_e"}, StringSplitOptions.RemoveEmptyEntries))
                foreach (IMessageExecutor messageExecutor in Executors)
                    messageExecutor.Execute(client, str);
                
            

            //Send reply message to the client
            //client.SendMessage(
            //    new ScsTextMessage(
            //        "Hello client. I got your message (" + message.Text + ")",
            //        message.MessageId //Set first message's id as replied message id
            //        ));
        }
    }

    public class TcpAuthClient
    {
        public string Name;
        public long Id;
        public long Address;

        public TcpAuthClient(long id)
        {
            Id = id;
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
        void Execute(IScsServerClient client, string str);
    }

    public class LoginExecutor : IMessageExecutor
    {
        public void Execute(IScsServerClient client, string str)
        {
            string[] args = str.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (args[0] == "lg")
            {
                MySqlConnectionStringBuilder conn_string = new MySqlConnectionStringBuilder();
                conn_string.Server = "dresssho.mysql.ukraine.com.ua";
                conn_string.UserID = "dresssho_euca";
                conn_string.Password = "zzylrbfm";
                conn_string.Database = "dresssho_euca";

                string con = " WHERE name = '" + args[1] + "' AND pass = '" + Md5Encryptor.GetHash(args[2]) + "'";

                List<string[]> lst = new MySqlConnector(conn_string.ToString()).Select("users", null, con);

                if (lst.Count == 1)
                {
                    TcpAuthClient authClient = Program.AuthClients.FirstOrDefault(c => c.Id == client.ClientId);
                    if (authClient != null)
                    {
                        authClient.Address = Convert.ToInt64(lst[0][0]);
                        authClient.Name = lst[0][1];
                        client.SendMessage(new ScsTextMessage("lg;"+authClient.Address + ";_e"));
                        return;
                    }

                }
                client.SendMessage(new ScsTextMessage("lg;-1;_e"));
            }
        }


    }

    public class SendExecutor : IMessageExecutor
    {
        public void Execute(IScsServerClient client, string str)
        {
            string[] args = str.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (args[0] == "sd")
            {
                TcpAuthClient authClient = null;
                foreach (TcpAuthClient tcpAuthClient in Program.AuthClients)
                    if (tcpAuthClient.Address == Convert.ToInt64(args[1])) authClient = tcpAuthClient;

                if (authClient != null)
                {
                    var currentClient = Program.Clients.FirstOrDefault(a => a.ClientId == authClient.Id);
                    currentClient?.SendMessage(new ScsTextMessage("rc;" + authClient.Address + ";" + args[2]));
                }


            }
        }


    }

    public class AddFriendExecutor : IMessageExecutor
    {
        public void Execute(IScsServerClient client, string str)
        {
            string[] args = str.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            if (args[0] == "ad")
            {
                try
                {
                    long id = long.Parse(args[1]);

                    MySqlConnectionStringBuilder conn_string = new MySqlConnectionStringBuilder();
                    conn_string.Server = "dresssho.mysql.ukraine.com.ua";
                    conn_string.UserID = "dresssho_euca";
                    conn_string.Password = "zzylrbfm";
                    conn_string.Database = "dresssho_euca";

                    string con = "WHERE id = " + id;

                    List<string[]> lst = new MySqlConnector(conn_string.ToString()).Select("users", null, con);

                    if (lst.Count == 1) client.SendMessage(new ScsTextMessage("ad;"+id+";_e"));
                    else client.SendMessage(new ScsTextMessage("ad;-1;_e"));



                }
                catch (Exception)
                {
                    client.SendMessage(new ScsTextMessage("ad;-1;_e"));
                }
            }
        }
    }
}
