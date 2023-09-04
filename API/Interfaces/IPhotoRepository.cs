using API.Entities;

namespace API;

public interface IPhotoRepository

{
    Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos();
    Task<Photo> GetPhotoById(int id);
    void RemovePhoto(Photo photo);
}
