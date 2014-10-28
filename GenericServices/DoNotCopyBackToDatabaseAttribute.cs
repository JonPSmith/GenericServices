using System;

namespace GenericServices
{
    /// <summary>
    /// Place this on a property to stop it being copied back to the TEntity
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DoNotCopyBackToDatabaseAttribute : Attribute
    {
    }
}
