using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EvelynStores.Core.Models
{
    public class UnitModel
    {
        [Required(ErrorMessage = "Unit name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Short name is required")]
        public string ShortName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
