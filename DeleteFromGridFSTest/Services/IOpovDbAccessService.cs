using DeleteFromGridFSTest.Data;

namespace DeleteFromGridFSTest.Services
{
    public interface IOpovDbAccessService
    {
        bool UploadImageData(byte[] data, string fileName);

        bool AddContact(Contact contact);

        string DeleteContact(string contactId);

        Task<List<Contact>> GetAllContactsAsync();

        Task<byte[]> GetImageFileData(string dbRecordId);

        Task<List<(string, string)>> GetImageFilesInfoListAsync();

        Task<bool> DeleteImageFile(string idOfImageFile);
    }
}
