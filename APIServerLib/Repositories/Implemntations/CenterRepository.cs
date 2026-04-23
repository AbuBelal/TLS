using APIServerLib.Data;
using APIServerLib.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Responses;

namespace APIServerLib.Repositories.Implemntations
{
    public class CenterRepository : ICenterRepository
    {
        private readonly ApplicationDbContext _context;

        public CenterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Center>> GetAll()
        {
            return await _context.Centers.ToListAsync();
        }

        public async Task<Center> GetById(long id)
        {
            return await _context.Centers.FindAsync(id);
        }

        public async Task<GeneralResponse> Insert(Center item)
        {
            _context.Centers.Add(item);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Center added successfully.", item.Id);
        }

        public async Task<GeneralResponse> Update(CenterUpsertDto dto)
        {
            var userCenter = await _context.Centers.FindAsync(dto.Id);
            userCenter.Name = dto.Name;
            userCenter.CenterCode = dto.CenterCode;
            userCenter.Address = dto.Address;
            userCenter.DaysOfWeek = dto.DaysOfWeek;
            userCenter.Rooms = dto.Rooms;
            userCenter.Tarpaulins = dto.Tarpaulins;
            userCenter.OtherSpaces = dto.OtherSpaces;
            userCenter.Comments = dto.Comments;
            userCenter.WhoursId = dto.WHours;
            userCenter.EnName = dto.EnName;
            userCenter.SortOrder = dto.SortOrder;
            userCenter.BuildingCode = dto.BuildingCode;


            _context.Centers.Update(userCenter);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Center updated successfully.", userCenter.Id);
        }

        public async Task<GeneralResponse> DeleteById(long id)
        {
            var center = await _context.Centers.FindAsync(id);
            if (center == null)
                return new GeneralResponse(false, "Center not found.", 0);

            _context.Centers.Remove(center);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Center deleted successfully.", id);
        }


        // ── الجديد: جلب مركز المستخدم الحالي ─────────────────────

        public async Task<Center?> GetByUserIdAsync(string userId)
        {
            // المستخدم → موظف → آخر EmpCenter نشط → مركز
            var empCenter = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.Employee)
                .Where(e => e != null)
                .SelectMany(e => e!.EmpCenters)
                .Where(ec => ec.ToDate == null)
                .OrderByDescending(ec => ec.FromDate)
                .Include(ec => ec.Center)
                .FirstOrDefaultAsync();

            return empCenter?.Center;
        }

        // ── الجديد: تعديل آمن بعد التحقق من الملكية ───────────────

        public async Task<GeneralResponse> UpdateByUserAsync(CenterUpsertDto dto, string userId)
        {
            // 1. تحقق أن المستخدم ينتمي لهذا المركز
            var userCenter = await GetByUserIdAsync(userId);

            if (userCenter is null)
                return new GeneralResponse(false, "لا يوجد مركز مرتبط بحسابك.", 0);

            if (userCenter.Id != dto.Id)
                return new GeneralResponse(false, "غير مصرح لك بتعديل هذا المركز.", 0);

            // 2. تطبيق التعديلات
            userCenter.Name = dto.Name;
            userCenter.CenterCode = dto.CenterCode;
            userCenter.Address = dto.Address;
            userCenter.DaysOfWeek = dto.DaysOfWeek;
            userCenter.Rooms = dto.Rooms;
            userCenter.Tarpaulins = dto.Tarpaulins;
            userCenter.OtherSpaces = dto.OtherSpaces;
            userCenter.Comments = dto.Comments;

            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "تم تعديل بيانات المركز بنجاح.", userCenter.Id);
        }

        public async Task<GeneralResponse> Update(Center item)
        {
            _context.Centers.Update(item);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Center updated successfully.", item.Id);
        }
    }
}