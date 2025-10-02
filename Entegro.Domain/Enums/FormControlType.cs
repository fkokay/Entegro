using System.ComponentModel.DataAnnotations;

namespace Entegro.Domain.Enums
{
    public enum FormControlType
    {
        [Display(Name = "Açılır Liste")]
        Dropdown = 0,

        [Display(Name = "Radyo Düğmesi Listesi")]
        RadioButtonList = 1,

        [Display(Name = "Onay Kutusu")]
        Checkbox = 2,

        [Display(Name = "Metin Kutusu")]
        TextBox = 3,

        [Display(Name = "Çok Satırlı Metin Kutusu")]
        MultilineTextBox = 4,

        [Display(Name = "Takvim")]
        DatePicker = 5,

        [Display(Name = "Dosya Yükleme")]
        FileUpload = 6,

        [Display(Name = "Kutular (Renk ve Görüntü)")]
        ColorOrImageBox = 7
    }

}
