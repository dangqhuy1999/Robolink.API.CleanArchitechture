@echo off

set BASE=Projects\Components

echo Creating folders...

:: ===== INPUTS =====
mkdir %BASE%\System\Inputs\AppTextInput
mkdir %BASE%\System\Inputs\AppTextArea
mkdir %BASE%\System\Inputs\AppNumberInput
mkdir %BASE%\System\Inputs\AppSelect
mkdir %BASE%\System\Inputs\AppDatePicker
mkdir %BASE%\System\Inputs\AppTimePicker
mkdir %BASE%\System\Inputs\AppCheckbox
mkdir %BASE%\System\Inputs\AppRadio
mkdir %BASE%\System\Inputs\AppSwitch
mkdir %BASE%\System\Inputs\AppFileUpload

:: ===== ACTIONS =====
mkdir %BASE%\System\Actions\AppButton
mkdir %BASE%\System\Actions\AppIconButton
mkdir %BASE%\System\Actions\AppLink
mkdir %BASE%\System\Actions\AppDropdownButton

:: ===== DISPLAY =====
mkdir %BASE%\System\Display\AppBadge
mkdir %BASE%\System\Display\AppTag
mkdir %BASE%\System\Display\AppAvatar
mkdir %BASE%\System\Display\AppTooltip

:: ===== FEEDBACK =====
mkdir %BASE%\System\Feedback\AppAlert
mkdir %BASE%\System\Feedback\AppToast
mkdir %BASE%\System\Feedback\AppSpinner
mkdir %BASE%\System\Feedback\AppSkeleton

:: ===== LAYOUT =====
mkdir %BASE%\System\Layout\AppCard
mkdir %BASE%\System\Layout\AppPanel
mkdir %BASE%\System\Layout\AppModal
mkdir %BASE%\System\Layout\AppDivider

:: ===== NAVIGATION =====
mkdir %BASE%\System\Navigation\AppTabs
mkdir %BASE%\System\Navigation\AppBreadcrumb
mkdir %BASE%\System\Navigation\AppPagination

:: ===== DATA DISPLAY =====
mkdir %BASE%\DataDisplay\AppTable
mkdir %BASE%\DataDisplay\AppDataGrid
mkdir %BASE%\DataDisplay\AppList
mkdir %BASE%\DataDisplay\AppEmptyState

:: ===== COMPOSITE =====
mkdir %BASE%\Composite\SearchBox
mkdir %BASE%\Composite\FilterPanel
mkdir %BASE%\Composite\ConfirmDialog
mkdir %BASE%\Composite\FormSection

:: ===== FEATURE =====
mkdir %BASE%\Feature\Project\ProjectCard
mkdir %BASE%\Feature\Project\ProjectForm
mkdir %BASE%\Feature\Project\ProjectTree

mkdir %BASE%\Feature\User\UserAvatar
mkdir %BASE%\Feature\User\UserProfileCard


echo Creating razor files...

for /r %BASE% %%d in (.) do (
    if not exist "%%d\%%~nd.razor" (
        echo @* %%~nd component *@ > "%%d\%%~nd.razor"
        echo.>> "%%d\%%~nd.razor"
        echo ^<div class="%%~nd"^> >> "%%d\%%~nd.razor"
        echo     %%~nd component >> "%%d\%%~nd.razor"
        echo ^</div^> >> "%%d\%%~nd.razor"
    )

    if not exist "%%d\%%~nd.razor.cs" (
        echo using Microsoft.AspNetCore.Components; > "%%d\%%~nd.razor.cs"
        echo.>> "%%d\%%~nd.razor.cs"
        echo public partial class %%~nd : ComponentBase >> "%%d\%%~nd.razor.cs"
        echo { >> "%%d\%%~nd.razor.cs"
        echo } >> "%%d\%%~nd.razor.cs"
    )

    if not exist "%%d\%%~nd.razor.css" (
        echo .%%~nd { > "%%d\%%~nd.razor.css"
        echo } >> "%%d\%%~nd.razor.css"
    )
)

echo Done.
pause