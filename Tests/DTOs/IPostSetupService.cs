namespace Tests.DTOs
{
    public interface IPostSetupService
    {
        /// <summary>
        /// This should be called before the dto is shown. It sets up the dropdownLists etc with data from the database
        /// </summary>
        /// <param name="dto"></param>
        void SetupDropDownLists(IDetailPostDto dto);
    }
}