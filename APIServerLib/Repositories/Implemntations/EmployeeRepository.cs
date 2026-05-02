using APIServerLib.Data;
using APIServerLib.Repositories.Interfaces;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Responses;

namespace APIServerLib.Repositories.Implemntations
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetAll()
        {
            return await _context.Employees.AsNoTracking().Include(x => x.EmpCenters).ThenInclude(x => x.Center).ToListAsync();
        }

        public async Task<Employee> GetById(long id)
        {
            return await _context.Employees.Where(x => x.Id == id).Include(x => x.EmpCenters).ThenInclude(x => x.Center).FirstOrDefaultAsync();
        }

        public async Task<GeneralResponse> Insert(Employee item)
        {
            _context.Employees.Add(item);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Employee added successfully.", item.Id);
        }

        public async Task<GeneralResponse> AddEmployeeWithCenter(Employee employee, long centerid)
        {
            var emp = await _context.Employees.Where(s => s.EmpId == employee.EmpId && s.CivilId==employee.CivilId)
               .Include(x => x.EmpCenters).ThenInclude(x => x.Center)
               .Include(x => x.Specialization)
               .FirstOrDefaultAsync();

            if (centerid <= 0)
                return new GeneralResponse(false, " يرجى تحديد المركز !", 0);
            else
                if (emp is null)
                {
                    await _context.Database.BeginTransactionAsync();
                    _context.Employees.Add(employee);
                    await _context.SaveChangesAsync(); // للحصول على Id employee بعد الحفظ

                    var empCenter = new EmpCenter
                    {
                        EmployeeId = employee.Id,
                        CenterId = centerid,
                        FromDate = DateOnly.FromDateTime(DateTime.Now)
                    };
                    _context.EmpCenters.Add(empCenter);
                    await _context.SaveChangesAsync();
                    await _context.Database.CommitTransactionAsync();

                    return new GeneralResponse(true, "تم إضافة الموظف للمركز بنجاح.");
                }
            if (emp.EmpCenters is null || emp.EmpCenters.Count() == 0)
                return new GeneralResponse(false, $"رقم الهوية أو رقم الموظف موجود مسبقاً في مركز ", 0);
            else
                return new GeneralResponse(false, $"رقم الهوية موجود مسبقاً لموظف اسمه {emp.Name} في التخصص {emp.Specialization?.Name} ، وغير مسجل في أي مركز ، هل تريد إضافته في مركزكم ؟ ", 0);
        }

        public async Task<GeneralResponse> Update(Employee item)
        {

            /////// تحديث بيانات الموظف
            _context.Employees.Update(item);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Employee updated successfully.");
        }

        public async Task<GeneralResponse> UpdateWithCenter(EmployeeUpsertDto item)
        {
            var emp = await _context.Employees.Where(x => x.EmpId == item.EmpId && x.CivilId == item.CivilId)
                .Include(x => x.EmpCenters).ThenInclude(x => x.Center)
                .FirstOrDefaultAsync();

            if (item.CenterId is null || item.CenterId <= 0 )
                return new GeneralResponse(false, " يرجى تحديد المركز !", 0);
            else
            
                if (emp is not null)
                {
                    var EmpCenters = _context.EmpCenters.Where(x => x.EmployeeId == emp.Id && x.IsActive).ToList();
                    await _context.Database.BeginTransactionAsync();
                    if (EmpCenters.FirstOrDefault()?.CenterId == item.CenterId)
                    {
                        emp.Name = item.Name;
                        emp.EnName = item.EnName;
                        emp.Mobile = item.Mobile;
                        emp.GenderId = item.GenderId;
                        emp.JobId = item.JobId;
                        emp.OrgJobId = item.OrgJobId;
                        emp.SpecializationId = item.SpecializationId;
                        emp.Address = item.Address;
                        emp.BirthDate = item.BirthDate;
                        emp.Comments = item.Comments;
                        emp.OrgSchool = item.OrgSchool;

                    _context.Employees.Update(emp);
                    }
                    //await _context.SaveChangesAsync(); 
                    else
                    //if (EmpCenters.FirstOrDefault()?.CenterId != item.CenterId)
                    {
                        EmpCenters.ForEach(x =>
                        {
                            x.ToDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
                            x.IsActive = false;
                        });

                        await _context.SaveChangesAsync();

                        var empCenter = new EmpCenter
                        {
                            EmployeeId = emp.Id,
                            CenterId = item.CenterId ?? 0,
                            IsActive = true,
                            FromDate = DateOnly.FromDateTime(DateTime.Now)
                        };
                        _context.EmpCenters.Add(empCenter);

                    }

                    await _context.SaveChangesAsync();
                    await _context.Database.CommitTransactionAsync();


                    return new GeneralResponse(true, "تم تعديل بيانات الموظف في المركز بنجاح.");
                }

                return new GeneralResponse(false, $"رقم الهوية أو الاسم موجود مسبقاً في مركز {emp.EmpCenters?.OrderByDescending(x => x.FromDate).First().Center?.Name} لموظف اسمه {emp.Name} في التخصص {emp.Specialization?.Name} ", 0);

                //    var employeeToSave = (new EmployeeMapper()).ToEntity(item);

                ////var managers = await GetAllManagers();
                ////long EmpCurCenterId = managers.FirstOrDefault(x => x.EmpId == item.EmpId)?.EmpCenters.OrderByDescending(ec => ec.FromDate).FirstOrDefault()?.CenterId ?? 0;
                //long EmpCurCenterId = _context.Employees
                //    .AsNoTracking()
                //    .Where(x => x.EmpId == item.EmpId)
                //    .Include(x => x.EmpCenters)
                //    .FirstOrDefault()?
                //    .EmpCenters.OrderByDescending(ec => ec.FromDate)
                //    .FirstOrDefault()?
                //    .CenterId ?? 0;

                //await _context.Database.BeginTransactionAsync();
                //if (EmpCurCenterId != 0 && EmpCurCenterId != item.CenterId)
                //{
                //    _context.EmpCenters.Where(x => x.EmployeeId == item.Id).OrderByDescending(x => x.FromDate).FirstOrDefault()?.ToDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
                //    // تم تغيير مركز الموظف، قم بتحديث EmpCenters
                //   _context.EmpCenters.Add(new EmpCenter() { FromDate=DateOnly.FromDateTime(DateTime.Now),EmployeeId = employeeToSave.Id, CenterId = item.CenterId ?? 0});
                //}

                ///////// تحديث بيانات الموظف
                //_context.Employees.Update(employeeToSave);
                //await _context.SaveChangesAsync();

                //await _context.Database.CommitTransactionAsync();
                //return new GeneralResponse(true, "تم تحديث بيانات الموظف.");
            }

        public async Task<GeneralResponse> DeleteById(long id)
        {
            var employee = await _context.Employees.Include(e=>e.EmpCenters.Where(x=>x.IsActive)).ThenInclude(x=>x.Center).Where(x=>x.Id==id).FirstOrDefaultAsync();

            if (employee == null)
                return new GeneralResponse(false, "الموظف غير موجود.");

            var empCenter = employee.EmpCenters.FirstOrDefault(x => x.IsActive);
            empCenter?.ToDate = DateOnly.FromDateTime(DateTime.Now);
            empCenter?.IsActive = false;
            //_context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "الموظف تم حذفه بنجاح.");
        }

        public async Task<int> GetCenterEmployeesCountAsync(long CenterId)
        {
            var count = await _context.Employees.Where(e => e.EmpCenters.OrderByDescending(x => x.FromDate).First().CenterId == CenterId).CountAsync();
            return count;
        }

        public async Task<EmployeePaginatedResponse> GetPaginatedEmployesAsync(EmployeeFilterRequest request, long CenterId = 0)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
            IQueryable<Employee> query;

            //if (CenterId == 0)
            //{
            //     query = _context.Employees
            //       .AsNoTracking()
            //       .Include(e => e.Gender)
            //       .Include(e => e.Job)
            //       .Include(e => e.Specialization)
            //       .AsQueryable();
            //}
            //else
            //{
            query = _context.Employees
               .Where(e => CenterId == 0 ? true : e.EmpCenters.FirstOrDefault(x => x.IsActive).CenterId == CenterId)
               .AsNoTracking()
               .OrderBy(e => e.EmpCenters.FirstOrDefault(x => x.IsActive).Center.Name)
               .Include(e => e.Gender)
               .Include(e => e.Job)
               .Include(e => e.Specialization)
               .Include(x => x.EmpCenters).ThenInclude(x => x.Center)
               .AsQueryable();
            //}

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                var term = request.SearchText.Trim();
                query = query.Where(e =>
                    e.Name.Contains(term) ||
                    (e.EnName != null && e.EnName.Contains(term)) ||
                    (e.CivilId != null && e.CivilId.Contains(term)) ||
                    (e.EmpId != null && e.EmpId.Contains(term))
                    );
            }

            if (!string.IsNullOrWhiteSpace(request.Gender))
            {
                query = query.Where(e => e.Gender != null && e.Gender.Name == request.Gender);
            }

            if (!string.IsNullOrWhiteSpace(request.Job))
            {
                query = query.Where(e => e.Job != null && e.Job.Name == request.Job);
            }

            if (!string.IsNullOrWhiteSpace(request.Center))
            {
                query = query.Where(e => e.EmpCenters != null && e.EmpCenters.FirstOrDefault(x => x.IsActive).Center.Name == request.Center);
            }

            // فلتر تاريخ الإضافة
            if (request.FromDate.HasValue)
            {
                query = query.Where(e =>
                    e.EmpCenters.Any(ec => ec.FromDate >= request.FromDate.Value));
            }

            var totalCount = await query.CountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling((double)totalCount / pageSize));
            if (pageNumber > totalPages)
            {
                pageNumber = totalPages;
            }

            var items = await query
                .OrderBy(e => e.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EmployeeListItemDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    EnName = e.EnName,
                    EmpId = e.EmpId,
                    CivilId = e.CivilId,
                    Mobile = e.Mobile,
                    GenderName = e.Gender != null ? e.Gender.Name : null,
                    JobName = e.Job != null ? e.Job.Name : null,
                    SpecializationName = e.Specialization != null ? e.Specialization.Name : null,
                    CenterName = e.EmpCenters.FirstOrDefault(x => x.IsActive).Center.Name,
                    AddedDate = e.EmpCenters.FirstOrDefault(x => x.IsActive).FromDate,  // ← جديد
                })
                .ToListAsync();

            // الخيارات لاستخراجها مرة واحدة داخل نفس endpoint.
            var genderOptions = await _context.Employees
                .AsNoTracking()
                .Include(e => e.Gender)
                .Where(e => e.Gender != null)
                .Select(e => e.Gender!.Name!)
                .Distinct()
                .OrderBy(v => v)
                .ToListAsync();

            var jobOptions = await _context.Employees
                .AsNoTracking()
                .Include(e => e.Job)
                .Where(e => e.Job != null)
                .Select(e => e.Job!.Name!)
                .Distinct()
                .OrderBy(v => v)
                .ToListAsync();


            return new EmployeePaginatedResponse
            {
                Items = items,
                TotalCount = totalCount,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                GenderOptions = genderOptions,
                JobOptions = jobOptions,
                CenterOptions = await _context.Centers.AsNoTracking().Select(c => c.Name!).ToListAsync()
            };
        }

        public async Task<EmployeeUpsertDto?> GetByCivilId(string CivilId)
        {
            var E = await _context.Employees.Where(x => x.CivilId == CivilId)
                .Include(x => x.EmpCenters).ThenInclude(x => x.Center).FirstOrDefaultAsync();
            if (E is null) return null;

            var R = (new EmployeeMapper()).ToEmployeeUpsertDTO(E);
            R.CenterName = E.EmpCenters.FirstOrDefault(x => x.IsActive)?.Center.Name;
            return R;
        }

        public async Task<EmployeeUpsertDto?> GetByEmpId(string EmpId)
        {
            var E = await _context.Employees.Where(x => x.EmpId == EmpId)
                .Include(x => x.EmpCenters).ThenInclude(x => x.Center).FirstOrDefaultAsync();
            if (E is null) return null;

            var R = (new EmployeeMapper()).ToEmployeeUpsertDTO(E);
            R.CenterName = E.EmpCenters.FirstOrDefault(x=>x.IsActive)?.Center.Name;
            return R;
        }

        // ╔══════════════════════════════════════════════════════════════╗
        // ║  APIServerLib/Repositories/Implemntations/EmployeeRepository ║
        // ║  أضف هاتين الدالتين في نهاية الـ class الموجود              ║
        // ╚══════════════════════════════════════════════════════════════╝

        public async Task<List<EmployeeListItemDto>> GetFilteredForExportAsync(
            EmployeeFilterRequest request, long centerId)
        {
            IQueryable<Employee> query;
            if (centerId == 0)
            {
                query = _context.Employees
                    .Include(s => s.EmpCenters).ThenInclude(sc => sc.Center)
                .AsNoTracking()
                .Include(e => e.Gender)
                .Include(e => e.Job)
                .Include(e => e.OrgJob)
                .Include(e => e.Specialization)
                .AsQueryable();
            }
            else
            {
                query = _context.Employees
                    .Include(s => s.EmpCenters).ThenInclude(sc => sc.Center)
                .Where(e => e.EmpCenters
                    .OrderByDescending(ec => ec.FromDate)
                    .FirstOrDefault()!.CenterId == centerId)
                .AsNoTracking()
                .Include(e => e.Gender)
                .Include(e => e.Job)
                .Include(e => e.OrgJob)
                .Include(e => e.Specialization)
                .AsQueryable();
            }

            // نفس منطق الفلترة في GetPaginatedEmployesAsync
            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                var term = request.SearchText.Trim();
                query = query.Where(e =>
                    e.Name.Contains(term) ||
                    (e.EnName != null && e.EnName.Contains(term)) ||
                    (e.CivilId != null && e.CivilId.Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(request.Gender))
                query = query.Where(e => e.Gender != null && e.Gender.Name == request.Gender);

            if (!string.IsNullOrWhiteSpace(request.Job))
                query = query.Where(e => e.Job != null && e.Job.Name == request.Job);

            if (!string.IsNullOrWhiteSpace(request.Center))
                query = query.Where(e => e.EmpCenters != null && e.EmpCenters.FirstOrDefault(x=>x.IsActive).Center.Name == request.Center);

            if (request.FromDate.HasValue)
                query = query.Where(e =>
                    e.EmpCenters.Any(ec => ec.FromDate >= request.FromDate.Value));

            return await query
                .OrderBy(e => e.Name)
                .Select(e => new EmployeeListItemDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    EnName = e.EnName,
                    EmpId = e.EmpId,
                    CivilId = e.CivilId,
                    Mobile = e.Mobile,
                    GenderName = e.Gender != null ? e.Gender.Name : null,
                    JobName = e.Job != null ? e.Job.Name : null,
                    SpecializationName = e.Specialization != null ? e.Specialization.Name : null,
                    CenterName = e.EmpCenters.FirstOrDefault(x => x.IsActive).Center.Name,
                    AddedDate = e.EmpCenters.FirstOrDefault(x => x.IsActive).FromDate,  // ← جديد
                })
                .ToListAsync();
        }

        public async Task<List<EmployeeListItemDto>> GetAllByCenterAsync(long centerId)
        {
            if (centerId == 0)
            {
                return await _context.Employees
                    .Include(s => s.EmpCenters).ThenInclude(sc => sc.Center)
               .AsNoTracking()
               .Include(e => e.Gender)
               .Include(e => e.Job)
               .Include(e => e.Specialization)
               .OrderBy(e => e.Job!.Name)
               .ThenBy(e => e.Name)
               .Select(e => new EmployeeListItemDto
               {
                   Id = e.Id,
                   Name = e.Name,
                   EnName = e.EnName,
                   EmpId = e.EmpId,
                   CivilId = e.CivilId,
                   Mobile = e.Mobile,
                   GenderName = e.Gender != null ? e.Gender.Name : null,
                   JobName = e.Job != null ? e.Job.Name : null,
                   SpecializationName = e.Specialization != null ? e.Specialization.Name : null,
                   CenterName = e.EmpCenters.FirstOrDefault(x => x.IsActive).Center.Name
               })
               .ToListAsync();
            }
            else
                return await _context.Employees
                        .Include(s => s.EmpCenters).ThenInclude(sc => sc.Center)
                    .Where(e => e.EmpCenters
                        .FirstOrDefault(x => x.IsActive)!.CenterId == centerId)
                    .AsNoTracking()
                    .Include(e => e.Gender)
                    .Include(e => e.Job)
                    .Include(e => e.Specialization)
                    .OrderBy(e => e.Job!.Name)
                    .ThenBy(e => e.Name)
                    .Select(e => new EmployeeListItemDto
                    {
                        Id = e.Id,
                        Name = e.Name,
                        EnName = e.EnName,
                        EmpId = e.EmpId,
                        CivilId = e.CivilId,
                        Mobile = e.Mobile,
                        GenderName = e.Gender != null ? e.Gender.Name : null,
                        JobName = e.Job != null ? e.Job.Name : null,
                        SpecializationName = e.Specialization != null ? e.Specialization.Name : null,
                        CenterName = e.EmpCenters.FirstOrDefault(x => x.IsActive).Center.Name,
                        AddedDate = e.EmpCenters.FirstOrDefault(x => x.IsActive).FromDate,  // ← جديد
                    })
                    .ToListAsync();
        }

        public async Task<List<Employee>> GetAllManagers()
        {
            return await _context.Employees
                .Where(e => e.Job.Name == "مدير مركز" && e.EmpCenters.FirstOrDefault(x => x.IsActive) != null)
                .AsNoTracking()
                .Include(e => e.EmpCenters).ThenInclude(ec => ec.Center)
                .ToListAsync();
        }
        public async Task<Employee> GetCenterManagers(long centerId)
        {
            var managers = await GetAllManagers();
            return managers.FirstOrDefault(m => m.EmpCenters.OrderByDescending(d => d.FromDate).FirstOrDefault().CenterId == centerId);

        }

        public async Task<Employee?> IsCivilIdDuplicateAsync(EmployeeDuplicateCheckRequest request)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.CivilId == request.CivilId && e.Id != request.ExcludeEmployeeId);
        }
        public async Task<Employee?> IsEmpIdDuplicateAsync(EmployeeDuplicateCheckRequest request)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.EmpId == request.EmpId && e.Id != request.ExcludeEmployeeId);
        }
        public async Task<Employee?> IsEmployeeDuplicateAsync(EmployeeDuplicateCheckRequest request)
        {
            return await _context.Employees.Include(e => e.EmpCenters.Where(c=>c.IsActive)).ThenInclude(ec => ec.Center)
                .FirstOrDefaultAsync(e => (e.EmpId == request.EmpId || e.CivilId == request.CivilId) && e.Id != request.ExcludeEmployeeId);
        }
    }
}