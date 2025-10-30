using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using ScrumStatsHelper.DTOs;
using System.Configuration;
using System.Drawing;
using System;

namespace ScrumStatsHelper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void AnalyzeSprint(object sender, EventArgs e)
        {
            txtLog.Text = string.Empty;
            txtAnalysis.Text = string.Empty;
            txtLog.Text += "Inicando análisis del Sprint ..." + Environment.NewLine;
            TaskBoardItem[] taskBoardItems = GetSprintTaskBoardItems((int)cbSprint.SelectedItem);
            txtLog.Text += $"Sprint {cbSprint.SelectedText} tiene un total de {taskBoardItems.Count().ToString()} PBIs y/o Spykes" + Environment.NewLine;
            List<SprintItem> sprint = FillSprintItemsDetails(taskBoardItems);
            AnalyzeSprint(sprint);
        }

        public TaskBoardItem[] GetSprintTaskBoardItems(int sprint)
        {
            TaskBoardItem[] result = new TaskBoardItem[0];
            string organization = "cegid";
            string project = "VisualTime";
            string iterationPath = $"VisualTime\\Sprint {sprint}";
            string personalAccessToken = ConfigurationManager.AppSettings["DevOpsToken"];
            string apiVersion = "6.0";

            using (HttpClient client = new HttpClient())
            {
                // Configurar la solicitud y la autenticación
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(String.Format("{0}:{1}", "", personalAccessToken))));

                // Construir la URL de la API
                string url = $"https://dev.azure.com/{organization}/{project}/_apis/wit/wiql?api-version={apiVersion}";

                string wiqlQuery = $"SELECT [System.Id], [System.Title] FROM WorkItems WHERE [System.IterationPath] = '{iterationPath}' AND ([System.WorkItemType] = 'Product Backlog Item' OR [System.WorkItemType] = 'Spike')";

                var requestBody = new StringContent(JsonConvert.SerializeObject(new { query = wiqlQuery }), Encoding.UTF8, "application/json");

                // Realizar la solicitud POST y obtener la respuesta
                HttpResponseMessage response = client.PostAsync(url, requestBody).Result;
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    // Leer el contenido de la respuesta como una cadena JSON
                    string responseBody = response.Content.ReadAsStringAsync().Result;

                    // Deserializar manualmente la cadena JSON para obtener los IDs de los workitems
                    TaskBoardItems taskboard = JsonConvert.DeserializeObject<TaskBoardItems>(responseBody);
                    result = taskboard.workItems;
                }

                return result;
            }
        }

        public List<SprintItem> FillSprintItemsDetails(TaskBoardItem[] taskboardItems)
        {
            List<SprintItem> sprintMainItems = new List<SprintItem>();

            txtLog.Text += $"Iniciando análisis de los ítems del Sprint ..." + Environment.NewLine;

            //APIs settings
            string organization = "cegid";
            string Project = "VisualTime";
            string devOpsapiVersion = "1.0";

            //Conexión 7Pace
            string baseUri7PAce;
            string relativePath7Pace;
            string requestUri7Pace = string.Empty;
            string responseBody7Pace;
            baseUri7PAce = $"https://{organization}.timehub.7pace.com/api/odata/v3.2";
            HttpClient httpClient7Pace = new HttpClient();
            HttpResponseMessage response7Pace;
            httpClient7Pace.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigurationManager.AppSettings["7PaceToken"]);

            //Conexión devOps
            HttpClient httpClientdevOps = new HttpClient();
            HttpResponseMessage responsedevops;
            string responseBodydevOps;
            httpClientdevOps.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClientdevOps.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", ConfigurationManager.AppSettings["DevOpsToken"]))));
            httpClientdevOps.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true };
            string urldevOps;

            //General
            string nature = string.Empty;
            string title;
            string state = string.Empty;
            int iteration = 0;
            int effort;
            List<int> lChildren;
            double totalItemTimeInput = 0;
            JObject jsonResponse;

            double maintenanceMinutes = 0;
            double evolutionMinutes = 0;
            double specificMinutes = 0;
            double otherOrNoneMinutes = 0;
            int itemorder = 4;

            foreach (TaskBoardItem taskboardItem in taskboardItems)
            {
                //NATURE
                urldevOps = $"https://dev.azure.com/{organization}/{Project}/_apis/wit/workitems/{taskboardItem.id}?$expand=all&api-version={devOpsapiVersion}";
                responsedevops = httpClientdevOps.GetAsync(urldevOps).Result;
                responsedevops.EnsureSuccessStatusCode();

                title = string.Empty;
                effort = 0;
                // Verifica el código de estado de la respuesta
                if (responsedevops.IsSuccessStatusCode)
                {
                    // Lee el contenido de la respuesta como una cadena de texto
                    responseBodydevOps = responsedevops.Content.ReadAsStringAsync().Result;
                    jsonResponse = JObject.Parse(responseBodydevOps);
                    title = (string?)jsonResponse["fields"]?["System.Title"] ?? string.Empty;
                    effort = (int?)jsonResponse["fields"]?["Microsoft.VSTS.Scheduling.Effort"] ?? 0;
                    state = (string?)jsonResponse["fields"]?["System.State"] ?? string.Empty;
                    nature = (string?)jsonResponse["fields"]?["Custom.Nature"] ?? string.Empty;
                }

                txtLog.Text += $"PBI {taskboardItem.id.ToString()} - {title}" + Environment.NewLine;
                taskboardItem.sprintItem.ItemName = title;
                taskboardItem.sprintItem.Effort = effort;
                taskboardItem.sprintItem.Done = (state == "Done") ? true : false;

                taskboardItem.sprintItem.Order = itemorder;
                if (title.ToLower().Contains("tickets sprint")) taskboardItem.sprintItem.Order = 1;
                if (title.ToLower().Contains("unplanned sprint")) taskboardItem.sprintItem.Order = 2;
                if (title.ToLower().Contains("soporte planificado")) taskboardItem.sprintItem.Order = 3;

                switch (nature)
                {
                    case "4. Maintenance":
                        taskboardItem.sprintItem.Nature = "MANT";
                        break;
                    case "2. Product Evolution":
                        taskboardItem.sprintItem.Nature = "EVO";
                        break;
                    case "6. Specific":
                        taskboardItem.sprintItem.Nature = "SPC";
                        break;
                    default:
                        taskboardItem.sprintItem.Nature = "OTHER";
                        break;
                }

                lChildren = new List<int>();
                relativePath7Pace = $"workItems({taskboardItem.id})/Children";
                requestUri7Pace = $"{baseUri7PAce}/{relativePath7Pace}";

                response7Pace = httpClient7Pace.GetAsync(requestUri7Pace).Result;
                response7Pace.EnsureSuccessStatusCode();

                // Verifica el código de estado de la respuesta
                if (response7Pace.IsSuccessStatusCode)
                {
                    // Lee el contenido de la respuesta como una cadena de texto
                    responseBody7Pace = response7Pace.Content.ReadAsStringAsync().Result;
                    jsonResponse = JObject.Parse(responseBody7Pace);

                    lChildren.Add(taskboardItem.id);
                    foreach (JObject item in jsonResponse["value"])
                    {
                        if ((int)item["System_Id"] != null) lChildren.Add((int)item["System_Id"]);
                    }

                    txtLog.Text += $"----->{(lChildren.Count() - 1).ToString()} hijos" + Environment.NewLine;

                    maintenanceMinutes = 0;
                    evolutionMinutes = 0;
                    specificMinutes = 0;
                    otherOrNoneMinutes = 0;

                    foreach (int iworkItem in lChildren)
                    {
                        //NATURE
                        urldevOps = $"https://dev.azure.com/{organization}/{Project}/_apis/wit/workitems/{iworkItem}?api-version={devOpsapiVersion}";
                        responsedevops = httpClientdevOps.GetAsync(urldevOps).Result;
                        //responsedevops.EnsureSuccessStatusCode();

                        nature = string.Empty;
                        // Verifica el código de estado de la respuesta
                        if (responsedevops.IsSuccessStatusCode)
                        {
                            // Lee el contenido de la respuesta como una cadena de texto
                            responseBodydevOps = responsedevops.Content.ReadAsStringAsync().Result;
                            jsonResponse = JObject.Parse(responseBodydevOps);
                            nature = (string?)jsonResponse["fields"]?["Custom.Nature"] ?? string.Empty;
                            string iterationPath = (string?)jsonResponse["fields"]?["System.IterationPath"] ?? string.Empty;
                            iteration = iterationPath.Contains("VisualTime\\Sprint ") ? int.Parse(iterationPath.Replace("VisualTime\\Sprint ", "")) : 0;
                        }

                        if (iteration == (int)cbSprint.SelectedItem)
                        {
                            if (taskboardItem.id == iworkItem)
                            {
                                taskboardItem.sprintItem.ItemName = title;
                            }

                            txtLog.Text += $"----------> Item {iworkItem}: Nature {nature} " + Environment.NewLine;

                            //TIEMPO
                            //Dado un id, tiempo imputado a el y todos su shijos      workItemsHierarchyAllLevels(1163876) / AllWorklogs
                            //Dado un od, su tiempo      workLogsOnly?$apply=filter(WorkItemId eq {sprintWorkItem.id})
                            relativePath7Pace = $"workLogsOnly?$apply=filter(WorkItemId eq {iworkItem})";
                            requestUri7Pace = $"{baseUri7PAce}/{relativePath7Pace}";

                            response7Pace = httpClient7Pace.GetAsync(requestUri7Pace).Result;
                            response7Pace.EnsureSuccessStatusCode();

                            if (response7Pace.IsSuccessStatusCode)
                            {
                                //Sumo todas las duraciones imputadas a ese workitem
                                responseBody7Pace = response7Pace.Content.ReadAsStringAsync().Result;
                                jsonResponse = JObject.Parse(responseBody7Pace);
                                totalItemTimeInput = 0;

                                JToken? value = jsonResponse["value"];
                                if (value != null && value.Type == JTokenType.Array)
                                {
                                    foreach (JObject item in value)
                                    {
                                        double periodLength = (double)item["PeriodLength"];
                                        txtLog.Text += $"---------------> Time input: {(periodLength / 3600).ToString("0.00")} horas" + Environment.NewLine;
                                        if (periodLength != null) totalItemTimeInput = totalItemTimeInput + periodLength;
                                    }
                                    txtLog.Text += $"---------------> Total {iworkItem}: {(totalItemTimeInput / 3600).ToString("0.00")} horas" + Environment.NewLine;
                                }
                            }

                            switch (nature)
                            {
                                case "4. Maintenance":
                                    maintenanceMinutes += totalItemTimeInput / 60;
                                    break;
                                case "2. Product Evolution":
                                    evolutionMinutes += totalItemTimeInput / 60;
                                    break;
                                case "6. Specific":
                                    specificMinutes += totalItemTimeInput / 60;
                                    break;
                                default:
                                    otherOrNoneMinutes += totalItemTimeInput / 60;
                                    break;
                            }
                        }
                    }
                    taskboardItem.sprintItem.MaintenanceMinutes = maintenanceMinutes;
                    taskboardItem.sprintItem.EvolutionMinutes = evolutionMinutes;
                    taskboardItem.sprintItem.SpecificMinutes = specificMinutes;
                    taskboardItem.sprintItem.OtherNaturesMinutes = otherOrNoneMinutes;

                    txtLog.Text += $"----->Mantenimiento: {(maintenanceMinutes / 60).ToString("0.00")} horas" + Environment.NewLine;
                    txtLog.Text += $"----->Evolutio: {(evolutionMinutes / 60).ToString("0.00")} horas" + Environment.NewLine;
                    txtLog.Text += $"----->Específico: {(specificMinutes / 60).ToString("0.00")} horas" + Environment.NewLine;
                    txtLog.Text += $"----->Otras: {(otherOrNoneMinutes / 60).ToString("0.00")} horas" + Environment.NewLine;

                }
                sprintMainItems.Add(taskboardItem.sprintItem);
                itemorder++;
            }
            return sprintMainItems;
        }

        public void AnalyzeSprint(List<SprintItem> result)
        {
            double totalHoras = 0;
            double totalSP = 0;
            double ratioSP = 0;
            StringBuilder sb = new StringBuilder();
            string line = string.Empty;
            string finalizado = string.Empty;
            foreach (DTOs.SprintItem item in result.OrderBy(item => item.Order))
            {
                ratioSP = 0;
                finalizado = item.Done ? "X" : "";
                totalHoras = (item.MaintenanceMinutes + item.EvolutionMinutes + item.SpecificMinutes + item.OtherNaturesMinutes) / 60;
                if (item.Effort != 0) ratioSP = totalHoras / item.Effort;
                line = $"{item.ItemName}\t{finalizado}\t{(totalHoras).ToString("0.0")}\t{item.Effort.ToString("0")}\t{item.Nature.ToString()}\t{(ratioSP).ToString("0.0")}";
                line += $"\t";
                line += $"\t{(item.MaintenanceMinutes / 60).ToString("0.0")}\t{(item.EvolutionMinutes / 60).ToString("0.0")}\t{(item.SpecificMinutes / 60).ToString("0.0")}\t{(item.OtherNaturesMinutes / 60).ToString("0.0")}";
                line += $"\t";
                if (item.Order < 4)
                {
                    //PBIS de tickets. SP estimados en base a horas dedicadas a razón de 1SP = 3 horas
                    line += $"\t{(item.MaintenanceMinutes / 60 / 3).ToString("0")}\t{(item.EvolutionMinutes / 60 / 3).ToString("0")}\t{(item.SpecificMinutes / 60 / 3).ToString("0")}\t{(item.OtherNaturesMinutes / 60 / 3).ToString("0")}";
                }
                else
                {
                    switch (item.Nature)
                    {
                        case "MANT":
                            line += $"\t{(item.Effort).ToString("0")}\t0\t0\t0";
                            break;
                        case "EVO":
                            line += $"\t0\t{(item.Effort).ToString("0")}\t0\t0";
                            break;
                        case "SPC":
                            line += $"\t0\t0\t{(item.Effort).ToString("0")}\t0";
                            break;
                        case "OTHER":
                            line += $"\t0\t0\t0\t{(item.Effort).ToString("0")}";
                            break;
                    }
                }
                sb.AppendLine(line);
            }

            txtAnalysis.Text = sb.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            this.WindowState = FormWindowState.Maximized;
            DateTime startDate = new DateTime(2023, 06, 28);
            DateTime today = DateTime.Now;

            int currentSprint = Helper.GetSprintNumberOnDate(today);

            //Cargar combo de sprints
            int index = 0;
            for (int i = (currentSprint - 5); i <= currentSprint + 1; i++)
            {
                cbSprint.Items.Add(i);
                //    if (i == currentSprint) cbSprint.SelectedIndex = index;
                index++;
            }

            cbSprint.SelectedItem = currentSprint;

            Tuple<DateTime, DateTime> sprintDates = Helper.GetSprintDates(currentSprint);

            dtSprintStart.Value = sprintDates.Item1;
            dtSprintEnd.Value = sprintDates.Item2;

        }

        private void cbSprint_SelectedIndexChanged(object sender, EventArgs e)
        {
            Tuple<DateTime, DateTime> sprintDates = Helper.GetSprintDates((int)cbSprint.SelectedItem);

            dtSprintStart.Value = sprintDates.Item1;
            dtSprintEnd.Value = sprintDates.Item2;
        }
    }

}

