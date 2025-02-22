namespace Services.Model
{
    public class WorkFlowStepModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsApproveFinalStep { get; set; }
        public bool IsLastStep { get; set; }
        public bool StepIsManual { get; set; }
        //public int? RejectNextStepId { get; set; }
        //public int? ApproveNextStepId { get; set; }
        //public int? ReturnToCorrectionNextStepId { get; set; }
    }
}
