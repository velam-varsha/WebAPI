namespace WebAPIDemo.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class RequiredClaimAttribute : Attribute 
    {
        // claim is basically a key and a value pair, that describes the information about the application or the user. In our case, it describes the information about the application.
        // here ClaimType is basically the key in the claim. Here it doesnt need to implemennt the setter, so lets remove the set from the property, coz we are gonna initialize them in the constructor
        public string ClaimType { get; }
        public string ClaimValue { get; }
        public RequiredClaimAttribute(string claimType, string claimValue)
        {
            this.ClaimType = claimType;
            this.ClaimValue = claimValue;
        }

    }
}
