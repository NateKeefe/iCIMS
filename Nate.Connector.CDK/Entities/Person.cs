using Newtonsoft.Json;
using Scribe.Connector.Common.Reflection;
using Scribe.Connector.Common.Reflection.Actions;

namespace CDK.Entities.Person
{
    [Query]
    [CreateWith]
    [ObjectDefinition(Name = "Person")]
    public class Rootobject
    {
        [PropertyDefinition]
        public string firstname { get; set; }
        [PropertyDefinition]
        public Folder folder { get; set; }
        [PropertyDefinition]
        public Link[] links { get; set; }
        [PropertyDefinition]
        public string email { get; set; }
        [PropertyDefinition]
        public string lastname { get; set; }

        //Filters
        [PropertyDefinition(UsedInQueryConstraint = true, UsedInQuerySelect = false, UsedInActionInput = false, UsedInActionOutput = false)]
        [JsonIgnore]
        public string peopleId { get; set; }
        //Results
        [PropertyDefinition(RequiredInActionInput = false, UsedInActionInput = false, UsedInQueryConstraint = false)]
        [JsonIgnore]
        public string location { get; set; }
    }

    [ObjectDefinition]
    public class Folder
    {
        [PropertyDefinition]
        public string id { get; set; }
        [PropertyDefinition]
        public string formattedvalue { get; set; }
        [PropertyDefinition]
        public string value { get; set; }
    }

    [ObjectDefinition]
    public class Link
    {
        [PropertyDefinition]
        public string rel { get; set; }
        [PropertyDefinition]
        public string title { get; set; }
        [PropertyDefinition]
        public string url { get; set; }
    }

}
