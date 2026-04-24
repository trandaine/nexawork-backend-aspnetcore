using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaWork.Infrastructure.Persistence;

namespace NexaWork.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly NexaWorkDbContext _nexaWorkDbContext;
        // private readonly ICurrentUserService _currentUserService;
        public CustomersController(NexaWorkDbContext nexaWorkDbContext)
        {
            _nexaWorkDbContext = nexaWorkDbContext;
        }




    }
}
