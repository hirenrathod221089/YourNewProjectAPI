// Add this helper class at the absolute bottom of your Program.cs file
public class InternalControllerFeatureProvider : Microsoft.AspNetCore.Mvc.Controllers.ControllerFeatureProvider
{
    protected override bool IsController(System.Reflection.TypeInfo typeInfo)
    {
        // Force the app to accept internal classes ending with "Controller"
        if (!typeInfo.IsClass || typeInfo.IsAbstract || typeInfo.ContainsGenericParameters)
            return false;

        return typeInfo.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase);
    }
}