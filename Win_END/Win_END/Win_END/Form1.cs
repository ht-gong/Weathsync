using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Win_END.Models;

namespace Win_END
{
    public partial class frmDataselect : Form
    {
        OleDbConnection Conn;
        OleDbDataAdapter Adapter;
        OleDbCommand Command;
        DataTable Table;
        private static System.Timers.Timer aTimer;
        private static System.Windows.Forms.Timer recTimer;
        private static TimeSpan elapsed;

        string glbFolderPath;
        string [] fileDirectories;
        public frmDataselect()
        {
            InitializeComponent();
            aTimer = new System.Timers.Timer();
            recTimer = new System.Windows.Forms.Timer();

            recTimer.Interval = 1000;
            aTimer.Interval = TimeSpan.FromMinutes(10).TotalMilliseconds;
            // 
            aTimer.Elapsed += btnAnalysis_Click;
            // 
            aTimer.AutoReset = true;
            recTimer.Tick += RecTimer_Tick;
            elapsed = new TimeSpan();

            this.Resize += Form1_Resize;
        }

        private void RecTimer_Tick(object sender, EventArgs e)
        {
            
            elapsed += TimeSpan.FromSeconds(1);
            label3.Text = String.Format("{0:00}:{1:00}:{2:00}:{3:00}",
            elapsed.Days, elapsed.Hours, elapsed.Minutes, elapsed.Seconds);
        }

        private void btnSelectPath_Click(object sender, EventArgs e)
        {
            DialogResult res = folderBrowserDialog.ShowDialog();
            {
                if(res == DialogResult.OK)
                {
                    string path = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    txtSelectedPath.Text = folderBrowserDialog.SelectedPath;
                    glbFolderPath = folderBrowserDialog.SelectedPath;
                    StreamWriter writer = new StreamWriter(path + "\\GeoChemAPPfilepath.txt");
                    writer.Write(glbFolderPath);
                    writer.Close();
                    writer.Dispose();
                }
            }
        }

        private void btnAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                fileDirectories = System.IO.Directory.GetFiles(glbFolderPath, "*.xlsx");        //寻找所有文件 -- xls?
                foreach (var item in fileDirectories)
                {
                    if (!checkfileread(item))
                    {
                        Initialize(item);
                        writefileread(Path.GetFileName(item));
                    }
                }
            }

            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "寻找文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Initialize(string fileDir) //初始化
        {

            try
            {
                var connstr = @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + fileDir + "; " + "Extended Properties=\"Excel 12.0 Xml;HDR=YES;\"";
                Conn = new OleDbConnection(connstr);
                Conn.Open();
                DataTable dtTemp;
                string datatype;
                datatype = finddatatype(Path.GetFileName(fileDir));

                if (datatype == null)
                    throw new Exception("无法确认表格检测种类！");

                dtTemp = Conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                if (dtTemp == null)
                {
                    return;
                }

                String[] excelSheets = new string[dtTemp.Rows.Count];
                int i = 0;

                foreach (DataRow row in dtTemp.Rows)
                {
                    if (!row["TABLE_NAME"].ToString().Contains("FilterDatabase"))
                    {
                        excelSheets[i] = row["TABLE_NAME"].ToString();
                        i++;
                    }
                }

                dtTemp.Dispose();

                // Loop through all of the sheets
                for (int j = 0; j < i; j++)
                {
                    try
                    {
                        Command = new OleDbCommand("Select * from [" + excelSheets[j] + "]", Conn);
                        Adapter = new OleDbDataAdapter();
                        Table = new DataTable();
                        Adapter.SelectCommand = Command;
                        Adapter.Fill(Table);
                        string columnTimeName = null;
                        string columnConcName = null;


                        DataTable dtColomn = Conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[]
                        { null,null, excelSheets[j], null });

                        List<string> listColumn = new List<string>();
                        foreach (DataRow row in dtColomn.Rows)
                        {
                            listColumn.Add(row["Column_name"].ToString());
                        }

                        dtColomn.Dispose();
                        bool timefound = false, concfound = false;
                        foreach (string column in listColumn)
                        {

                            var result = findColumnbelong(column);
                            switch (result)
                            {
                                case "Time_Found":
                                    columnTimeName = column;
                                    timefound = true;
                                    break;
                                case "Conc_Found":
                                    columnConcName = column;
                                    concfound = true;
                                    break;
                                default:
                                    break;
                            }
                        }

                        if (timefound == false || concfound == false)
                            throw new Exception("查找时间或者浓度失败！");

                        if (Table.Rows.Count < 2)
                            throw new Exception("没有足够数据以查找时间间隔！");

                        TimeSpan time_interval = new TimeSpan(0, 0, 0);
                        long intervalcount_thatmakesupaday;
                        long intervalcount_thatmakesup8hrs = 1;

                        try
                        {
                            DateTime Value1, Value2;
                            if (DateTime.TryParse(Table.Rows[0][columnTimeName].ToString(), out Value1) && DateTime.TryParse(Table.Rows[1][columnTimeName].ToString(), out Value2))
                                time_interval = Value2 - Value1;

                            if (time_interval > TimeSpan.FromDays(1) || time_interval == TimeSpan.FromDays(0))
                                throw new Exception("时间间隔有误！");

                            if (TimeSpan.FromDays(1).Ticks % time_interval.Ticks != 0)
                                throw new Exception("时间间隔不能以24小时进行区分！");

                            if (datatype == "O3")
                            {
                                if (TimeSpan.FromHours(8).Ticks % time_interval.Ticks != 0)
                                    throw new Exception("时间间隔不能以8小时进行区分！");
                                intervalcount_thatmakesup8hrs = TimeSpan.FromHours(8).Ticks / time_interval.Ticks;
                            }

                            intervalcount_thatmakesupaday = TimeSpan.FromDays(1).Ticks / time_interval.Ticks;

                        }
                        catch (Exception)
                        {

                            throw;
                        }

                        if (Table.Rows.Count < intervalcount_thatmakesupaday)
                            throw new Exception("数据不足以求出日均！");

                        int rowCount = 1;
                        double day_sum = 0;
                        Datapoint prev = new Datapoint()
                        {
                            Concentration = 0,
                            Continued_time = new TimeSpan(0),
                            IsHighPollution = false,
                            IsMonitorError = false,
                            Time = new DateTime(0),
                            Monitor_Error = "",
                            Pollution_Level = ""
                        };
                        Datapoint current = new Datapoint()
                        {
                            Concentration = 0,
                            Continued_time = new TimeSpan(0),
                            IsHighPollution = false,
                            IsMonitorError = false,
                            Time = new DateTime(0),
                            Monitor_Error = "",
                            Pollution_Level = ""
                        };

                        List<double> day_elements = new List<double>();

                        foreach (DataRow row in Table.Rows)
                        {
                            string problems_found = "";
                            string pollution_level = "";
                            current.Concentration = (double)row[columnConcName];
                            DateTime result = new DateTime();

                            if (row[columnTimeName].GetType() == typeof(DateTime))
                                result = (DateTime)row[columnTimeName];
                            else
                            {
                                if (!DateTime.TryParse((string)row[columnTimeName], out result))
                                    throw new Exception(string.Format("无法分析第{0}行的时间数据", rowCount));

                            }
                            current.Time = result;

                            if (rowCount < intervalcount_thatmakesupaday)
                            {
                                day_sum += (double)row[columnConcName];
                                if (datatype == "O3")
                                {
                                    day_elements.Add((double)row[columnConcName]);
                                }
                            }
                            else if (rowCount > intervalcount_thatmakesupaday)
                            {
                                day_sum -= (double)Table.Rows[rowCount - 1 - (int)intervalcount_thatmakesupaday][columnConcName];
                                day_sum += (double)row[columnConcName];

                                if (datatype == "O3")
                                {
                                    day_elements.RemoveAt(0);
                                    day_elements.Add((double)row[columnConcName]);
                                    pollution_level = O3avg_results(day_elements.OrderByDescending(item => item).Take((int)intervalcount_thatmakesup8hrs).Sum()/intervalcount_thatmakesup8hrs, (double)row[columnConcName]);
                                }
                                else
                                    pollution_level = calculateAvgandPollutionlvl((double)row[columnConcName], day_sum, intervalcount_thatmakesupaday, datatype);

                            }
                            else
                            {

                                if (datatype == "O3")
                                    pollution_level = O3avg_results(day_elements.OrderByDescending(item => item).Take((int)intervalcount_thatmakesup8hrs).Sum()/intervalcount_thatmakesup8hrs, (double)row[columnConcName]);
                                else
                                    pollution_level = calculateAvgandPollutionlvl((double)row[columnConcName], day_sum, intervalcount_thatmakesupaday, datatype);
                            }

                            var monitorinfo = examineMonitorErrors((double)row[columnConcName], datatype);
                            if (monitorinfo != null)
                                problems_found += monitorinfo;

                            if (problems_found == "")
                            {
                                current.IsMonitorError = false;
                                current.Monitor_Error = "";
                            }
                            else
                            {
                                current.IsMonitorError = true;
                                current.Monitor_Error = problems_found;
                            }
                            current.Pollution_Level = pollution_level;
                            if (pollution_level == "Intermidiate Pollution" || pollution_level == "Heavy Pollution" || pollution_level == "Severe Pollution")
                            {
                                current.IsHighPollution = true;

                            }

                            else
                            {
                                current.IsHighPollution = false;
                                current.Pollution_Level = "";
                            }

                            //Console.WriteLine(string.Format("{0},{1},{2},{3},{4}", prev.Concentration, prev.Time, prev.Continued_time, prev.Pollution_Level, prev.Monitor_Error));
                            current.Field = datatype;

                            if (current.Equals(prev))
                            {
                                current.Mergewith(prev, time_interval);
                            }
                            else if (!current.Equals(prev) && prev.isProblematic())
                            {
                                ReportSQL(prev);

                                //Console.WriteLine(string.Format("{0},{1},{2},{3},{4}", prev.Concentration, prev.Time, prev.Continued_time, prev.Pollution_Level, prev.Monitor_Error));


                            }

                            else
                            {

                            }
                            prev.Copy(current);
                            current.Reset();
                            rowCount++;


                        }


                        //Disposeof();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(String.Format("在表格{0}内的{1}表单，{2}", fileDir, excelSheets[j], ex.Message), "分析文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    Command.Dispose();
                    Adapter.Dispose();
                    Table.Dispose();
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("在表格{0}内，{1}", fileDir, ex.Message), "分析文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
            Conn.Close();
            Conn.Dispose();

           

        }


        private void Disposeof()
        {
            if (Command != null)
                Command.Dispose();
            if (Adapter != null)
                Adapter.Dispose();
            if (Table != null)
                Table.Dispose();
            return;
        }

        private void Frm_Load(object sender, EventArgs e)
        {
            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (!File.Exists(path+ "\\GeoChemAPPreadtexts.txt"))
            {
                File.Create(path + "\\GeoChemAPPreadtexts.txt").Close();
            }

            string path2 = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (!File.Exists(path + "\\GeoChemAPPfilepath.txt"))
            {
                File.Create(path + "\\GeoChemAPPfilepath.txt").Close();
            }

            if(new FileInfo(path+ "\\GeoChemAPPfilepath.txt").Length > 0)
            {
                glbFolderPath = File.ReadAllLines(path + "\\GeoChemAPPfilepath.txt")[0];
                txtSelectedPath.Text = glbFolderPath;
            }
            if(Properties.Settings.Default.Help_Show == true)
            {
                var form2 = new ChildForm();
                form2.Show();
                form2.TopMost = true;
            }
        }

        private bool checkfileread(string check_path)
        {
            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string check_name = Path.GetFileName(check_path);
            foreach (string line in File.ReadAllLines(path + "\\GeoChemAPPreadtexts.txt")) 
            {
                if (line.Contains(check_name))
                    return true;
            }
            
            return false;
        }

        private string findColumnbelong(string check_column)
        {
            if (check_column.Contains("时间") || check_column.Contains("Time") || check_column.Contains("日期"))
                return "Time_Found";
            else if (check_column.Contains("浓度") || check_column.Contains("Conc"))
                return "Conc_Found";
            else
                return null;
           
        }
        private void writefileread(string check_path)
        {
            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string check_name = Path.GetFileName(check_path);
            TextWriter writer = File.AppendText(path + "\\GeoChemAPPreadtexts.txt");
            writer.WriteLine(check_name);
            writer.Close();
            writer.Dispose();
        }

        private string examineMonitorErrors(double conc, string Datatype)
        {
            string problems = null;
            switch(Datatype)
            {
                case "PM2.5":
                    if (conc <= 10)
                        problems += "valuetoosmall";
                    if (conc >= 500)
                        problems += "valuetoobig";
                    break;
                case "PM10":
                    if (conc <= 5)
                        problems += "valuetoosmall";
                    if (conc >= 500)
                        problems += "valuetoobig";
                    break;
                case "SO2":
                    if (conc >= 2000)
                        problems += "valuetoobig";
                    break;
                case "NO2":
                    if (conc <= 10)
                        problems += "valuetoosmall";
                    if (conc >= 2000)
                        problems += "valuetoobig";
                    break;
                case "CO":
                    if (conc >= 100)
                        problems += "valuetoobig";
                    break;
                case "O3":
                    if (conc >= 2000)
                        problems += "valuetoobig";
                    break;
                case "TSP":
                    if (conc <= 10)
                        problems += "valuetoosmall";
                    if (conc >= 3000)
                        problems += "valuetoobig";
                    break;
                case "NOx":
                    if (conc <= 10)
                        problems += "valuetoosmall";
                    if (conc >= 2000)
                        problems += "valuetoobig";
                    break;
                case "BaP":
                    if (conc <= 0.0001)
                        problems += "valuetoosmall";
                    if (conc >= 0.1)
                        problems += "valuetoobig";
                    break;
                default:
                    return null;
            }
            return problems;

        }

        private string finddatatype(string filename)
        {

            if (filename.Contains("PM2.5") || (filename.Contains("颗粒物") && filename.Contains("2.5")))
                return "PM2.5";
            else if (filename.Contains("PM10") || (filename.Contains("颗粒物") && filename.Contains("10")))
                return "PM10";
            else if (filename.Contains("SO2") || filename.Contains("二氧化硫"))
                return "SO2";
            else if (filename.Contains("NO2") || filename.Contains("二氧化氮"))
                return "NO2";
            else if (filename.Contains("CO") || filename.Contains("一氧化碳"))
                return "CO";
            else if (filename.Contains("O3") || filename.Contains("臭氧"))
                return "O3";
            else if (filename.Contains("总悬浮颗粒物") || filename.Contains("TSP"))
                return "TSP";
            else if (filename.Contains("NO") || filename.Contains("氮氧化物"))
                return "NOx";
            else if (filename.Contains("BaP") || filename.Contains("苯并a芘"))
                return "BaP";
            return null;
            
        }

        private string calculateAvgandPollutionlvl(double curr, double sum, double days, string datatype)
        {
            double avg = sum / days;
            switch (datatype)
            {
                case "PM2.5":
                    return PMtwopointfiveavg_results(avg);
                case "PM10":
                    return PMtenavg_results(avg);
                case "SO2":
                    return SO2avg_results(avg,curr);
                case "NO2":
                    return NO2avg_results(avg,curr);
                case "CO":
                    return COavg_results(avg,curr);
                case "TSP":
                    return TSPavg_results(avg);
                case "NOx":
                    return NOxavg_results(avg,curr);
                case "BaP":
                    return BaPavg_results(avg);
                default: return "";
                    
            }
        }

        private string PMtwopointfiveavg_results(double avg_conc)
        {
            if (avg_conc <= 35)
                return "Good";
            else if (avg_conc <= 75)
                return "OK";
            else if (avg_conc <= 115)
                return "Slight Pollution";
            else if (avg_conc <= 150)
                return "Intermidiate Pollution";
            else if (avg_conc <= 250)
                return "Heavy Pollution";
            else if (avg_conc <= 500)
                return "Severe Pollution";
            return "";
        }

        private string PMtenavg_results(double avg_conc)
        {
            if (avg_conc <= 35)
                return "Good";
            else if (avg_conc <= 75)
                return "OK";
            else if (avg_conc <= 150)
                return "Intermidiate Pollution";
            return "Severe Pollution";
        }

        private string NO2avg_results(double avg_conc, double curr_conc)
        {
            if (avg_conc <= 80 && curr_conc <= 200)
                return "OK";
            else if(avg_conc <= 500)
                return "Intermidiate Pollution";
            return "Severe Pollution";
        }
        private string SO2avg_results(double avg_conc, double curr_conc)
        {
            if (avg_conc <= 50 && curr_conc <= 150)
                return "Good";
            else if (avg_conc <= 150 && curr_conc <= 500)
                return "OK";
            else if (avg_conc <= 300)
                return "Intermidiate Pollution";
            return "Severe Pollution";
        }
        private string COavg_results(double avg_conc, double curr_conc)
        {
            if (avg_conc <= 4 && curr_conc <= 10)
                return "OK";
            else if (avg_conc <= 20)
                return "Intermidiate Pollution";
            return "Severe Pollution";
        }
        private string O3avg_results(double avg_conc, double curr_conc)
        {
            if (avg_conc <= 100 && curr_conc <= 160)
                return "Good";
            else if (avg_conc <= 160 && curr_conc <= 200)
                return "OK";
            else if (avg_conc <= 250)
                return "Intermidiate Pollution";
            return "Severe Pollution";
        }
        private string TSPavg_results(double avg_conc)
        {
            if (avg_conc <= 120)
                return "Good";
            else if (avg_conc <= 300)
                return "OK";
            else if (avg_conc <= 600)
                return "Intermidiate Pollution";
            return "Severe Pollution";
        }
        private string NOxavg_results(double avg_conc, double curr_conc)
        {
            if (avg_conc <= 100 && curr_conc <= 250)
                return "OK";
            else if (avg_conc <= 500)
                return "Intermidiate Pollution";
            return "Severe Pollution";
        }

        private string BaPavg_results(double avg_conc)
        {
            if (avg_conc <= 0.0025)
                return "OK";
            else if (avg_conc <= 0.005)
                return "Intermidiate Pollution";
            return "Severe Pollution";
        }
        private void ReportSQL(Datapoint datapoint)
        {
            string cmdString = "INSERT INTO Env_datapoint (Concentration,Time, Continued_Time, IsMonitorError, Monitor_Error,IsHighPollution,Pollution_Level,Field) VALUES (@val1, @val2, @val3, @val4, @val5, @val6, @val7,@val8)";
            string connString = "Server=tcp:geolabtrialserver.database.windows.net,1433;Initial Catalog=GZ_GeoChemDB;Persist Security Info=False;User ID=ht2012;Password=Ipad1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;
                    comm.CommandText = cmdString;
                    comm.Parameters.AddWithValue("@val1", datapoint.Concentration);
                    comm.Parameters.AddWithValue("@val2", datapoint.Time.ToString());
                    comm.Parameters.AddWithValue("@val3", datapoint.Continued_time.ToString());
                    comm.Parameters.AddWithValue("@val4", datapoint.IsMonitorError);
                    comm.Parameters.AddWithValue("@val5", datapoint.Monitor_Error);
                    comm.Parameters.AddWithValue("@val6", datapoint.IsHighPollution);
                    comm.Parameters.AddWithValue("@val7", datapoint.Pollution_Level);
                    comm.Parameters.AddWithValue("@val8", datapoint.Field);
                    try
                    {
                        conn.Open();
                        comm.ExecuteNonQuery();
                    }
                    catch(SqlException e)
                    {
                        throw;
                    }
                }
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {

        }

        private void NotifyIcon_DblClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        private void Maximize_Click(object sender, EventArgs e)
        {
            this.Show();
            WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        private void btnStartSrvc_Click(object sender, EventArgs e)
        {

            if (glbFolderPath == null)
            {
                MessageBox.Show("路径为空", "寻找文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                aTimer.Enabled = true;
                recTimer.Enabled = true;

                btnStopSrvc.Enabled = true;
                btnSelectPath.Enabled = false;
                txtSelectedPath.ReadOnly = true;
                btnStartSrvc.Enabled = false;
                btnClear.Enabled = false;
                StopToolStripMenuItem.Enabled = true;
                label3.ForeColor = Color.Green;

                btnAnalysis_Click(sender, e);
            }
        }

        private void btnStopSrvc_Click(object sender, EventArgs e)
        {
            aTimer.Enabled = false;
            recTimer.Enabled = false;
            btnSelectPath.Enabled = true;
            btnStopSrvc.Enabled = false;
            btnStartSrvc.Enabled = true;
            StopToolStripMenuItem.Enabled = false;
            txtSelectedPath.ReadOnly = false;
            btnClear.Enabled = true;
            label3.ForeColor = Color.Black;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("是否要清空所有记录？","清空",MessageBoxButtons.YesNo,MessageBoxIcon.Error) == DialogResult.Yes)
            {
                string connString = "Server=tcp:geolabtrialserver.database.windows.net,1433;Initial Catalog=GZ_GeoChemDB;Persist Security Info=False;User ID=ht2012;Password=Ipad1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    using (SqlCommand comm = new SqlCommand())
                    {
                        try
                        {
                            conn.Open();
                            comm.Connection = conn;
                            comm.CommandText = "DELETE FROM Env_Datapoint";
                            comm.ExecuteNonQuery();
                        }
                        catch
                        {
                            throw;
                        }
                    }
                    
                }
                string path = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                path += "\\GeoChemAPPreadtexts.txt";
                File.Create(path).Close();
            }
        }

        private void StopToolstrip_Click(object sender, EventArgs e)
        {
            btnStopSrvc_Click(sender, e);
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(aTimer.Enabled == true)
            {
                btnStopSrvc_Click(sender, e);
            }
            this.Close();
        }
    }
}
