using Microsoft.AspNetCore.Mvc;
using Services;
using System.Threading;
using System.Threading.Tasks;

namespace Web.UI.ViewComponents
{
    public class PersonJobInfoViewComponent : ViewComponent
    {
        private readonly IPersonService personService;

        public PersonJobInfoViewComponent(IPersonService personService)
        {
            this.personService = personService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int personId, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult((IViewComponentResult)View("PersonJobInfo", await personService.GetJobInfo(personId, cancellationToken)));
        }
    }
}
