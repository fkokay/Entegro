using System.ComponentModel.DataAnnotations;

namespace Entegro.Domain.Enums
{
    public enum FormControlType
    {
        [Display(Name = "Açılır Liste")]
        Dropdown = 1,

        [Display(Name = "Radyo Düğmesi Listesi")]
        RadioButtonList = 2,

        [Display(Name = "Onay Kutusu")]
        Checkbox = 3,

        [Display(Name = "Metin Kutusu")]
        TextBox = 4,

        [Display(Name = "Çok Satırlı Metin Kutusu")]
        MultilineTextBox = 10,

        [Display(Name = "Takvim")]
        DatePicker = 20,

        [Display(Name = "Dosya Yükleme")]
        FileUpload = 30,

        [Display(Name = "Kutular (Renk ve Görüntü)")]
        ColorOrImageBox = 40
    }

}
