namespace ScrumStatsHelper.DTOs
{
    public class Sprint
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime startDate { get; set; }
        public DateTime finishDate { get; set; }
        public List<TaskBoardItem> taskBoardItems { get; set; }
    }

    public class TaskBoardItem
    {
        public int id { get; set; }
        public string url { get; set; }
        public SprintItem sprintItem { get; set; }

        public TaskBoardItem() {
            id = 0;
            url = string.Empty;
            sprintItem = new SprintItem(id);
        }
    }

    public class TaskBoardItems
    {
        public DTOs.TaskBoardItem[] workItems { get; set; }
    }

    public class SprintItem
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int Iteration { get; set; }
        public bool Done { get; set; }
        public double MaintenanceMinutes { get; set; }
        public double EvolutionMinutes { get; set; }
        public double SpecificMinutes { get; set; }
        public double OtherNaturesMinutes { get; set; }
        public int Effort { get; set; }
        public string Nature { get; set; }
        public int Order { get; set; }

        public List<TimeInput>? TimeInputs;

        public List<ChildItem>? ChildItems;

        public SprintItem(int itemId)
        {
            ItemId = 0;
            ItemName = string.Empty;
            Iteration = 0;
            Nature = string.Empty;
            Effort = 0;
            Done = false;
            Order = 0;
            TimeInputs = null;
            MaintenanceMinutes = 0;
            EvolutionMinutes = 0;
            SpecificMinutes = 0;
            OtherNaturesMinutes = 0;
            ChildItems = null;
        }
    }

    public class ChildItem
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int Iteration { get; set; }
        public string Nature { get; set; }

        public List<TimeInput>? TimeInputs;

        public ChildItem(int itemId)
        {
            ItemId = itemId;
            ItemName = string.Empty;
            Iteration = 0;
            Nature = string.Empty;
            TimeInputs = null;
        }
    }

    public class TimeInput
    {
        public double duration { get; set; }
        public DateOnly date { get; set; }
        public string activity { get; set; }
    }

}
