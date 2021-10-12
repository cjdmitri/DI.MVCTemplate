using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DI.MVCTemplate.TagHelpers
{
    //Данный тег-хелпер будет применяться к тегам "a", в которых заполнен "href"
    [HtmlTargetElement("a", Attributes = "href")]
    public class LinkTagHelper : TagHelper
    {
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            try
            {
                //Получаем полную строку запроса
                string urlP = $"{ViewContext?.HttpContext.Request.Path}{ViewContext?.HttpContext.Request.QueryString}";
                var attr = output.Attributes.FirstOrDefault(x => x.Name == "href");

                //=======================================================
                //Сравниваем значение атрибута "href" с полученной строкой запроса
                //Исключаем зацикливание ссылок (ссылки сами на себя)
                //=======================================================
                if (System.Net.WebUtility.UrlEncode(urlP.ToLower()) == System.Net.WebUtility.UrlEncode(attr.Value.ToString().ToLower()))
                {
                    /*
                     * данный код меняет значение атрибутов тега a
                     */
                    output.Attributes.SetAttribute("href", "javascript:location.reload();");
                    var aclass = output.Attributes.FirstOrDefault(x => x.Name == "class");
                    if (aclass == null)
                        output.Attributes.Add("class", "active");
                    else
                        output.Attributes.SetAttribute("class", aclass.Value.ToString() + " active");
                }

                //=======================================================
                //Все ссылки в документе переводим в нижний регистр
                //=======================================================
                string href = output.Attributes.FirstOrDefault(x => x.Name == "href").Value.ToString();
                if (!href.StartsWith("javascript"))
                    output.Attributes.SetAttribute("href", href.ToLower());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
        }
    }
}
