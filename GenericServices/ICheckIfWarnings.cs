namespace GenericServices
{
    public interface ICheckIfWarnings
    {
        /// <summary>
        /// This allows the user to control whether data should still be written even if warnings found
        /// </summary>
        bool WriteEvenIfWarning { get; }
    }

}
