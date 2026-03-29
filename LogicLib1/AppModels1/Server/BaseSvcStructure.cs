using System.ComponentModel.DataAnnotations;

namespace LogicLib1.AppModels1.Server.Services;

public class BaseSvcStructure
{
    [Required] public string              Uid           { get; set; } = "";
               public decimal             Cost          { get; set; }
               public string              Details       { get; set; } = "";
               public BaseSchedSlot       ScheduleSlots { get; set; } = new BaseSchedSlot();
}