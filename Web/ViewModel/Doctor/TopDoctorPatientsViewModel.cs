namespace Web.ViewModel.Doctor
{
    

    public class TopDoctorPatientsViewModel
    {
        public int DoctorId { get; set; }

        public string DoctorName { get; set; } = string.Empty;

        public string Specialization { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public decimal? ConsultationFee { get; set; }

        public int PatientsCount { get; set; }
    }
}
