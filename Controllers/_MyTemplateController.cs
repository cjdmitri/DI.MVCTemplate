using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using DI.MVCTemplate.Models;
using DI.MVCTemplate.Data;

namespace DI.MVCTemplate
{
    public class _MyTemplateController : Controller
    {
        /// <summary>
        /// Отслеживаем время выполнения
        /// </summary>
        internal Stopwatch sw = new Stopwatch();
        /// <summary>
        /// Тип сообщения
        /// </summary>
        internal enum MessageType
        {
            warning,
            danger,
            success,
            info
        };

        internal IWebHostEnvironment env;
        internal ApplicationDbContext db;
        internal readonly UserManager<AppUser> _userManager;
        internal readonly SignInManager<AppUser> _signInManager;
        internal RoleManager<IdentityRole> _roleManager;

        internal _MyTemplateController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager,  SignInManager<AppUser> signInManager, IWebHostEnvironment _env, ApplicationDbContext _db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            env = _env;
            db = _db;
        }

        internal _MyTemplateController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, IWebHostEnvironment _env)
        {
            _userManager = userManager;
            env = _env;
            _roleManager = roleManager;
        }




        /// <summary>
        /// Добавляет в выход данные с сообщением об ошибке, выполнении информации
        /// Данные передаются в частичное представление _Message
        /// </summary>
        /// <param name="type">Тип сообщения</param>
        /// <param name="title">Заголовок сообщения</param>
        /// <param name="message">Текст сообщения</param>
        internal void AddDataMessage(MessageType type, string title, string message)
        {
            TempData["TypeMessage"] = Enum.GetName(typeof(MessageType), type.GetHashCode());
            TempData["TitleMessage"] = title;
            TempData["Message"] = message;
        }

        internal async Task<List<AppUserData>> GetUserDatasAsync(AppUser user)
        {
            var ud = new List<AppUserData>();
            try
            {
                ud.AddRange(await db.AppUsersData.AsNoTracking().Where(x => x.UserId == user.Id).ToListAsync());
            }
            catch (Exception ex)
            {
                MyApp.LogAdd(MyApp.TypeMessageLog.Error, $"Ошибка при получении UserData пользователя: {ex.Message} \t {ex.StackTrace}");
            }


            return ud;
        }


        /// <summary>
        /// Обновляет или создает пользовательские данные
        /// </summary>
        /// <param name="userId">ИД пользователя</param>
        /// <param name="keyUserData">Ключ поля</param>
        /// <param name="data">Данные</param>
        /// <param name="keyName">Название ключа</param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        internal async Task SaveUserData(string userId, string keyUserData, object data, string keyName = "", string dateTime = "")
        {
            AppUserData udata = await db.AppUsersData.AsNoTracking()
                .Where(x => x.UserId == userId)
                .Where(x => x.Key == keyUserData)
                .FirstOrDefaultAsync(x => x.KeyName == keyName);
            if (udata == null)
            {
                AppUserData n = new AppUserData
                {
                    UserId = userId,
                    Key = keyUserData,
                    KeyName = keyName,
                    Data = Convert.ToString(data)
                };
                if (String.IsNullOrEmpty(dateTime))
                    n.DateCreate = DateTime.Now.ToShortDateString();
                else
                    n.DateCreate = dateTime;
                db.AppUsersData.Add(n);
            }
            else
            {
                udata.Data = Convert.ToString(data);
                db.AppUsersData.Update(udata);
            }

            await db.SaveChangesAsync();
        }

        /// <summary>
        /// создает пользовательские данные
        /// </summary>
        /// <param name="userId">ИД пользователя</param>
        /// <param name="keyUserData">Ключ поля</param>
        /// <param name="data">Данные</param>
        /// <param name="keyName">Название ключа</param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        internal async Task NewUserData(string userId, string keyUserData, object data, string keyName = "", string dateTime = "")
        {

            AppUserData n = new AppUserData
            {
                UserId = userId,
                Key = keyUserData,
                KeyName = keyName,
                Data = Convert.ToString(data)
            };
            if (String.IsNullOrEmpty(dateTime))
                n.DateCreate = DateTime.Now.ToShortDateString();
            else
                n.DateCreate = dateTime;
            db.AppUsersData.Add(n);

            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Обновление данных пользователя
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="data">Данные</param>
        /// <returns></returns>
        internal async Task UpdateUserData(int id, string userId, object data)
        {
            AppUserData udata = await db.AppUsersData.AsNoTracking()
                .Where(x => x.UserId == userId)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (udata != null)
            {
                udata.Data = Convert.ToString(data);
                db.AppUsersData.Update(udata);
                await db.SaveChangesAsync();
            }
        }
        /// <summary>
        /// Удаляет пользовательские данные по ид
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal async Task DeleteUserData(int id)
        {
            AppUserData userData = await db.AppUsersData.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (userData != null)
            {
                db.Entry(userData).State = EntityState.Deleted;
                await db.SaveChangesAsync();
            }
            else
            {
                AddDataMessage(MessageType.danger, "Ошибка!", "Запись не найдена");
            }
        }
    }
}