namespace Web.Utils.LabAnalyzer;

public class LabReportViewModel
{
    public LabReportResult? Result { get; set; }
    public string? ErrorMessage { get; set; }
    public bool HasResult => Result != null;
}

public class LabReportResult
{
    public ReportMetadata? report_metadata { get; set; }
    public List<LabParameter>? parameters { get; set; }
    public List<CriticalFlag>? critical_flags { get; set; }
    public List<string>? patterns_detected { get; set; }
    public string? overall_summary { get; set; }
    public List<FollowupTest>? recommended_followup_tests { get; set; }
    public string? disclaimer { get; set; }
}

public class ReportMetadata
{
    public string? lab_name { get; set; }
    public string? patient_name { get; set; }
    public string? patient_age { get; set; }
    public string? patient_gender { get; set; }
    public string? sample_date { get; set; }
    public string? print_date { get; set; }
    public string? referring_doctor { get; set; }
    public string? test_type { get; set; }
    public string? test_type_arabic { get; set; }
    public string? sample_type { get; set; }
    public string? image_quality { get; set; }
    public string? inferred_fields_note { get; set; }
}

public class LabParameter
{
    public string? abbreviation { get; set; }
    public string? full_name_english { get; set; }
    public string? full_name_arabic { get; set; }
    public string? what_it_measures { get; set; }
    public string? result { get; set; }
    public string? unit { get; set; }
    public string? reference_range { get; set; }
    public string? reference_source { get; set; }
    public bool inferred { get; set; }
    public string? status { get; set; }
    public string? status_arabic { get; set; }
    public string? risk_if_high { get; set; }
    public string? risk_if_low { get; set; }
    public string? interpretation { get; set; }
    public string? clinical_significance { get; set; }
}

public class CriticalFlag
{
    public string? parameter { get; set; }
    public string? reason { get; set; }
    public string? urgency { get; set; }
}

public class FollowupTest
{
    public string? test_name { get; set; }
    public string? reason { get; set; }
}
