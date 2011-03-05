
namespace ObjectGraph.Test.Xml.Model
{
    public enum ContactMethodType
    {
        HomePhone,
        MobilePhone,
        Email,
        Address,
    }

    public class ContactMethod
    {
        public ContactMethodType Type { get; set; }

        public string Value { get; set; }
    }

    public class AddressContactMethod : ContactMethod
    {
        public string StreetName { get; set; }
    }
}
