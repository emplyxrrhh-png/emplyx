@Imports Robotics.Base.DTOs

@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")
End Code

<section id="collectiveEmployees" class="mt-3">
    <div class="panHeader2 panBottomMargin">
        <span class="panelTitleSpan">
            <span id="">Usuarios</span>
        </span>
    </div>
    <div style="min-height:15px"></div>
    <div style="width: 95%;margin: 0 auto;">
        <div class="employees-container">
            <div style="display: flex; gap: 10px; align-items: center;">
                <div id="datepicker"></div>
                <div id="refreshemployees"></div>
            </div>
            <div id="employeesDatagrid"></div>
        </div>
    </div>
</section>