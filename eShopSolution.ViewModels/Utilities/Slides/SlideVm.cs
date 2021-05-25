using eShopSolution.Data.Enums;

namespace eShopSolution.ViewModels.Utilities.Slides
{
    public class SlideVm
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string Email { set; get; }
        public string Description { set; get; }
        public string Url { set; get; }
        public int SortOrder { get; set; }
        public Status Status { set; get; }

        public string Image { get; set; }
    }
}