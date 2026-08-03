using APIServerLib.Data;
using APIServerLib.Repositories.Interfaces;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Responses;
using System.Security.Claims;

namespace APIServerLib.Repositories.Implemntations
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Student>> GetAll()
        {
            var students = await _context.Students
                .AsNoTracking()
                .Include(x => x.Level)
                .Include(x => x.Gender)
                .Include(x => x.StdCenters.FirstOrDefault(z => z.IsActive)).ThenInclude(x => x.Center)
                .ToListAsync();
            return students;

        }

        public async Task<Student> GetById(long id)
        {
            var student = await _context.Students.AsNoTracking().Where(s => s.Id == id)
                .Include(s => s.StdCenters).ThenInclude(c => c.Center)
                .Include(g => g.Gender)
                .Include(l => l.Level).FirstOrDefaultAsync();
            return student;
        }

        public async Task<GeneralResponse> Insert(Student item)
        {
            var Std = await _context.Students.Where(s => s.CivilId == item.CivilId)
                .Include(x => x.StdCenters).ThenInclude(x => x.Center)
                .Include(x => x.Level).FirstOrDefaultAsync();
            if (Std is null)
            {
                _context.Students.Add(item);
                await _context.SaveChangesAsync();
                return new GeneralResponse(true, "تم إضافة الطالب بنجاح.", item.Id);
            }

            return new GeneralResponse(false, $"رقم الهوية موجود مسبقاً في مركز {Std.StdCenters.OrderByDescending(x => x.FromDate).First().Center.Name} لطالب اسمه {Std.Name} في الصف {Std.Level.Name} ", 0);
        }

        public async Task<GeneralResponse> Update(Student item)
        {
            _context.Students.Update(item);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "تم تحديث بيانات الطالب بنجاح .", item.Id);
        }

        public async Task<GeneralResponse> DeleteById(long id)
        {
            var student = await _context.Students.Include(x => x.StdCenters).Where(x => x.Id == id).FirstOrDefaultAsync();
            if (student == null)
                return new GeneralResponse(false, "الطالب غير موجود.", 0);
            var stdCenter = student.StdCenters.FirstOrDefault(x => x.IsActive);
            stdCenter?.ToDate = DateOnly.FromDateTime(DateTime.Now);
            stdCenter?.IsActive = false;
            //_context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "تم حذف الطالب من المركز بنجاح.", id);
        }

        public async Task<int> GetCenterStudentsCountAsync(long CenterId)
        {
            //var count =await _context.Students.Where(s => s.StdCenters.Any(sc => sc.CenterId == CenteId && sc.ToDate == null)).CountAsync();
            var count = await _context.Students.AsNoTracking().Where(s => s.StdCenters.First(x => x.IsActive).CenterId == CenterId).CountAsync();
            return count;
        }

        public async Task<GeneralResponse> AddStudentWithCenter(Student student, long centerid)
        {
            var Std = await _context.Students.Where(s => s.CivilId == student.CivilId /*|| s.Name.Trim() == student.Name.Trim()*/)
               .Include(x => x.StdCenters).ThenInclude(x => x.Center)
               .Include(x => x.Level)
               .FirstOrDefaultAsync();

            if (centerid <= 0)
                return new GeneralResponse(false, " يرجى تحديد المركز !", 0);
            else
                if (Std is null)
                {
                    //if(centerid == 0) return await Insert(student);

                    await _context.Database.BeginTransactionAsync();
                    _context.Students.Add(student);
                    await _context.SaveChangesAsync(); // للحصول على Id الطالب

                    var stdCenter = new StdCenter
                    {
                        StudentId = student.Id,
                        CenterId = centerid,
                        IsActive = true,
                        FromDate = DateOnly.FromDateTime(DateTime.Now)
                    };
                    _context.StdCenters.Add(stdCenter);
                    await _context.SaveChangesAsync();
                    await _context.Database.CommitTransactionAsync();

                    return new GeneralResponse(true, "تم إضافة الطالب للمركز بنجاح.");
                }

            var StdCenter = Std.StdCenters?.FirstOrDefault(x => x.IsActive);
            if (StdCenter is not null)
                return new GeneralResponse(false, $"رقم الهوية موجود مسبقاً في مركز {StdCenter?.Center?.Name} لطالب اسمه {Std.Name} في الصف {Std.Level?.Name} ", Std.Id);
            else
                return new GeneralResponse(false, $"  رقم الهوية موجود مسبقاً لطالب اسمه / {Std.Name} ،  في الصف / {Std.Level?.Name} ، وغير مسجل في أي مركز ، هل تريد إضافته في مركزكم ؟  ", Std.Id);

        }
        public async Task<GeneralResponse> UpdateStudentWithCenter(Student student, long centerid)
        {
            var oldStd = await _context.Students.FindAsync(student.Id);

            var DubStd = await _context.Students.Where(s => s.CivilId == student.CivilId && s.Id != student.Id)
                .Include(x => x.StdCenters).ThenInclude(x => x.Center)
               .Include(x => x.Level)
               .FirstOrDefaultAsync();
            //var IsStdExist = await _context.Students.AnyAsync(s => s.CivilId == student.CivilId);

            if (centerid <= 0)
                return new GeneralResponse(false, " يرجى تحديد المركز !", 0);
            else if (DubStd is not null)
            {
                return new GeneralResponse(false, $"رقم الهوية موجود مسبقاً في مركز {DubStd.StdCenters.FirstOrDefault(x => x.IsActive)?.Center?.Name} لطالب اسمه {DubStd.Name} في الصف {DubStd.Level?.Name} ", DubStd.Id);
            }
            else
                if (oldStd is not null)
                {
                    await _context.Database.BeginTransactionAsync();

                    //_context.Students.Update(student);
                    //await _context.SaveChangesAsync(); 

                    var StdCenters = _context.StdCenters.Where(x => x.StudentId == student.Id && x.IsActive).ToList();
                    if (StdCenters is null || StdCenters?.FirstOrDefault()?.CenterId != centerid)
                    {
                        StdCenters.ForEach(x =>
                        {
                            x.ToDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
                            x.IsActive = false;
                        });

                        //await _context.SaveChangesAsync();

                        var stdCenter = new StdCenter
                        {
                            StudentId = student.Id,
                            CenterId = centerid,
                            IsActive = true,
                            FromDate = DateOnly.FromDateTime(DateTime.Now)
                        };
                        _context.StdCenters.Add(stdCenter);
                    }
                    else
                    {
                        oldStd.Name = student.Name;
                        oldStd.CivilId = student.CivilId;
                        oldStd.EnName = student.EnName;
                        oldStd.Mobile = student.Mobile;
                        oldStd.LevelId = student.LevelId;
                        oldStd.GenderId = student.GenderId;
                        oldStd.SectionNo = student.SectionNo;
                        oldStd.BirthDate = student.BirthDate;
                        oldStd.IsUnrwa = student.IsUnrwa;
                        oldStd.IsSpecialNeeds = student.IsSpecialNeeds;
                        oldStd.SpecialNeeds = student.SpecialNeeds;
                        oldStd.Comments = student.Comments;
                    }

                    await _context.SaveChangesAsync();
                    await _context.Database.CommitTransactionAsync();


                    return new GeneralResponse(true, "تم تعديل بيانات الطالب في المركز بنجاح.");
                }

            return new GeneralResponse(false, $" الطالب غير موجود", 0);
        }

        public async Task<PaginatedResponse<StudentDto>> GetPaginatedStudentsAsync(StudentFilterRequest request, long CenterId = 0)
        {
            // 1. بناء الاستعلام الأساسي مع Include
            var query = _context.Students
                .AsNoTracking()
                .Where(x => CenterId == 0 ? true : x.StdCenters.FirstOrDefault(z => z.IsActive).CenterId == CenterId)
                .Include(s => s.Gender)
                .Include(s => s.Level)
                //.Include(x => x.StdCenters).ThenInclude(x => x.Center)
                .AsQueryable();

            // 2. تطبيق الفلاتر
            // البحث النصي
            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                var term = request.SearchText.Trim().ToLower();
                query = query.Where(s =>
                    (s.Name != null && s.Name.ToLower().Contains(term)) ||
                    (s.EnName != null && s.EnName.ToLower().Contains(term)) ||
                    (s.CivilId != null && s.CivilId.Contains(term)));
            }

            // فلتر الجنس
            if (!string.IsNullOrWhiteSpace(request.Gender))
            {
                query = query.Where(s =>
                    s.Gender != null && s.Gender.Name == request.Gender);
            }

            // فلتر المستوى
            if (!string.IsNullOrWhiteSpace(request.Level))
            {
                query = query.Where(s =>
                    s.Level != null && s.Level.Name == request.Level);
            }
            // فلتر الشعبة
            if (request.Section is not null && request.Section > 0)
            {
                query = query.Where(s =>
                    s.Level != null && s.SectionNo == request.Section);
            }

            if (request.ActiveCenters)
            {
                query = query.Where(x => x.StdCenters.Any(c => c.IsActive && c.Center.IsActive));
            }

            // فلتر المركز
            if (!string.IsNullOrWhiteSpace(request.Center))
            {
                switch (request.Center)
                {
                    case "-2":
                        //query = query.Where(x=>x.StdCenters.Count(c=>c.IsActive)<0);
                        break;
                    case "-1":
                        query = query.Where(x => x.StdCenters.Count(c => c.IsActive) > 0);
                        break;
                    case "0":
                        query = query.Where(x => x.StdCenters.Count(c => c.IsActive) <= 0);
                        break;
                    default:
                        query = query.Where(s => s.StdCenters.FirstOrDefault(z => z.IsActive).Center.Name == request.Center);
                        break;
                }
                //query = query.Where(s =>
                //    s.StdCenters.OrderByDescending(x=>x.FromDate).FirstOrDefault().Center.Name == request.Center);
            }

            // فلتر تاريخ الإضافة
            if (request.FromDate.HasValue)
            {
                query = query.Where(s =>
                    s.StdCenters.Any(sc => sc.FromDate >= request.FromDate.Value));
            }

            // 3. حساب العدد الإجمالي (بعد الفلترة)
            var totalCount = await query.CountAsync();

            // 4. تطبيق الترتيب والتقسيم
            var pageSize = Math.Clamp(request.PageSize, 1, 100);
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var currentPage = Math.Clamp(request.Page, 1,
                              Math.Max(1, totalPages));

            var items = await query
                .Include(x => x.StdCenters).ThenInclude(x => x.Center)
                .OrderBy(s => s.Name)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var studentDtos = items.Select(s => new StudentDto()
            {
                Id = s.Id,
                Name = s.Name,
                CivilId = s.CivilId,
                Mobile = s.Mobile,
                GenderName = s.Gender?.Name,
                LevelName = s.Level?.Name,
                Section = s.SectionNo,
                CenterName = s.StdCenters.FirstOrDefault(x => x.IsActive)?.Center?.Name,
                AddedDate = s.StdCenters.FirstOrDefault(x => x.IsActive)?.FromDate,
            }).OrderByDescending(s => s.AddedDate).ToList();

            // 5. بناء الاستجابة
            var response = new PaginatedResponse<StudentDto>
            {
                Items = studentDtos,
                TotalCount = totalCount,
                CurrentPage = currentPage,
                PageSize = pageSize
            };

            return response;
        }
        public async Task<List<Student>> GetFilteredForExportAsync(
        StudentFilterRequest request, long centerId)
        {
            IQueryable<Student> query = _context.Students
                 .OrderBy(s => s.StdCenters
                     .FirstOrDefault(z => z.IsActive)!.CenterId)
                    .ThenBy(s => s.Level.SortOrder)
                .Include(s => s.StdCenters).ThenInclude(sc => sc.Center).AsQueryable();

            if (request.ActiveCenters)
            {
                query = query.Where(x => x.StdCenters.Any(c => c.IsActive && c.Center.IsActive));
            }

            if (centerId == 0)
            {
                switch (request.Center)
                {
                    case "-2":
                        //query = query.Where(x=>x.StdCenters.Count(c=>c.IsActive)<0);
                        break;
                    case "-1":
                        query = query.Where(x => x.StdCenters.Count(c => c.IsActive) > 0);
                        break;
                    case "0":
                        query = query.Where(x => x.StdCenters.Count(c => c.IsActive) <= 0);
                        break;
                    default:
                        if (request.Center is not null)
                            query = query.Where(s => s.StdCenters.FirstOrDefault(z => z.IsActive).Center.Name == request.Center);
                        break;
                }
                query = query
                    .Include(s => s.Gender)
                    .Include(s => s.Level)
                    .AsNoTracking()
                    //.OrderBy(s => s.StdCenters
                    // .FirstOrDefault(z => z.IsActive)!.CenterId)
                    //.ThenBy(s=>s.Level.SortOrder)
                    .AsQueryable();
            }
            else
            {
                query = _context.Students
                   .Where(s => s.StdCenters
                       .FirstOrDefault(z => z.IsActive)!.CenterId == centerId)
                   .Include(s => s.StdCenters).ThenInclude(sc => sc.Center)
                   .Include(s => s.Gender)
                   .Include(s => s.Level)
                   .AsNoTracking()
                   //.OrderBy(s => s.StdCenters
                   //    .OrderByDescending(sc => sc.FromDate)
                   //    .FirstOrDefault()!.CenterId)
                   //.ThenBy(s => s.Level.SortOrder)
                   .AsQueryable();
            }

            // نفس منطق الفلترة في GetPaginatedStudentsAsync
            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                var term = request.SearchText.Trim().ToLower();
                query = query.Where(s =>
                    (s.Name != null && s.Name.ToLower().Contains(term)) ||
                    (s.EnName != null && s.EnName.ToLower().Contains(term)) ||
                    (s.CivilId != null && s.CivilId.Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(request.Gender))
                query = query.Where(s => s.Gender != null && s.Gender.Name == request.Gender);

            if (!string.IsNullOrWhiteSpace(request.Level))
                query = query.Where(s => s.Level != null && s.Level.Name == request.Level);

            if (request.FromDate.HasValue)
                query = query.Where(s =>
                    s.StdCenters.Any(sc => sc.FromDate >= request.FromDate.Value));

            return await query/*.OrderBy(s => s.Name)*/.ToListAsync();
        }

        public async Task<List<Student>> GetAllByCenterAsync(long centerId)
        {
            if (centerId == 0)
            {
                return await _context.Students
                     .AsNoTracking()
                    .Include(s => s.StdCenters).ThenInclude(sc => sc.Center)
                     .Include(s => s.Gender)
                     .Include(s => s.Level)
                     .OrderBy(s => s.StdCenters
                     .FirstOrDefault(z => z.IsActive)!.CenterId)
                     .ThenBy(s => s.Level.SortOrder)
                     .ToListAsync();

            }
            else
            {
                return await _context.Students
                    .Where(s => s.StdCenters
                        .FirstOrDefault(z => z.IsActive)!.CenterId == centerId)
                    .Include(s => s.StdCenters).ThenInclude(sc => sc.Center)
                    .Include(s => s.Gender)
                    .Include(s => s.Level)
                    .AsNoTracking()
                    .OrderBy(s => s.StdCenters
                        .FirstOrDefault(z => z.IsActive)!.CenterId)
                    .ThenBy(s => s.Level.SortOrder)
                    .ToListAsync();
            }


        }

        public async Task<GeneralResponse> DeleteFromDBAsync(long studentId)
        {
            var student = await _context.Students.Include(x => x.StdCenters).Where(x => x.Id == studentId).FirstOrDefaultAsync();
            if (student == null)
                return new GeneralResponse(false, "الطالب غير موجود.", 0);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "تم حذف الطالب نهائياً من قاعدة البيانات بنجاح.", studentId);

        }

        #region Std Counts
        public async Task<List<StudentsCountsDto>> GetTotalStudentsCountAsync(StudentsCountsRequestDto request)
        {
            if (request == null) return new List<StudentsCountsDto>();
            if (request.CenterId.HasValue && request.CenterId.Value > 0)
            {
                List<StudentsCountsDto> StdCounts = new List<StudentsCountsDto>();
                for (DateOnly currentDate = request.From.Value; currentDate <= request.To.Value; currentDate = currentDate.AddDays(1))
                {
                    StdCounts.Add(new StudentsCountsDto
                    {
                        date = currentDate,
                        StdCount = _context.StdCenters.Count(sc => sc.CenterId == request.CenterId.Value &&
                            sc.FromDate <= currentDate &&
                            (sc.ToDate == null || sc.ToDate >= currentDate))
                    });
                }

                return StdCounts;
                //return await _context.StdCenters
                //    .Where(sc => sc.CenterId == request.CenterId.Value
                //                 && (!request.From.HasValue || sc.FromDate >= request.From.Value)
                //                 && (!request.To.HasValue || sc.FromDate <= request.To.Value))
                //    .GroupBy(sc => sc.FromDate)
                //    .Select(g => new StudentsCountsDto
                //    {
                //        date = g.Key,
                //        StdCount = g.Count()
                //    })
                //    .ToListAsync();
            }
            else
            {
                List<StudentsCountsDto> StdCounts = new List<StudentsCountsDto>();
                for (DateOnly currentDate = request.From.Value; currentDate <= request.To.Value; currentDate = currentDate.AddDays(1))
                {
                    StdCounts.Add(new StudentsCountsDto
                    {
                        date = currentDate,
                        StdCount = _context.StdCenters.Count(sc =>
                            sc.FromDate <= currentDate &&
                            (sc.ToDate == null || sc.ToDate >= currentDate))
                    });
                }

                return StdCounts;
                //return await _context.Students
                //    .Where(s => s.StdCenters.Any(sc =>
                //                 (!request.From.HasValue || sc.FromDate >= request.From.Value) &&
                //                 (!request.To.HasValue || sc.FromDate <= request.To.Value)))
                //    .SelectMany(s => s.StdCenters)
                //    .GroupBy(sc => sc.FromDate)
                //    .Select(g => new StudentsCountsDto
                //    {
                //        date = g.Key,
                //        StdCount = g.Count()
                //    })
                //    .ToListAsync();
            }
        }
        #endregion


        public async Task<GeneralResponse> PromotionStudentsAsync(long FromLevelId, long ToLevelId)
        {
            var Levels = await _context.LookupValues.ToListAsync();
            var students = await _context.Students.Where(s => s.LevelId == FromLevelId).ToListAsync();
            if (ToLevelId == -100)
            {
                _context.Students.RemoveRange(students);
            }
            else
                foreach (var student in students)
                {
                    student.LevelId = ToLevelId;
                }
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, $"تم ترقية {students.Count} الطلاب من المستوى {Levels.FirstOrDefault(l=>l.Id==FromLevelId)?.Name} إلى المستوى {Levels.FirstOrDefault(l => l.Id == ToLevelId)?.Name}.", students.Count);
        }

    }
 }