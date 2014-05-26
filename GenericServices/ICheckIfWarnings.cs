namespace GenericServices
{
    public interface ICheckIfWarnings
    {
        /// <summary>
        /// This allows the user to control whether data should still be written if warnings
        /// </summary>
        bool WriteEvenIfWarning { get; }
    }

}
