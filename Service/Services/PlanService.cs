using Service.Interfaces;
using Service.Models;

namespace Service.Services
{
    public class PlanService : GenericService<Plan>, IPlanService
    {
        public PlanService(HttpClient? httpClient = null) : base(httpClient)
        {
        }
    }
}