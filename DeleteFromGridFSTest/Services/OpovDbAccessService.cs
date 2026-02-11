using DeleteFromGridFSTest.Data;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System.Collections;

namespace DeleteFromGridFSTest.Services
{
    public class OpovDbAccessService : IOpovDbAccessService
    {
        private MongoClient? mongoClient;
        private IMongoDatabase? iMongoDatabase;
        private GridFSBucket UploadedPicsBucket;

        public OpovDbAccessService(MongoDbConfig config)
        {
            try
            {
                mongoClient = new MongoClient(config.OpovDbConnectionString);
                iMongoDatabase = mongoClient.GetDatabase(config.DatabaseName);

                iMongoDatabase!.CreateCollection("Contacts");

                UploadedPicsBucket = new GridFSBucket(iMongoDatabase, new GridFSBucketOptions { BucketName = "GridFsUploadedPics" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }                     
        }

        public bool AddContact(Contact contact)
        {
            var contactCollection = iMongoDatabase!.GetCollection<Contact>("Contacts");

            var builder = Builders<Contact>.Filter;
            var filter = builder.And(
                builder.Eq(g => g.FirstName, contact.FirstName),
                builder.Eq(r => r.LastName, contact.LastName));

            var results = contactCollection.Find(filter).ToList();

            if (results.Count == 0)
            {
                contactCollection.InsertOne(contact);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Returns empty string on Error
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public string DeleteContact(string contactId)
        {
            try
            {
                var contactCollection = iMongoDatabase!.GetCollection<Contact>("Contacts");
                var filter = Builders<Contact>.Filter.Eq(r => r.Id, contactId);
                DeleteResult result = contactCollection.DeleteOne(filter);
                if (result.DeletedCount == 1)
                {
                    return $"Record ({contactId}) was deleted.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return "";
        }

        public async Task<List<Contact>> GetAllContactsAsync()
        {
            List<Contact> results = [];
            try
            {
                var contactCollection = iMongoDatabase!.GetCollection<Contact>("Contacts");
                var builder = Builders<Contact>.Filter;
                var filter = builder.Empty;

                results = (await contactCollection.FindAsync(filter)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return results;
        }


        public bool UploadImageData(byte[] data, string fileName)
        {
            try
            {
                using (GridFSUploadStream? uploader = UploadedPicsBucket.OpenUploadStream(fileName))
                {
                    if (uploader is not null)
                    {
                        uploader.Write(data);
                        return true;
                    }
                }
            }
            catch (Exception e) 
            { 
                Console.WriteLine(e);
            }
            return false;
        }

        public async Task<byte[]> GetImageFileData(string dbRecordId)
        {
            byte[] imageFileData = [];
            try
            {
                ObjectId fileObjectId = ObjectId.Parse(dbRecordId);
                FilterDefinition<GridFSFileInfo<ObjectId>> filter = Builders<GridFSFileInfo<ObjectId>>.Filter.Eq(x => x.Id, fileObjectId);

                GridFSFileInfo<ObjectId> fInfo = UploadedPicsBucket.Find(filter).FirstOrDefault();

                if (fInfo != null)
                {
                    using (var downloader = UploadedPicsBucket.OpenDownloadStream(fInfo.Id))
                    {
                        var buffer = new byte[downloader.Length];
                        downloader.Read(buffer, 0, buffer.Length);
                        imageFileData = buffer;
                    }
                }

                return imageFileData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Array.Empty<byte>();
            }
        }

        public async Task<List<(string, string)>> GetImageFilesInfoListAsync()
        {
            List<(string, string)> listOfTuples = [];

            var filter = Builders<GridFSFileInfo>.Filter.Empty;

            try
            {
                using (var cursor = UploadedPicsBucket.Find(filter))
                {
                    //var fileInfo = cursor.ToList().FirstOrDefault();
                    // fileInfo either has the matching file information or is null
                    var fileInfo = cursor.ToList();
                    foreach (var file in fileInfo)
                    {
                        var temp = file;
                        if (temp is not null)
                        {
                            string name = temp.Filename;
                            string fileId = temp.Id.ToString();
                            listOfTuples.Add((name, fileId));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            return listOfTuples;
        }

        public async Task<bool> DeleteImageFile(string idOfImageFile)
        {     
            try
            {
                if (!string.IsNullOrEmpty(idOfImageFile))
                {
                    ObjectId rawDataIdAsObj = new ObjectId(idOfImageFile);

                    var tsk = UploadedPicsBucket.DeleteAsync(rawDataIdAsObj);
                    tsk.Wait();

                    // First delete the data file pointed to by the ImageFile
                    // var tsk =
                //    UploadedPicsBucket.DeleteAsync(rawDataId);  // this just hangs for some reason
                    //       UploadedPicsBucket.Delete(rawDataId);                  // this also just hangs
                    //var tsk = UploadedPicsBucket.Delete(rawDataIdAsObj); 
                    //var tsk2 = 

                    //ObjectId rawDataIdAsObj = new ObjectId(rawDataId);
                    //var bb = BsonValue(rawDataIdAsObj);
                    //;

                    //await tsk;

                    //if (tsk.IsCompleted)
                    //{
                    //    var dbg = tsk.Id;
                    //}


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }
    }
}
