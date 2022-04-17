using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Volo.Abp.AspNetCore.Mvc;

namespace Passingwind.Abp.ElsaModule.Controllers;

public class HomeController : AbpController
{
    public ActionResult Index()
    {
        return Redirect("~/swagger");
    }

    public ActionResult Test()
    {
        var data = new TestDto
        {
            ActivityData = new Dictionary<string, IDictionary<string, object>> {
                { "aaaa", new Dictionary<string,object>{ { "bbb", new Dictionary<string, object> { { "path", "23123" } } } } },
                { "bbb", new Dictionary<string,object>{ { "ccc", new JObject(new { a=1,b=2,c= new string[] { "1","2" } })   } } }
            }
        };

        return Ok(data);
    }
}

internal class TestDto
{

    public Dictionary<string, IDictionary<string, object>> ActivityData { get; set; }

}
