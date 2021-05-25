using eShopSolution.ViewModels.System.Languages;
using System.Collections.Generic;

namespace eShopeSolution.AddminApp.Models
{
    public class NavigationViewModel
    {
        public List<LanguageVm> Languages { get; set; }

        public string CurrentLanguageId { get; set; }
        public string ReturnUrl { get; set; }
    }
}