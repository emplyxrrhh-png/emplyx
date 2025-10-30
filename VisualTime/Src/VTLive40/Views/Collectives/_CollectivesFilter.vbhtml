@Imports Robotics.Base.DTOs

@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")
End Code
<script>
    window.Collectives = { i18n: JSON.parse('@Html.Raw(Robotics.VTBase.roJSONHelper.SerializeNewtonSoft(labels).Replace("'", "\'")) ') };
</script>
<div class="mt-3 d-flex gap-3">
    <style>
        
    </style>
    <section id="collectiveHistoric" class="w-25">
        <div id="collectiveHistoricGrid"></div>
    </section>
    <section id="collectiveFilter" class="w-75 flex-grow-1">
        @Html.Partial("_stateBarIcons", New With {.Section = "CollectivesDefinition", .Title = labels("Collectives#DefinitionHeader"), .IsHeader = True, .ShowClose = False, .ShowUndo = True, .ShowDelete = True, .ShowSave = True, .ShowVisualize = True})
        <div class="mt-3" style="width: 95%;margin: 0 auto;">

            <div class="d-flex align-items-center gap-5">
                <div class="d-flex mb-4 align-items-center flex-grow-1">
                    <div style="width: 100px;">
                        @Html.Raw(labels("Collectives#lblDesc"))
                    </div>
                    <div class="w-100">
                        @(Html.DevExtreme().TextBox() _
.OnValueChanged("changeCollectiveDefinitionDesc") _
.ID("txtHistoricDescription") _
.MaxLength(256) _
                            )
                    </div>
                </div>
                <div class="d-flex mb-2 align-items-center">
                    <div style="width: 100px;">
                        @Html.Raw(labels("Collectives#DefinitionDateLabel"))
                    </div>
                    <div id="datepicker"></div>
                </div>
                <div><p id="historicLastChange" class="fst-italic">@Html.Raw(labels("Collectives#ModifiedBy1")) <span id="lastChange_name"></span> @Html.Raw(labels("Collectives#ModifiedBy2")) <span id="lastChange_date"></span> @Html.Raw(labels("Collectives#ModifiedBy3")) <span id="lastChange_time"></span></p></div>
            </div>

            <div class="filter-container">
                <div class="d-flex flex-column gap-4 align-items-center">
                    <div id="filterBuilder"></div>
                </div>
                <div class="results" style="display: flex; align-items: center; padding-bottom: 40px; max-width: 22vw;">
                    <p id="filterText" style="background-color: #eef8f9; border-radius: 5px; padding: 20px; height: auto; border: 2px solid gray; margin-top:10px; opacity:0;"></p>
                </div>
            </div>
            <div id="employeesPopup" style="display:none;">
            </div>
            
        </div>
    </section>
</div>