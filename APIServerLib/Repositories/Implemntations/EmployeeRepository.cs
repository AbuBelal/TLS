using APIServerLib.Data;
using APIServerLib.Repositories.Interfaces;
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
            return await _context.Employees.AsNoTracking().Include(x=>x.EmpCenters).ThenInclude(x=>x.Center).ToListAsync();
        }

        public async Task<Employee> GetById(long id)
        {
            return await _context.Employees.Where(x=> x.Id == id).Include(x=>x.EmpCenters).FirstOrDefaultAsync();
        }

        public async Task<GeneralResponse> Insert(Employee item)
        {
            _context.Employees.Add(item);
            await _context.SaveChangesAsync();
            return new GeneralResponse (  true,  "Employee added successfully." ,item.Id);
        }

        public async Task<GeneralResponse> AddEmployeeWithCenter(Employee employee, long centerid)
        {
            var emp = await _context.Employees.Where(s => s.EmpId == employee.EmpId)
               .Include(x => x.EmpCenters).ThenInclude(x => x.Center)
               .Include(x => x.Specialization).FirstOrDefaultAsync();

            if (emp is null)
            {

                if (centerid == 0) return await Insert(employee);

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
            return new GeneralResponse(false, $"رقم الهوية موجود مسبقاً في مركز {emp.EmpCenters.OrderByDescending(x => x.FromDate).First().Center.Name} لموظف اسمه {emp.Name} في التخصص {emp.Specialization.Name} ", 0);
        }

        public async Task<GeneralResponse> Update(Employee item)
        {
            _context.Employees.Update(item);
            await _context.SaveChangesAsync();
            return new GeneralResponse( true,  "Employee updated successfully." );
        }

        public async Task<GeneralResponse> DeleteById(long id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return new GeneralResponse (false,  "Employee not found." );

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return new GeneralResponse  ( true, "Employee deleted successfully." );
        }

        public async Task<int> GetCenterEmployeesCountAsync(long CenterId)
        {
           var count =await _context.Employees.Where(e => e.EmpCenters.OrderByDescending(x => x.FromDate).First().CenterId == CenterId).CountAsync();
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
                    .Where(e => CenterId==0 ? true: e.EmpCenters.OrderByDescending(x => x.FromDate).FirstOrDefault().CenterId == CenterId)
                    .AsNoTracking()
                    .Include(e => e.Gender)
                    .Include(e => e.Job)
                    .Include(e => e.Specialization)
                    .Include(x=> x.EmpCenters).ThenInclude(x=>x.Center)
                    .AsQueryable();
            //}

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                var term = request.SearchText.Trim();
                query = query.Where(e =>
                    e.Name.Contains(term) ||
                    (e.EnName != null && e.EnName.Contains(term)) ||
                    (e.CivilId != null && e.CivilId.Contains(term)));
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
                query = query.Where(e => e.EmpCenters != null && e.EmpCenters.OrderByDescending(x=>x.FromDate).FirstOrDefault().Center.Name == request.Center);
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
                    EmpId= e.EmpId,
                    CivilId = e.CivilId,
                    Mobile = e.Mobile,
                    GenderName = e.Gender != null ? e.Gender.Name : null,
                    JobName = e.Job != null ? e.Job.Name : null,
                    SpecializationName = e.Specialization != null ? e.Specialization.Name : null,
                    CenterName = e.EmpCenters.OrderByDescending(x => x.FromDate).FirstOrDefault().Center.Name,
                    AddedDate = e.EmpCenters.OrderByDescending(x => x.FromDate).FirstOrDefault().FromDate,  // ← جديد
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
                .Include(x=>x.EmpCenters).ThenInclude(x=>x.Center).FirstOrDefaultAsync();
            if (E is null) return null;

           var R =(new EmployeeMapper()).ToEmployeeUpsertDTO(E);
            R.CenterName = E.EmpCenters.OrderByDescending(x => x.FromDate).FirstOrDefault()?.Center.Name;
            return R;
        }

        public async Task<EmployeeUpsertDto?> GetByEmpId(string EmpId)
        {
            var E = await _context.Employees.Where(x => x.EmpId == EmpId)
                .Include(x => x.EmpCenters).ThenInclude(x => x.Center).FirstOrDefaultAsync();
            if (E is null) return null;

           var R =(new EmployeeMapper()).ToEmployeeUpsertDTO(E);
            R.CenterName = E.EmpCenters.OrderByDescending(x => x.FromDate).FirstOrDefault()?.Center.Name;
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
                query = query.Where(e => e.EmpCenters != null && e.EmpCenters.OrderByDescending(x=>x.FromDate).FirstOrDefault().Center.Name == request.Center);

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
                    CenterName = e.EmpCenters.OrderByDescending(x => x.FromDate).FirstOrDefault().Center.Name,
                    AddedDate = e.EmpCenters.OrderByDescending(x => x.FromDate).FirstOrDefault().FromDate,  // ← جديد
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
                   CenterName = e.EmpCenters.OrderByDescending(x => x.FromDate).FirstOrDefault().Center.Name
               })
               .ToListAsync();
            }
            else
            return await _context.Employees
                    .Include(s => s.EmpCenters).ThenInclude(sc => sc.Center)
                .Where(e => e.EmpCenters
                    .OrderByDescending(ec => ec.FromDate)
                    .FirstOrDefault()!.CenterId == centerId)
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
                    CenterName=e.EmpCenters.OrderByDescending(x=>x.FromDate).FirstOrDefault().Center.Name,
                    AddedDate = e.EmpCenters.OrderByDescending(x => x.FromDate).FirstOrDefault().FromDate,  // ← جديد
                })
                .ToListAsync();
        }
    }
}