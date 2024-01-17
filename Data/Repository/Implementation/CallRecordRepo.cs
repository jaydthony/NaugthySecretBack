using Data.Context;
using Data.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.DTO;
using Model.Enitities;

namespace Data.Repository.Implementation
{
    public class CallRecordRepo : ICallRecordRepo
    {
        private readonly DatingSiteContext _context;

        public CallRecordRepo(DatingSiteContext context)
        {
            _context = context;
        }

        public async Task<bool> AddCallRecord(CallRecord callRecord)
        {
            await _context.CallRecords.AddAsync(callRecord);
            var save = await _context.SaveChangesAsync();
            if (save > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<PaginatedGenericDto<IEnumerable<UserTimeUsedDto>>> AllCamgirlCallRecordsTimeUSed(int pageNumber, int perPageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            perPageSize = perPageSize < 1 ? 5 : perPageSize;
            var Users = _context.CallRecords
            .GroupBy(cr => cr.CamgirlId);
            decimal totalMinute = await _context.CallRecords.SumAsync(u => u.TimeUsed);
            var totalCount = await Users.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / perPageSize);
            var paginated = await Users.Select(g => new
            {
                UserId = g.Key,
                TotalTime = g.Sum(cr => cr.TimeUsed)
            }).Skip((pageNumber - 1) * perPageSize).Take(perPageSize).ToListAsync();
            var userIds = paginated.Select(u => u.UserId).ToList();
            var CamgirlWithTime = await _context.Users.Where(u => userIds.Contains(u.Id)).Select(User => new UserTimeUsedDto
            {
                UserName = User.UserName,
                Email = User.Email,
                FirstName = User.FirstName,
                LastName = User.LastName,
                PhoneNumber = User.PhoneNumber,
                ProfilePicture = User.ProfilePicture,
                Gender = User.Gender,
                Id = User.Id
            }).ToListAsync();
            foreach (var userDto in CamgirlWithTime)
            {
                var user = paginated.FirstOrDefault(u => u.UserId == userDto.Id);
                if (user != null)
                {
                    userDto.TotalTimeUsed = user.TotalTime;
                }
            }
            var result = new PaginatedGenericDto<IEnumerable<UserTimeUsedDto>>
            {
                CurrentPage = pageNumber,
                PageSize = perPageSize,
                TotalPages = totalPages,
                TotalMinuteUsed = totalMinute,
                Result = CamgirlWithTime
            };
            return result;
        }

        public async Task<PaginatedGenericDto<IEnumerable<CallRecord>>> GetAllCallRecords(int pageNumber, int perPageSize)
        {
            var getAllCallRecord = _context.CallRecords;
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            perPageSize = perPageSize < 1 ? 5 : perPageSize;
            var totalCount = await getAllCallRecord.CountAsync();
            decimal totalMinute = await getAllCallRecord.SumAsync(u => u.TimeUsed);
            var totalPages = (int)Math.Ceiling((double)totalCount / perPageSize);
            var paginated = await getAllCallRecord.Skip((pageNumber - 1) * perPageSize).Take(perPageSize).ToListAsync();

            var result = new PaginatedGenericDto<IEnumerable<CallRecord>>
            {
                CurrentPage = pageNumber,
                PageSize = perPageSize,
                TotalPages = totalPages,
                TotalMinuteUsed = totalMinute,
                Result = paginated
            };
            return result;
        }

        public async Task<PaginatedGenericDto<IEnumerable<CallRecord>>> GetUserCallRecords(string userid, int pageNumber, int perPageSize)
        {
            var getUserCallRecord = _context.CallRecords;
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            perPageSize = perPageSize < 1 ? 5 : perPageSize;
            var totalCount = await getUserCallRecord.CountAsync(u => u.UserId == userid);
            decimal totalMinute = await getUserCallRecord.Where(u => u.UserId == userid).SumAsync(u => u.TimeUsed);
            var totalPages = (int)Math.Ceiling((double)totalCount / perPageSize);
            var paginated = await getUserCallRecord.Where(u => u.UserId == userid).Skip((pageNumber - 1) * perPageSize).Take(perPageSize).ToListAsync();

            var result = new PaginatedGenericDto<IEnumerable<CallRecord>>
            {
                CurrentPage = pageNumber,
                PageSize = perPageSize,
                TotalPages = totalPages,
                TotalMinuteUsed = totalMinute,
                Result = paginated
            };
            return result;
        }

        public async Task<IEnumerable<UserTimeUsedDto>> GetUserWithMostUsedTime(int topcount)
        {
            var topUsers = await _context.CallRecords
            .GroupBy(cr => cr.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                TotalTime = g.Sum(cr => cr.TimeUsed)
            })
            .OrderBy(g => g.TotalTime)
            .Take(topcount)
            .ToListAsync();
            var userIds = topUsers.Select(u => u.UserId).ToList();
            var topUsersWithMostTime = await _context.Users.Where(u => userIds.Contains(u.Id)).Select(User => new UserTimeUsedDto
            {
                UserName = User.UserName,
                Email = User.Email,
                FirstName = User.FirstName,
                LastName = User.LastName,
                PhoneNumber = User.PhoneNumber,
                ProfilePicture = User.ProfilePicture,
                Gender = User.Gender,
                Id = User.Id
            }).ToListAsync();
            foreach (var userDto in topUsersWithMostTime)
            {
                var user = topUsers.FirstOrDefault(u => u.UserId == userDto.Id);
                if (user != null)
                {
                    userDto.TotalTimeUsed = user.TotalTime;
                }
            }
            return topUsersWithMostTime;
        }
        public async Task<UserTimeUsedDto> GetUserWithTotalTimeUsedById(string userid)
        {
            var topUserDetails = await _context.Users
                .Where(u => u.Id == userid)
                .Select(user => new
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    ProfilePicture = user.ProfilePicture,
                    Gender = user.Gender,
                    Id = user.Id,
                    TotalTimeUsed = _context.CallRecords
              .Where(cr => cr.UserId == userid)
              .Sum(cr => cr.TimeUsed)
                })
      .FirstOrDefaultAsync();

            if (topUserDetails != null)
            {
                UserTimeUsedDto topUsersWithMostTime = new UserTimeUsedDto
                {
                    UserName = topUserDetails.UserName,
                    Email = topUserDetails.Email,
                    FirstName = topUserDetails.FirstName,
                    LastName = topUserDetails.LastName,
                    PhoneNumber = topUserDetails.PhoneNumber,
                    ProfilePicture = topUserDetails.ProfilePicture,
                    Gender = topUserDetails.Gender,
                    Id = topUserDetails.Id,
                    TotalTimeUsed = topUserDetails.TotalTimeUsed
                };

                return topUsersWithMostTime;
            }


            return null;


        }
    }
}