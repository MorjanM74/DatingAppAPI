using API.DTOs;
using API.Entites;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public UserRepository(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public void Update(AppUser user)
    {
        _dbContext.Entry(user).State = EntityState.Modified;
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        var data = _dbContext.Users.Include(x=>x.Photos).ToListAsync();
        return await data;
    }

    public async Task<AppUser> GetUserByIdAsync(int id)
    {
        var data = _dbContext.Users.FindAsync(id);
        return await data;
    }

    public async Task<AppUser> GetUserByUsernameAsync(string username)
    {
        var data = _dbContext.Users.Include(x=>x.Photos).Where(x => x.UserName == username).FirstOrDefaultAsync();
        return await data;
    }

    public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
    {
        var query = _dbContext.Users
            .AsQueryable();

        query = query.Where(u => u.UserName != userParams.CurrentUsername);
        query = query.Where(u => u.Gender == userParams.Gender);

        var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
        var MaxDob = DateTime.Today.AddYears(-userParams.MinAge);

        query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= MaxDob);
        
        return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking(),
            userParams.PageNumber, userParams.PageSize);
    }

    public async Task<MemberDto> GetMemberAsync(string username)
    {
        return await _dbContext.Users
            .Where(x => x.UserName == username)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }
}