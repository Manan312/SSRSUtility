namespace Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SkipResponseMiddlewareAttribute:Attribute
    {
    }
}
