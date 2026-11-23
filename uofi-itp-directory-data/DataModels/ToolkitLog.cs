using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uofi_itp_directory_data.DataModels {
    public class ToolkitLog : BaseDataItem {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public bool Success { get; set; }

        public string SuccessMessage => Success ? "Success" : "Failure";

        public bool IsLast { get; set; }

        public ToolkitTypeEnum ToolkitType { get; set; }

        public string Timestamp { get; set; } = "";

        public string Log { get; set; } = "";

        public int Page { get; set; }

        public string Schema { get; set; } = "";

        public override string ToString() {
            return $"ToolkitLog(Success={Success}, IsLast={IsLast}, ToolkitType={ToolkitType}, Timestamp={Timestamp}, Page={Page}, DateTime={LastUpdated})";
        }

        public ToolkitLog Advance() {
            if (IsLast) {
                return new ToolkitLog() {
                    Success = false,
                    IsLast = false,
                    IsActive = true,
                    ToolkitType = ToolkitType,
                    Timestamp = LastUpdated.ToString("yyyyMMddHHmm"),
                    Schema = Schema,
                    Page = 0
                };
            }
            return new ToolkitLog() {
                Success = false,
                IsLast = false,
                IsActive = true,
                ToolkitType = ToolkitType,
                Timestamp = Timestamp,
                Page = Page + 1
            };
        }

        public static ToolkitLog Start(ToolkitTypeEnum toolkitTypeEnum) {
            return new ToolkitLog() {
                Success = false,
                IsLast = false,
                IsActive = true,
                ToolkitType = toolkitTypeEnum,
                Timestamp = "200001010101",
                Log = "",
                Page = 0
            };
        }
    }
}
