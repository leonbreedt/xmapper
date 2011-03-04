
namespace ObjectGraph.Test.Xml.Model
{
    public enum ContactMethodType
    {
        HomePhone,
        MobilePhone,
        Email,
    }

    public class ContactMethod
    {
        public ContactMethodType Type { get; set; }

        public string Value { get; set; }
    }
}
