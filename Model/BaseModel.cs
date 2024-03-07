using System.ComponentModel;

namespace SimpleCRUD.Model
{
    public class BaseModel
    {
        [DisplayName("Status")]
        public int Status { get; set; }

        [DisplayName("Created By")]
        public int CreatedBy { get; set; }

        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Updated By")]
        public int UpdatedBy { get; set; }

        [DisplayName("Updated Date")]
        public DateTime UpdatedDate { get; set; }
    }
}
