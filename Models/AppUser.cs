using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DI.MVCTemplate.Models
{
    public class AppUser : IdentityUser
    {
        /// <summary>
        /// Информация о пользователе в свободной форме
        /// </summary>
        [MaxLength(400)]
        public string Info { get; set; }
        /// <summary>
        /// Дата регистрации на сайте
        /// </summary>
        [MaxLength(30)]
        public string DateRegister { get; set; }
        /// <summary>
        /// Имя пользователя на сайте
        /// </summary>
        [MaxLength(30)]
        public string NameInSite { get; set; }
    }
}
