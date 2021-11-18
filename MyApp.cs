using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DI.MVCTemplate.Models;
using System.Net;
using System.Net.Http;

namespace DI.MVCTemplate
{
    public static class MyApp
    {

        public static int TestTask;
        /// <summary>
        /// Строка подключения к базе данных
        /// В зависимости от режима работы указывается в классе Startup
        /// </summary>
        public static string ConnectionString;
        public static Stopwatch sw = new Stopwatch();

        /// <summary>
        /// Тип сообщения в логе
        /// </summary>
        public enum TypeMessageLog
        {
            Error,
            Info
        }

        /// <summary>
        /// Добавить сообщение в лог
        /// </summary>
        /// <param name="type"></param>
        /// <param name="text"></param>
        public static void LogAdd(TypeMessageLog type, string text)
        {
            try
            {
                string path = Path.Combine($@"Data\Logs\log{DateTime.Now.Year}.{DateTime.Now.Month}.{DateTime.Now.Day}.txt");
                string content = $"{Enum.GetName(typeof(TypeMessageLog), type.GetHashCode())} \t {DateTime.Now.ToShortTimeString()} \t {text}";
                if (!File.Exists(path))
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(content);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine(content);
                    }

                }
                ConsoleColor color = Console.BackgroundColor;
                switch (type)
                {
                    case TypeMessageLog.Error:
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.WriteLine(content);
                        break;
                    case TypeMessageLog.Info:
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine(content);
                        break;
                    default:
                        Console.WriteLine(content);
                        break;
                }
                Console.BackgroundColor = color;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }


        /// <summary>
        /// Generates a Random Password
        /// respecting the given strength requirements.
        /// </summary>
        /// <param name="opts">A valid PasswordOptions object
        /// containing the password strength requirements.</param>
        /// <returns>A random password</returns>
        public static string GenerateRandomPassword(PasswordOptions opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 8,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            string[] randomChars = new[] {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
            "abcdefghijkmnopqrstuvwxyz",    // lowercase
            "0123456789",                   // digits
            "!@$?_-"                        // non-alphanumeric
        };

            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }

        /// <summary>
        /// Получаем IP пользователя
        /// </summary>
        public static string GetUserIp()
        {
            System.Net.IPHostEntry heserver = Dns.GetHostEntry(Dns.GetHostName());
            var ip = heserver.AddressList[2].ToString();
            return ip;
        }
    }

}
