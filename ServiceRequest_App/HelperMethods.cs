using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using OpenHardwareMonitor;
using OpenHardwareMonitor.Collections;
using OpenHardwareMonitor.Hardware;
using System.Timers;

namespace ServiceRequest_App
{
    public class HelperMethods
    {

        //Температура 
        static public string GetTemperature()
        {
            string Info = null;
            Computer computer = new Computer() { CPUEnabled = true, GPUEnabled = true };
            computer.Open();
            foreach (IHardware hardware in computer.Hardware)
            {
                hardware.Update();
                Info += "\n" + String.Format("{0}: {1}", hardware.HardwareType, hardware.Name);
                foreach (ISensor sensor in hardware.Sensors)
                {
                    // Celsius is default unit
                    if (sensor.SensorType == SensorType.Temperature)
                    {
                        Info += "\n" + String.Format("{0}: {1}°C", sensor.Name, sensor.Value);
                    }
                }
            }
            return Info;
        }

        //Проверка на то, запущена ли программа от имени администратора
        public static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                    .IsInRole(WindowsBuiltInRole.Administrator);
        }

        //Получение IPv4 хоста
        static public string GetIPv4()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return null;
        }

        //Получение MAC с компьютера
        static public string GetMacAddress()
        {
            string macAddresses = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }
            return macAddresses;
        }

        //Красивый MAC
        static public string GetBeautiMacAddress()
        {
            string macAddresses = GetMacAddress();
            if (macAddresses.Length != 12)
            {
                return macAddresses = "Error";
            }
            macAddresses = macAddresses.Insert(2, ":");
            macAddresses = macAddresses.Insert(5, ":");
            macAddresses = macAddresses.Insert(8, ":");
            macAddresses = macAddresses.Insert(11, ":");
            macAddresses = macAddresses.Insert(14, ":");
            return macAddresses;
        }

        //Данные из БД

        static public string GetWorkTime(MainWindow winM)
        {
            string output = "";
            if (winM.Monday.IsChecked.Value)
            {
                output += "ПН:+\n";
            }
            else
            {
                output += "ПН:-\n";
            }
            if (winM.Tuesday.IsChecked.Value)
            {
                output += "ВТ:+\n";
            }
            else
            {
                output += "ВТ:-\n";
            }
            if (winM.Wednesday.IsChecked.Value)
            {
                output += "СР:+\n";
            }
            else
            {
                output += "СР:-\n";
            }
            if (winM.Thursday.IsChecked.Value)
            {
                output += "ЧТ:+\n";
            }
            else
            {
                output += "ЧТ:-\n";
            }
            if (winM.Friday.IsChecked.Value)
            {
                output += "ПТ:+\n";
            }
            else
            {
                output += "ПТ:-\n";
            }

            if (winM.Saturday.IsChecked.Value)
            {
                output += "СБ:+\n";
            }
            else
            {
                output += "СБ:-\n";
            }
            if (winM.Sunday.IsChecked.Value)
            {
                output += "ВС:+\n";
            }
            else
            {
                output += "ВС:-\n";
            }
            output += "С:" + winM.TimeFrom.Text + "\n";
            output += "До:" + winM.TimeTo.Text;
            return output;
        }


    }
}
