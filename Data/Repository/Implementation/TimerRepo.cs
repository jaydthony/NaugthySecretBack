using Data.Context;
using Data.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.DTO;
using Model.Enitities;

namespace Data.Repository.Implementation
{
    public class TimerRepo : ITimerRepo
    {
        private readonly DatingSiteContext _context;

        public TimerRepo(DatingSiteContext context)
        {
            _context = context;
        }

        public async Task<Timers> AddTimeAsync(Timers time)
        {
            var addTime = await _context.Timers.AddAsync(time);
            var resp = await _context.SaveChangesAsync();
            if (resp > 0)
            {
                return addTime.Entity;
            }
            return null;
        }

        public async Task<bool> DeleteTimeAsync(Timers time)
        {
            _context.Timers.Remove(time);
            var resp = await _context.SaveChangesAsync();
            if (resp > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<PaginatedTimeGenericDto<IEnumerable<UserTimeBuyDto>>> GetAllTimeBuyAsync(int pageNumber, int perPageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            perPageSize = perPageSize < 1 ? 5 : perPageSize;
            var Users = _context.Timers
            .GroupBy(cr => cr.UserId);
            long totalMinute = await _context.Timers.SumAsync(u => u.TimeBought);
            var totalCount = await Users.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / perPageSize);
            var paginated = await Users.Select(g => new
            {
                UserId = g.Key,
                TotalTime = g.Sum(cr => cr.TimeBought)
            }).Skip((pageNumber - 1) * perPageSize).Take(perPageSize).ToListAsync();
            var userIds = paginated.Select(u => u.UserId).ToList();
            var UserWithTime = await _context.Users.Where(u => userIds.Contains(u.Id)).Select(User => new UserTimeBuyDto
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
            foreach (var userDto in UserWithTime)
            {
                var user = paginated.FirstOrDefault(u => u.UserId == userDto.Id);
                if (user != null)
                {
                    userDto.TotalTimeBuy = user.TotalTime;
                }
            }
            var result = new PaginatedTimeGenericDto<IEnumerable<UserTimeBuyDto>>
            {
                CurrentPage = pageNumber,
                PageSize = perPageSize,
                TotalPages = totalPages,
                TotalMinuteBuy = totalMinute,
                Result = UserWithTime
            };
            return result;
        }

        public async Task<Timers> getTimeAsync(string timeid)
        {
            return await _context.Timers.FindAsync(timeid);
        }

        public async Task<PaginatedTimeGenericDto<IEnumerable<Timers>>> GetUserTimerBuy(string userid, int pageNumber, int perPageSize)
        {
            var getUserTimer = _context.Timers;
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            perPageSize = perPageSize < 1 ? 5 : perPageSize;
            var totalCount = await getUserTimer.CountAsync(u => u.UserId == userid);
            long totalMinute = await getUserTimer.Where(u => u.UserId == userid).SumAsync(u => u.TimeBought);
            var totalPages = (int)Math.Ceiling((double)totalCount / perPageSize);
            var paginated = await getUserTimer.Where(u => u.UserId == userid).Skip((pageNumber - 1) * perPageSize).Take(perPageSize).ToListAsync();

            var result = new PaginatedTimeGenericDto<IEnumerable<Timers>>
            {
                CurrentPage = pageNumber,
                PageSize = perPageSize,
                TotalPages = totalPages,
                TotalMinuteBuy = totalMinute,
                Result = paginated
            };
            return result;
        }

        public async Task<IEnumerable<UserTimeBuyDto>> GetUserWithMostBuyTime(int topcount)
        {
            var topUsers = await _context.Timers
            .GroupBy(cr => cr.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                TotalTime = g.Sum(cr => cr.TimeBought)
            })
            .OrderBy(g => g.TotalTime)
            .Take(topcount)
            .ToListAsync();
            var userIds = topUsers.Select(u => u.UserId).ToList();
            var topUsersWithMostTime = await _context.Users.Where(u => userIds.Contains(u.Id)).Select(User => new UserTimeBuyDto
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
                    userDto.TotalTimeBuy = user.TotalTime;
                }
            }
            return topUsersWithMostTime;
        }
    }
}