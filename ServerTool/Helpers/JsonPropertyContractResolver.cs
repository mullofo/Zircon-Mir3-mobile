using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Helpers
{
    public class JsonPropertyContractResolver : DefaultContractResolver
    {
        IEnumerable<string> lstInclude;
        public JsonPropertyContractResolver(IEnumerable<string> includeProperties)
        {
            lstInclude = includeProperties;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, memberSerialization).ToList().FindAll(p => !lstInclude.Contains(p.PropertyName));//需要输出的属性
        }
    }

}
