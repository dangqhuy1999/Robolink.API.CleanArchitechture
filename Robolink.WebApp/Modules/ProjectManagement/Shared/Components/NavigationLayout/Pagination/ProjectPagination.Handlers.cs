using Microsoft.AspNetCore.Components;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Components.NavigationLayout.Pagination
{
    public partial class ProjectPagination : ComponentBase
    {

        // ===== HÀM HANDLER TỔNG LỰC =====
        private async Task HandlePageSelection(int targetPage)
        {
            // 1. Chặn các trường hợp vô lý:
            // - Trang mục tiêu nhỏ hơn 1
            // - Trang mục tiêu lớn hơn tổng số trang
            // - Trang mục tiêu trùng với trang hiện tại (không cần load lại)
            if (targetPage < 1 || targetPage > TotalPages || targetPage == CurrentPage)
            {
                return;
            }

            // 2. Nếu mọi thứ ổn, bắn Event báo cho thằng Cha
            await OnPageChanged.InvokeAsync(targetPage);
        }
    }
}
