using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.IO;
using System.Text;

namespace LiveBot
{
    public class DataBase
    {
        //public static NpgsqlConnection Connection { get; set; }
        public static DataTable Warnings = new DataTable();

        public static DataTable User_warnings = new DataTable();
        public static DataTable Reaction_Roles_DT = new DataTable();
        public static DataTable vehicle_list = new DataTable();
        public static DataTable Leaderboard = new DataTable();
        public static DataTable Server_Leaderboard = new DataTable();
        public static DataTable Backgrounds = new DataTable();
        public static DataTable User_Background = new DataTable();
        public static DataTable StreamNotificationSettings = new DataTable();
        public static NpgsqlDataAdapter WarnAdapter = new NpgsqlDataAdapter();
        public static NpgsqlDataAdapter UserWarnAdapter = new NpgsqlDataAdapter();
        public static NpgsqlDataAdapter Reaction_Roles_Adapter = new NpgsqlDataAdapter();
        public static NpgsqlDataAdapter Vehicle_List_Adapter = new NpgsqlDataAdapter();
        public static NpgsqlDataAdapter Leaderboard_Adapter = new NpgsqlDataAdapter();
        public static NpgsqlDataAdapter Server_Leaderboard_Adapter = new NpgsqlDataAdapter();
        public static NpgsqlDataAdapter Background_Adapter = new NpgsqlDataAdapter();
        public static NpgsqlDataAdapter User_Backgrounds_Adapter = new NpgsqlDataAdapter();
        public static NpgsqlDataAdapter Stream_Notification_Adapter = new NpgsqlDataAdapter();

        public static NpgsqlConnection DBConnection()
        {
            string json;
            using (var fs = File.OpenRead("DBCFG.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();
            var cfgjson = JsonConvert.DeserializeObject<DBJson>(json);
            string con = $"Host={cfgjson.Host};Username={cfgjson.Username};Password={cfgjson.Password};Database={cfgjson.Database}";
            return new NpgsqlConnection(con);
        }

        public static void UpdateTables()
        {
            Warnings.Clear();
            User_warnings.Clear();
            WarnAdapter.Fill(Warnings);
            UserWarnAdapter.Fill(User_warnings);
        }

        public static void UpdateVehicleList()
        {
            vehicle_list.Clear();
            Vehicle_List_Adapter.Fill(vehicle_list);
        }

        public static void UpdateLeaderboards(string function)
        {
            if (function == "base")
            {
                Leaderboard_Adapter.Update(Leaderboard);
                Server_Leaderboard_Adapter.Update(Server_Leaderboard);
            }
            else if (function == "table")
            {
                Leaderboard.Clear();
                Server_Leaderboard.Clear();
                Leaderboard_Adapter.Fill(Leaderboard);
                Server_Leaderboard_Adapter.Fill(Server_Leaderboard);
            }
        }

        public static void UpdateBackgroundTable()
        {
            Backgrounds.Clear();
            Background_Adapter.Fill(Backgrounds);
        }

        public static void UpdateUserBackground(string function)
        {
            if (function == "base")
            {
                User_Backgrounds_Adapter.Update(User_Background);
            }
            else if (function == "table")
            {
                User_Background.Clear();
                User_Backgrounds_Adapter.Fill(User_Background);
            }
        }

        public static void UpdateStreamNotifications(string function)
        {
            if (function == "base")
            {
                Stream_Notification_Adapter.Update(StreamNotificationSettings);
            }
            else if (function == "table")
            {
                StreamNotificationSettings.Clear();
                Stream_Notification_Adapter.Fill(StreamNotificationSettings);
            }
        }

        public static void GetReactionRoles()
        {
            NpgsqlConnection Connection = DBConnection();
            Reaction_Roles_DT.Columns.Add("id", typeof(int));
            Reaction_Roles_DT.Columns.Add("role_id", typeof(string));
            Reaction_Roles_DT.Columns.Add("server_id", typeof(string));
            Reaction_Roles_DT.Columns.Add("message_id", typeof(string));
            Reaction_Roles_DT.Columns.Add("reaction_id", typeof(string));
            NpgsqlCommand SelectCMD = new NpgsqlCommand("SELECT id, role_id, server_id, message_id, reaction_id FROM reaction_roles", Connection);
            Reaction_Roles_Adapter.SelectCommand = SelectCMD;
            Reaction_Roles_Adapter.Fill(Reaction_Roles_DT);
        }

        public static void DataBaseStart()
        {
            NpgsqlConnection Connection = DBConnection();
            Warnings.Columns.Add("id_warning", typeof(int));
            Warnings.Columns.Add("reason", typeof(string));
            Warnings.Columns.Add("active", typeof(bool));
            Warnings.Columns.Add("date", typeof(string));
            Warnings.Columns.Add("admin_id", typeof(string));
            Warnings.Columns.Add("user_id", typeof(string));

            User_warnings.Columns.Add("warning_level", typeof(int));
            User_warnings.Columns.Add("warning_count", typeof(int));
            User_warnings.Columns.Add("kick_count", typeof(int));
            User_warnings.Columns.Add("ban_count", typeof(int));
            User_warnings.Columns.Add("id_user", typeof(string));

            NpgsqlCommand WarnSelect = new NpgsqlCommand("select id_warning, reason, active, date, admin_id, user_id from warnings", Connection);
            NpgsqlCommand WarnInsert = new NpgsqlCommand($"insert into warnings (reason, active, date, admin_id, user_id) values (@iemesls, @aktivs, @datums, @admins, @users)", Connection);
            WarnInsert.Parameters.Add(new NpgsqlParameter("@iemesls", DbType.String, 500, "reason"));
            WarnInsert.Parameters.Add(new NpgsqlParameter("@aktivs", DbType.Boolean, sizeof(bool), "active"));
            WarnInsert.Parameters.Add(new NpgsqlParameter("@datums", DbType.String, 500, "date"));
            WarnInsert.Parameters.Add(new NpgsqlParameter("@admins", DbType.String, 500, "admin_id"));
            WarnInsert.Parameters.Add(new NpgsqlParameter("@users", DbType.String, 500, "user_id"));
            NpgsqlCommand WarnUpdate = new NpgsqlCommand("UPDATE warnings SET active=@aktivs where id_warning=@ID", Connection);
            WarnUpdate.Parameters.Add(new NpgsqlParameter("@ID", DbType.Int32, sizeof(int), "id_warning"));
            WarnUpdate.Parameters.Add(new NpgsqlParameter("@aktivs", DbType.Boolean, sizeof(bool), "active"));
            NpgsqlCommand WarnDelete = new NpgsqlCommand("delete from warnings where id_warning=@ID", Connection);
            WarnDelete.Parameters.Add(new NpgsqlParameter("@ID", DbType.Int32, sizeof(int), "id_warning"));
            WarnAdapter.InsertCommand = WarnInsert;
            WarnAdapter.DeleteCommand = WarnDelete;
            WarnAdapter.SelectCommand = WarnSelect;
            WarnAdapter.UpdateCommand = WarnUpdate;

            NpgsqlCommand UserWarnSelect = new NpgsqlCommand("select warning_level, warning_count, kick_count, ban_count, id_user from user_warnings", Connection);
            NpgsqlCommand UserWarnInsert = new NpgsqlCommand($"insert into user_warnings (warning_level, warning_count, kick_count, ban_count, id_user) values (@limenis, @wskaits, @kskaits, @bskaits, @users)", Connection);
            UserWarnInsert.Parameters.Add(new NpgsqlParameter("@limenis", DbType.Int32, sizeof(int), "warning_level"));
            UserWarnInsert.Parameters.Add(new NpgsqlParameter("@wskaits", DbType.Int32, sizeof(int), "warning_count"));
            UserWarnInsert.Parameters.Add(new NpgsqlParameter("@kskaits", DbType.Int32, sizeof(int), "kick_count"));
            UserWarnInsert.Parameters.Add(new NpgsqlParameter("@bskaits", DbType.Int32, sizeof(int), "ban_count"));
            UserWarnInsert.Parameters.Add(new NpgsqlParameter("@users", DbType.String, 500, "id_user"));
            NpgsqlCommand UserWarnUpdate = new NpgsqlCommand("UPDATE user_warnings SET warning_level=@limenis, warning_count=@wskaits, kick_count=@kskaits, ban_count=@bskaits where id_user=@users", Connection);
            UserWarnUpdate.Parameters.Add(new NpgsqlParameter("@limenis", DbType.Int32, sizeof(int), "warning_level"));
            UserWarnUpdate.Parameters.Add(new NpgsqlParameter("@wskaits", DbType.Int32, sizeof(int), "warning_count"));
            UserWarnUpdate.Parameters.Add(new NpgsqlParameter("@kskaits", DbType.Int32, sizeof(int), "kick_count"));
            UserWarnUpdate.Parameters.Add(new NpgsqlParameter("@bskaits", DbType.Int32, sizeof(int), "ban_count"));
            UserWarnUpdate.Parameters.Add(new NpgsqlParameter("@users", DbType.String, 500, "id_user"));
            UserWarnAdapter.InsertCommand = UserWarnInsert;
            UserWarnAdapter.SelectCommand = UserWarnSelect;
            UserWarnAdapter.UpdateCommand = UserWarnUpdate;

            NpgsqlCommand Vehicle_List_Select = new NpgsqlCommand("select brand, model, year, type, discipline_name from vehicle_list as vl left join discipline_list as dl on vl.discipline=dl.id_discipline", Connection);
            Vehicle_List_Adapter.SelectCommand = Vehicle_List_Select;

            NpgsqlCommand Leaderboard_Select = new NpgsqlCommand("select id_user, followers, level, bucks from leaderboard", Connection);
            Leaderboard_Adapter.SelectCommand = Leaderboard_Select;

            NpgsqlCommand Leaderboard_Insert = new NpgsqlCommand("insert into leaderboard (id_user, followers, level, bucks) values (@userid , @follow, @lvl, @bcks)", Connection);
            Leaderboard_Insert.Parameters.Add(new NpgsqlParameter("@userid", NpgsqlDbType.Text, 500, "id_user"));
            Leaderboard_Insert.Parameters.Add(new NpgsqlParameter("@follow", NpgsqlDbType.Bigint, sizeof(long), "followers"));
            Leaderboard_Insert.Parameters.Add(new NpgsqlParameter("@lvl", DbType.Int32, sizeof(int), "level"));
            Leaderboard_Insert.Parameters.Add(new NpgsqlParameter("@bcks", DbType.Int64, sizeof(long), "bucks"));
            Leaderboard_Adapter.InsertCommand = Leaderboard_Insert;

            NpgsqlCommand Leaderboard_Update = new NpgsqlCommand("update leaderboard SET followers=@follow, level=@lvl, bucks=@bcks where id_user=@userid", Connection);
            Leaderboard_Update.Parameters.Add(new NpgsqlParameter("@userid", NpgsqlDbType.Text, 500, "id_user"));
            Leaderboard_Update.Parameters.Add(new NpgsqlParameter("@follow", DbType.Int64, sizeof(long), "followers"));
            Leaderboard_Update.Parameters.Add(new NpgsqlParameter("@lvl", DbType.Int32, sizeof(int), "level"));
            Leaderboard_Update.Parameters.Add(new NpgsqlParameter("@bcks", DbType.Int64, sizeof(long), "bucks"));
            Leaderboard_Adapter.UpdateCommand = Leaderboard_Update;

            Server_Leaderboard.Columns.Add("user_id", typeof(string));
            Server_Leaderboard.Columns.Add("server_id", typeof(string));
            Server_Leaderboard.Columns.Add("followers", typeof(long));

            NpgsqlCommand Server_Leaderboard_select = new NpgsqlCommand("select user_id, server_id, followers from server_ranks", Connection);
            Server_Leaderboard_Adapter.SelectCommand = Server_Leaderboard_select;

            NpgsqlCommand Server_Leaderboard_Insert = new NpgsqlCommand("insert into server_ranks (user_id, server_id, followers) values (@uid, @sid, @follow)", Connection);
            Server_Leaderboard_Insert.Parameters.Add(new NpgsqlParameter("@uid", NpgsqlDbType.Text, 500, "user_id"));
            Server_Leaderboard_Insert.Parameters.Add(new NpgsqlParameter("@follow", DbType.Int64, sizeof(long), "followers"));
            Server_Leaderboard_Insert.Parameters.Add(new NpgsqlParameter("@sid", DbType.String, 500, "server_id"));
            Server_Leaderboard_Adapter.InsertCommand = Server_Leaderboard_Insert;

            NpgsqlCommand Server_Leaderboard_Update = new NpgsqlCommand("update server_ranks SET followers=@follow where user_id=@uid and server_id=@sid", Connection);
            Server_Leaderboard_Update.Parameters.Add(new NpgsqlParameter("@uid", NpgsqlDbType.Text, 500, "user_id"));
            Server_Leaderboard_Update.Parameters.Add(new NpgsqlParameter("@follow", DbType.Int64, sizeof(long), "followers"));
            Server_Leaderboard_Update.Parameters.Add(new NpgsqlParameter("@sid", DbType.String, 500, "server_id"));
            Server_Leaderboard_Adapter.UpdateCommand = Server_Leaderboard_Update;

            NpgsqlCommand Background_Select = new NpgsqlCommand("select * from background_image", Connection);
            Background_Adapter.SelectCommand = Background_Select;

            NpgsqlCommand User_Background_Slect = new NpgsqlCommand("select * from user_images", Connection);
            User_Backgrounds_Adapter.SelectCommand = User_Background_Slect;
            NpgsqlCommand User_Background_Insert = new NpgsqlCommand("insert into user_images (user_id,bg_id) values (@uid,@bgid)", Connection);
            User_Background_Insert.Parameters.Add(new NpgsqlParameter("@uid", NpgsqlDbType.Text, 500, "user_id"));
            User_Background_Insert.Parameters.Add(new NpgsqlParameter("@bgid", NpgsqlDbType.Integer, sizeof(int), "bg_id"));
            User_Backgrounds_Adapter.InsertCommand = User_Background_Insert;

            NpgsqlCommand Stream_Notification_select = new NpgsqlCommand("select * from stream_notification", Connection);
            Stream_Notification_Adapter.SelectCommand = Stream_Notification_select;

            UpdateTables();
            UpdateVehicleList();
            UpdateLeaderboards("table");
            UpdateBackgroundTable();
            UpdateUserBackground("table");
            UpdateStreamNotifications("table");
        }
    }
}