using System.Collections.Generic;
using System.Data;

namespace FM132API.Models
{
    public class Inspection
    {
        public string Type { get; set; }

        public string TorqueSeal { get; set; }
        public string Plaster { get; set; }
        public string BrokenCorners { get; set; }
        public string BrokenEdges { get; set; }
        public string IdLegible { get; set; }
        public string ProgramColor { get; set; }
        public string Bushings { get; set; }
        public string Deterioration { get; set; }
        public string WitnessLine { get; set; }
        public string ColorCode { get; set; }
        public string LevelingInspection { get; set; }
        public string Observation { get; set; }
        public string ToolNotification { get; set; }
        public string ToolOrder { get; set; }

        public string Owner { get; set; }
        public string OwnerMail { get; set; }

        public string[] PartNumberLines { get; set; }

        public string[] SerialNumbersLines { get; set; }

        public string[] ToolCodesLines { get; set; }
        public List<Tool> ToolLines { get; set; }

        public void FormatArrayLines()
        {
            ToolLines = new List<Tool>();

            for (int i = 0; i < PartNumberLines.Length; i++)
            {
                Tool tool = new Tool();

                tool.PartNumber = PartNumberLines[i];
                tool.SerialNumber = SerialNumbersLines[i];
                tool.ToolCode = ToolCodesLines[i];

                ToolLines.Add(tool);
            }
        }

        public DataTable Tools { get; set; }

    }
}