namespace MyBlog.Controllers
{
    internal interface IFileService
    {
        object DecodeImage(byte[] imageData, string contentType);
    }
}