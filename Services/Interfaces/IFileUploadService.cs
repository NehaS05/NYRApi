namespace NYR.API.Services.Interfaces
{
    public interface IFileUploadService
    {
        /// <summary>
        /// Uploads an image file to the specified folder
        /// </summary>
        /// <param name="image">The image file to upload</param>
        /// <param name="folder">The folder name within uploads directory (default: "users")</param>
        /// <returns>The relative URL path of the uploaded image</returns>
        Task<string> UploadImageAsync(IFormFile image, string folder = "users");

        /// <summary>
        /// Deletes an image file from the server
        /// </summary>
        /// <param name="imageUrl">The relative URL of the image to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        Task<bool> DeleteImageAsync(string imageUrl);

        /// <summary>
        /// Validates if the uploaded file is a valid image
        /// </summary>
        /// <param name="file">The file to validate</param>
        /// <returns>True if the file is a valid image, false otherwise</returns>
        bool IsValidImageFile(IFormFile file);
    }
}