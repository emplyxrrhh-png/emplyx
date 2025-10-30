var survey = null;
var surveyResultNode = null;
var responses = null;



function paintResults(idEmployee) {

    if (idEmployee == 0) {
        $.ajax({
            type: "POST",
            url: BASE_URL + 'Surveys/GetSurveyResponses',
            dataType: "json",
            data: { idSurvey: currentView.Id },
            success: function (data) {
                var resultArray = null;
                /* registerCustom();*/
                if (data.Data != null) {
                    resultArray = data.Data.map(a => JSON.parse(a));
                }
                else {
                    resultArray = [];
                }
                var options = {
                    haveCommercialLicense: true,
                    labelTruncateLength: 27
                };
                responses = resultArray;

                switchModeExternal($("#radioGroupMode").dxRadioGroup("instance").option("value"));
                //var visPanel = new SurveyAnalytics.VisualizationPanel(survey.getAllQuestions(), resultArray, { haveCommercialLicense: true, labelTruncateLength: 27 });
                //visPanel.showHeader = true;

                //$("#loadingIndicator").hide();

                //surveyResultNode.innerHTML = "";
                //visPanel.render(surveyResultNode);
            },
            error: function () { DevExpress.ui.notify("No se han podido cargar los resultados de la encuesta", "error", 2000); }
        });
    }
    else {

        var idEmployees = resultsSelected.map(a => a.Id);

        $.ajax({
            type: "POST",
            url: BASE_URL + 'Surveys/GetSurveyResponsesByIdEmployee',
            dataType: "json",
            data: { idSurvey: currentView.Id, idEmployees: idEmployees },
            success: function (data) {
                var resultArray = null;
                /* registerCustom();*/
                if (data.Data != null) {
                    resultArray = data.Data.map(a => JSON.parse(a));
                }
                else {
                    resultArray = [];
                }
                var options = {
                    haveCommercialLicense: true,
                    labelTruncateLength: 27
                };
                responses = resultArray;
                switchModeExternal($("#radioGroupMode").dxRadioGroup("instance").option("value"));
                //var visPanel = new SurveyAnalytics.VisualizationPanel(survey.getAllQuestions(), resultArray, { haveCommercialLicense: true, labelTruncateLength: 27 });
                //visPanel.showHeader = true;

                //$("#loadingIndicator").hide();

                //surveyResultNode.innerHTML = "";
                //visPanel.render(surveyResultNode);
            },
            error: function () { DevExpress.ui.notify("No se han podido cargar los resultados de la encuesta", "error", 2000); }
        });
    }
}

function getSurveyResponses(content) {

    var json = content.replace("emotionsratings", "rating");

  survey = new Survey.Model(json);

     surveyResultNode = document.getElementById("results");
    surveyResultNode.innerHTML = "";

  
    setTimeout(function () {
        paintResults(0);
    }, 100);
}


   




