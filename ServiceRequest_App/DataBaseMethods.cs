using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ServiceRequest_App
{
    public class DataBaseMethods
    {


        //Отправка заявки в базу данных
        static public bool SendRequest(MainWindow winM, Request requestData)
        {
            //Соединение к базе данных.
            MySqlConnection connection = DBUtils.GetDBConnection();
            try
            {
                connection.Open();

                string sql = "Insert into mainrequestinfo ( DateCreate, UserName, Location, Mobile, Email, WithoutMe, problemWithMyPc, WorkTime, Text, Status) "
                                                 + " values (@dateCreate, @userName, @location, @mobile, @email, @withoutMe, @problemWithMyPc, @workTime, @text, @status);";
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.Add("@dateCreate", MySqlDbType.VarChar).Value = requestData.Date;
                cmd.Parameters.Add("@userName", MySqlDbType.VarChar).Value = requestData.UserName;
                cmd.Parameters.Add("@location", MySqlDbType.VarChar).Value = requestData.Location;
                cmd.Parameters.Add("@mobile", MySqlDbType.VarChar).Value = requestData.Mobile;
                cmd.Parameters.Add("@email", MySqlDbType.VarChar).Value = requestData.Email;
                cmd.Parameters.Add("@withoutMe", MySqlDbType.Int32).Value = requestData.WithoutMe;
                cmd.Parameters.Add("@problemWithMyPc", MySqlDbType.Int32).Value = requestData.ProblemWithMyPc;
                cmd.Parameters.Add("@workTime", MySqlDbType.Text).Value = requestData.WorkTime;
                cmd.Parameters.Add("@text", MySqlDbType.Text).Value = requestData.TextOfRequest;
                cmd.Parameters.Add("@status", MySqlDbType.VarChar).Value = requestData.Status;
                cmd.ExecuteNonQuery();
            }
            catch
            {
                MessageBox.Show("Невозможно подключиться к базе данных для отправки заявки, попробуйте повторить попытку позже.");
                return false;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
                connection = null;
            }
            //Отправка информации о пк
            if (requestData.ProblemWithMyPc)
            {
                try
                {
                    SendPcInfo(winM);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Ошибка отправки данных о компьютере: " + e);
                    return false;
                }
            }
            else
            {
                SendPcInfoOnlyMac(winM);
            }
            return true;
        }


        static private void SendPcInfo(MainWindow winM)
        {
            string lasRequestNumber = "";
            //Узнаем ID для вставки
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                string sql = "SELECT max(Last_insert_id(RequestNumber)) from mainrequestinfo;";
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            lasRequestNumber = reader.GetValue(0).ToString();
                        }
                    }
                }
            }
            catch
            {
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            //Отправляем информацию о пк
            MySqlConnection connection = DBUtils.GetDBConnection();
            connection.Open();
            try
            {
                PcInfo pcInfoData = FillInformationAboutPc();
                string json = JsonConvert.SerializeObject(pcInfoData);

                string sql = "Insert into pcinfo ( RequestNumber , IpAddress, MacAddress, HostName, Info, UserPassword) "
                                                 + " values (@requestNumber, @ipAddress, @macAddress, @hostName, @info, @userPassword);";
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.Add("@requestNumber", MySqlDbType.Int32).Value = Convert.ToInt32(lasRequestNumber);
                cmd.Parameters.Add("@ipAddress", MySqlDbType.VarChar).Value = HelperMethods.GetIPv4();
                cmd.Parameters.Add("@macAddress", MySqlDbType.VarChar).Value = HelperMethods.GetBeautiMacAddress();
                cmd.Parameters.Add("@hostName", MySqlDbType.VarChar).Value = Dns.GetHostName();
                cmd.Parameters.Add("@info", MySqlDbType.LongText).Value = json;
                cmd.Parameters.Add("@userPassword", MySqlDbType.VarChar).Value = winM.Password.Text;
                cmd.ExecuteNonQuery();
            }
            catch
            {
            }
            finally
            {
                connection.Close();
                connection.Dispose();
                connection = null;
            }
        }

        static private void SendPcInfoOnlyMac(MainWindow winM)
        {
            string lasRequestNumber = "";
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                string sql = "SELECT max(Last_insert_id(RequestNumber)) from mainrequestinfo;";
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            lasRequestNumber = reader.GetValue(0).ToString();
                        }
                    }
                }
            }
            catch
            {
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            MySqlConnection connection = DBUtils.GetDBConnection();
            connection.Open();
            try
            {
                PcInfo pcInfoData = FillInformationAboutPc();
                string json = JsonConvert.SerializeObject(pcInfoData);
                string sql = "Insert into pcinfo ( RequestNumber , IpAddress, MacAddress, HostName, Info, UserPassword) "
                                                 + " values (@requestNumber, @ipAddress, @macAddress, @hostName, @info, @userPassword);";
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.Add("@requestNumber", MySqlDbType.Int32).Value = Convert.ToInt32(lasRequestNumber);
                cmd.Parameters.Add("@ipAddress", MySqlDbType.VarChar).Value = "";
                cmd.Parameters.Add("@macAddress", MySqlDbType.VarChar).Value = HelperMethods.GetBeautiMacAddress();
                cmd.Parameters.Add("@hostName", MySqlDbType.VarChar).Value = "";
                cmd.Parameters.Add("@info", MySqlDbType.LongText).Value = "";
                cmd.Parameters.Add("@userPassword", MySqlDbType.VarChar).Value = "";
                cmd.ExecuteNonQuery();
            }
            catch
            {
            }
            finally
            {
                connection.Close();
                connection.Dispose();
                connection = null;
            }
        }

        static PcInfo FillInformationAboutPc()
        {
            PcInfo pcInfoData = new PcInfo();
            //Операционная система 
            ManagementObjectSearcher searcher5 = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_OperatingSystem");
            foreach (ManagementObject queryObj in searcher5.Get())
            {
                //  pcInfoData.OperatingSystem += String.Format("-----------------------------------\n");
                pcInfoData.OperatingSystem += String.Format("BuildNumber: {0}\n", queryObj["BuildNumber"]);
                pcInfoData.OperatingSystem += String.Format("Caption: {0}\n", queryObj["Caption"]);
                pcInfoData.OperatingSystem += String.Format("FreePhysicalMemory: {0}\n", queryObj["FreePhysicalMemory"]);
                pcInfoData.OperatingSystem += String.Format("FreeVirtualMemory: {0}\n", queryObj["FreeVirtualMemory"]);
                pcInfoData.OperatingSystem += String.Format("Name: {0}\n", queryObj["Name"]);
                pcInfoData.OperatingSystem += String.Format("OSType: {0}\n", queryObj["OSType"]);
                pcInfoData.OperatingSystem += String.Format("RegisteredUser: {0}\n", queryObj["RegisteredUser"]);
                pcInfoData.OperatingSystem += String.Format("SerialNumber: {0}\n", queryObj["SerialNumber"]);
                pcInfoData.OperatingSystem += String.Format("ServicePackMajorVersion: {0}\n", queryObj["ServicePackMajorVersion"]);
                pcInfoData.OperatingSystem += String.Format("ServicePackMinorVersion: {0}\n", queryObj["ServicePackMinorVersion"]);
                pcInfoData.OperatingSystem += String.Format("Status: {0}\n", queryObj["Status"]);
                pcInfoData.OperatingSystem += String.Format("SystemDevice: {0}\n", queryObj["SystemDevice"]);
                pcInfoData.OperatingSystem += String.Format("SystemDirectory: {0}\n", queryObj["SystemDirectory"]);
                pcInfoData.OperatingSystem += String.Format("SystemDrive: {0}\n", queryObj["SystemDrive"]);
                pcInfoData.OperatingSystem += String.Format("Version: {0}\n", queryObj["Version"]);
                pcInfoData.OperatingSystem += String.Format("WindowsDirectory: {0}\n", queryObj["WindowsDirectory"]);
            }
            //Видеокарта
            ManagementObjectSearcher searcher11 = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController");
            foreach (ManagementObject queryObj in searcher11.Get())
            {
                // pcInfoData.Videocard += String.Format("----------- VideoController -----------\n");
                pcInfoData.Videocard += String.Format("AdapterRAM: {0}\n", queryObj["AdapterRAM"]);
                pcInfoData.Videocard += String.Format("Caption: {0}\n", queryObj["Caption"]);
                pcInfoData.Videocard += String.Format("Description: {0}\n", queryObj["Description"]);
                pcInfoData.Videocard += String.Format("VideoProcessor: {0}\n", queryObj["VideoProcessor"]);
            }
            //CPU
            ManagementObjectSearcher searcher8 = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
            foreach (ManagementObject queryObj in searcher8.Get())
            {
                //   pcInfoData.CPU += String.Format("------------- Processor---------------\n");
                pcInfoData.CPU += String.Format("Name: {0}\n", queryObj["Name"]);
                pcInfoData.CPU += String.Format("NumberOfCores: {0}\n", queryObj["NumberOfCores"]);
                pcInfoData.CPU += String.Format("ProcessorId: {0}\n", queryObj["ProcessorId"]);
            }
            //RAM
            ManagementObjectSearcher searcher12 = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PhysicalMemory");
            foreach (ManagementObject queryObj in searcher12.Get())
            {
                pcInfoData.RAM += String.Format("BankLabel: {0} ; Capacity: {1} Gb; Speed: {2}\n", queryObj["BankLabel"],
                                  Math.Round(System.Convert.ToDouble(queryObj["Capacity"]) / 1024 / 1024 / 1024, 2), queryObj["Speed"]);
            }
            //Motherboard
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");
            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    pcInfoData.Motherboard += wmi.GetPropertyValue("Manufacturer").ToString();
                }
                catch { }
            }
            //Температура
            pcInfoData.Temperature += HelperMethods.GetTemperature();
            return pcInfoData;
        }



    }
}
