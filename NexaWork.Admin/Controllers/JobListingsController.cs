using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NexaWork.Domain.Entities;
using NexaWork.Admin.Models;
using Serilog;
using NexaWork.Infrastructure.Persistence;

namespace NexaWork.Admin.Controllers
{
    // [Authorize]
    public class JobListingsController : Controller
    {
        private readonly NexaWorkDbContext _context;

        public JobListingsController(NexaWorkDbContext context)
        {
            _context = context;
        }

        // GET: JobListings
        public async Task<IActionResult> Index()
        {
            var nexaWorkDbContext = _context.JobListings.Include(j => j.Organization);
            return View(await nexaWorkDbContext.ToListAsync());
        }

        // GET: JobListings/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobListing = await _context.JobListings
                .Include(j => j.Organization)
                .FirstOrDefaultAsync(m => m.JobListingId == id);
            if (jobListing == null)
            {
                return NotFound();
            }

            return View(jobListing);
        }

        // GET: JobListings/Create
        public IActionResult Create()
        {
            ViewData["OrganizationId"] = new SelectList(_context.Organizations, "OrganizationId", "Name");
            return View();
        }

        // POST: JobListings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobListingViewModel jobListingViewModel)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    var jobListing = new JobListing
                    {
                        JobListingId = jobListingViewModel.JobListingId,
                        OrganizationId = jobListingViewModel.OrganizationId,
                        Title = jobListingViewModel.Title,
                        Description = jobListingViewModel.Description,
                        Requirements = jobListingViewModel.Requirements,
                        Location = jobListingViewModel.Location,
                        EmploymentType = jobListingViewModel.EmploymentType,
                        SalaryRange = jobListingViewModel.SalaryRange,
                        ContactEmail = jobListingViewModel.ContactEmail,
                        CreatedAt = jobListingViewModel.CreatedAt,
                        UpdatedAt = jobListingViewModel.UpdatedAt,
                        IsActive = jobListingViewModel.IsActive
                    };
                    jobListing.JobListingId = Guid.NewGuid();
                    _context.Add(jobListing);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (System.Exception ex)
                {

                    Log.Information("CREATE error in JobListingsController: {Message}", ex.Message);
                }


            }
            ViewData["OrganizationId"] = new SelectList(_context.Organizations, "OrganizationId", "Name", jobListingViewModel.OrganizationId);
            return View(jobListingViewModel);
        }

        // GET: JobListings/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobListing = await _context.JobListings.FindAsync(id);
            if (jobListing == null)
            {
                return NotFound();
            }
            ViewData["OrganizationId"] = new SelectList(_context.Organizations, "OrganizationId", "Name", jobListing.OrganizationId);
            return View(jobListing);
        }

        // POST: JobListings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("JobListingId,OrganizationId,Title,Description,Requirements,Location,EmploymentType,SalaryRange,ContactEmail,CreatedAt,UpdatedAt,IsActive")] JobListing jobListing)
        {
            if (id != jobListing.JobListingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jobListing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobListingExists(jobListing.JobListingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrganizationId"] = new SelectList(_context.Organizations, "OrganizationId", "Name", jobListing.OrganizationId);
            return View(jobListing);
        }

        // GET: JobListings/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobListing = await _context.JobListings
                .Include(j => j.Organization)
                .FirstOrDefaultAsync(m => m.JobListingId == id);
            if (jobListing == null)
            {
                return NotFound();
            }

            return View(jobListing);
        }

        // POST: JobListings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var jobListing = await _context.JobListings.FindAsync(id);
            if (jobListing != null)
            {
                _context.JobListings.Remove(jobListing);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobListingExists(Guid id)
        {
            return _context.JobListings.Any(e => e.JobListingId == id);
        }
    }
}
