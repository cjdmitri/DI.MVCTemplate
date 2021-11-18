using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DI.MVCTemplate.Models
{
    public class AppUserData
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        /// <summary>
        /// Ключ
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Название ключа, для отображения на сайте
        /// </summary>
        public string KeyName { get; set; }
        /// <summary>
        /// Дата создания
        /// </summary>
        public string DateCreate { get; set; }
        /// <summary>
        /// Данные пользователя для данного ключа
        /// </summary>
        public string Data { get; set; }
    }
}
