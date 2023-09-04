using API.Data;
using API.Entities;

using Microsoft.EntityFrameworkCore;

namespace API;

public class PhotoRepository : IPhotoRepository
{
    private readonly DataContext _context;
    public PhotoRepository(DataContext context)
    {
        _context = context;

    }
    public async Task<Photo> GetPhotoById(int id)
    {
        return await _context.Photos
            .IgnoreQueryFilters()
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
    {
        return await _context.Photos
            .IgnoreQueryFilters()
            .Where(p => p.IsApproved == false)
            .Select(x => new PhotoForApprovalDto
            {
                Id = x.Id,
                Username = x.AppUser.UserName,
                Url = x.Url,
                IsApproved = x.IsApproved,
            }).ToListAsync();


    }

    public void RemovePhoto(Photo photo)
    {
        _context.Photos.Remove(photo);
    }
}
